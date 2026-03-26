using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour{
    // instance unique
    public static GameManager Instance { get; private set; }

    // nom de la scène accueil
    [Header("Nom de la scène Accueil")]
    [SerializeField] private string titleSceneName = "Accueil";

    // UI game over
    [Header("UI Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    // UI pause
    [Header("UI Pause")]
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject pausePanel;

    // UI temps
    [Header("UI Temps (optionnel)")]
    [SerializeField] private TextMeshProUGUI timeText;

    // UI victoire
    [Header("UI Victoire (optionnel)")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryDetailsText;

    // textes UI (life/capsules)
    private static TextMeshProUGUI lifeText;
    private static TextMeshProUGUI capsulesText;

    // nombre de kills
    public int kills { get; private set; } = 0;

    // timer du niveau
    private float levelTimer = 0f;

    // états du jeu
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isFinished = false;

    private void Awake(){
        // singleton
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start(){
        // remet le temps normal
        Time.timeScale = 1f;

        // reset états
        isPaused = false;
        isGameOver = false;
        isFinished = false;

        // reset score/timer
        kills = 0;
        levelTimer = 0f;

        // cache les panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseText != null) pauseText.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // récupère le texte Life
        GameObject lifeObj = GameObject.Find("Life");
        if (lifeObj != null) lifeText = lifeObj.GetComponent<TextMeshProUGUI>();

        // récupère le texte Capsules
        GameObject capsObj = GameObject.Find("Capsules");
        if (capsObj != null) capsulesText = capsObj.GetComponent<TextMeshProUGUI>();

        // récupère le texte Time si non assigné
        if (timeText == null){
            GameObject timeObj = GameObject.Find("Time");
            if (timeObj != null) timeText = timeObj.GetComponent<TextMeshProUGUI>();
        }

        // affiche le temps au départ
        UpdateTimeUI();
    }

    private void Update(){
        // toggle pause avec K
        if (Input.GetKeyDown(KeyCode.K)){
            if (isGameOver || isFinished) return;
            SetPause(!isPaused);
        }

        // incrémente le timer
        if (!isPaused && !isGameOver && !isFinished){
            levelTimer += Time.deltaTime;
            UpdateTimeUI();
        }
    }

    // met à jour la vie
    public static void UpdateLifeText(float life){
        if (lifeText != null) lifeText.text = "Life : " + life;
    }

    // met à jour les capsules
    public static void UpdateCapsulesText(int capsules){
        if (capsulesText == null) return;
        string label = (capsules == 1) ? "Capsule" : "Capsules";
        capsulesText.text = label + " : " + capsules;
    }

    // met à jour le temps affiché
    private void UpdateTimeUI(){
        if (timeText == null) return;

        int totalSeconds = Mathf.FloorToInt(levelTimer);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0) timeText.text = $"Temps : {hours:00}:{minutes:00}:{seconds:00}";
        else timeText.text = $"Temps : {minutes:00}:{seconds:00}";
    }

    // ajoute un kill
    public static void AddKill(int amount = 1){
        if (Instance == null) return;
        if (Instance.isGameOver || Instance.isFinished) return;
        Instance.kills += amount;
    }

    // retourne le temps du niveau
    public static float GetLevelTime(){
        if (Instance == null) return 0f;
        return Instance.levelTimer;
    }

    // calcule le score final
    public static int ComputeScore(int capsules){
        if (Instance == null) return 0;

        float timeTaken = Instance.levelTimer;
        int k = Instance.kills;

        int baseScore = 5000;
        int capsuleBonus = capsules * 250;
        int killBonus = k * 200;
        int timePenalty = Mathf.RoundToInt(timeTaken * 5f);

        int score = baseScore + capsuleBonus + killBonus - timePenalty;
        return Mathf.Max(0, score);
    }

    // affiche la victoire
    public void Victory(int capsules){
        if (isFinished || isGameOver) return;
        isFinished = true;

        // stop le jeu
        Time.timeScale = 0f;

        // affiche le panel victoire
        if (victoryPanel != null) victoryPanel.SetActive(true);

        // remplit les infos victoire
        if (victoryDetailsText != null){
            int totalSeconds = Mathf.FloorToInt(levelTimer);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            string timeStr = (hours > 0) ? $"{hours:00}:{minutes:00}:{seconds:00}" : $"{minutes:00}:{seconds:00}";
            int score = ComputeScore(capsules);

            string capLabel = (capsules == 1) ? "Capsule" : "Capsules";
            string killLabel = (kills == 1) ? "Kill" : "Kills";

            victoryDetailsText.text =
                $"VICTOIRE !\n\n" +
                $"Temps : {timeStr}\n" +
                $"{capLabel} : {capsules}\n" +
                $"{killLabel} : {kills}\n\n" +
                $"Score : {score}";
        }
    }

    // active le game over
    public static void GameOver(){
        if (Instance == null) return;

        Instance.isGameOver = true;
        Instance.isPaused = false;

        // affiche game over
        if (Instance.gameOverPanel != null) Instance.gameOverPanel.SetActive(true);

        // cache pause/victoire
        if (Instance.pauseText != null) Instance.pauseText.SetActive(false);
        if (Instance.pausePanel != null) Instance.pausePanel.SetActive(false);
        if (Instance.victoryPanel != null) Instance.victoryPanel.SetActive(false);

        // stop le jeu
        Time.timeScale = 0f;
    }

    // active ou désactive la pause
    public static void SetPause(bool pause){
        if (Instance == null) return;
        if (Instance.isGameOver || Instance.isFinished) return;

        Instance.isPaused = pause;

        // affiche pause
        if (Instance.pauseText != null) Instance.pauseText.SetActive(pause);
        if (Instance.pausePanel != null) Instance.pausePanel.SetActive(pause);

        // stop ou reprend
        Time.timeScale = pause ? 0f : 1f;
    }

    // bouton accueil
    public void BackToTitle() => GoToTitle();

    // charge la scène accueil
    public static void GoToTitle(){
        Time.timeScale = 1f;

        if (Instance != null){
            Instance.isPaused = false;
            Instance.isGameOver = false;
            Instance.isFinished = false;

            if (Instance.pausePanel != null) Instance.pausePanel.SetActive(false);
            if (Instance.pauseText != null) Instance.pauseText.SetActive(false);
            if (Instance.gameOverPanel != null) Instance.gameOverPanel.SetActive(false);
            if (Instance.victoryPanel != null) Instance.victoryPanel.SetActive(false);

            SceneManager.LoadScene(Instance.titleSceneName);
        }
        else{
            SceneManager.LoadScene("Accueil");
        }
    }

    // bouton quitter
    public void QuitButton() => QuitGame();

    // bouton restart
    public void RestartButton() => Restart();

    public static void QuitGame(){
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public static void Restart(){
        Time.timeScale = 1f;

        if (Instance != null){
            Instance.isPaused = false;
            Instance.isGameOver = false;
            Instance.isFinished = false;

            if (Instance.pausePanel != null) Instance.pausePanel.SetActive(false);
            if (Instance.pauseText != null) Instance.pauseText.SetActive(false);
            if (Instance.gameOverPanel != null) Instance.gameOverPanel.SetActive(false);
            if (Instance.victoryPanel != null) Instance.victoryPanel.SetActive(false);

            Instance.kills = 0;
            Instance.levelTimer = 0f;
        }

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // bouton next sur victoire
    public void LoadNextScene(){
        Time.timeScale = 1f;

        int idx = SceneManager.GetActiveScene().buildIndex;
        int next = idx + 1;

        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else SceneManager.LoadScene(titleSceneName);
    }
}