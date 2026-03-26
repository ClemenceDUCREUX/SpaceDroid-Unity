using UnityEngine;

public class ExitGate : MonoBehaviour{
    // évite le double déclenchement
    private bool triggered = false;

    private void OnTriggerEnter(Collider other){
        // ignore si déjà activé
        if (triggered) return;

        // vérifie si c'est le droid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats == null) return;

        // bloque les prochains triggers
        triggered = true;

        // lance la victoire et calcule le score
        if (GameManager.Instance != null){
            GameManager.Instance.Victory(stats.capsules);

            int score = GameManager.ComputeScore(stats.capsules);
            float timeTaken = GameManager.GetLevelTime();
            int kills = GameManager.Instance.kills;

            Debug.Log("[ExitGate] NIVEAU TERMINÉ !");
            Debug.Log($"Temps : {timeTaken:0.00}s | Capsules : {stats.capsules} | Kills : {kills} | SCORE : {score}");
        }
    }
}