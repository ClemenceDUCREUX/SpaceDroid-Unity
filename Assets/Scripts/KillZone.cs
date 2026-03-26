using UnityEngine;

public class KillZone : MonoBehaviour{
    private void OnTriggerEnter(Collider other){
        // ignore les météorites
        if (other.GetComponent<Meteorite>() != null)
            return;

        // cherche le StatsDroid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats == null)
            return;

        // vérifie que c’est bien le droid
        if (!stats.CompareTag("Droid"))
            return;

        // tue le droid
        stats.TakeDamage(stats.life);
    }
}