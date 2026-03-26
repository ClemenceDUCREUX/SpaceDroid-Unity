using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenManager : MonoBehaviour{
    // lance le niveau 1
    public void PlayStage1(){
        SceneManager.LoadScene("Stage 1");
    }

    // lance le niveau 2
    public void PlayStage2(){
        SceneManager.LoadScene("Stage 2");
    }

    // lance le niveau 3
    public void PlayStage3(){
        SceneManager.LoadScene("Stage 3");
    }

    // quitte le jeu
    public void QuitGame(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}