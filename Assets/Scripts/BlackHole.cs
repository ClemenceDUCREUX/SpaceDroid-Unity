using UnityEngine;

public class BlackHole : MonoBehaviour{
    private void OnTriggerEnter(Collider other){
        // évite les bugs avec soi-même
        if (other.gameObject == gameObject) return;

        // ignore la KillZone
        if (other.CompareTag("KillZone")) return;

        // vérifie si c'est le droid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats != null){
            // tue le droid
            stats.TakeDamage(stats.life);
            return;
        }

        // détruit l'objet avec Rigidbody
        if (other.attachedRigidbody != null){
            Destroy(other.attachedRigidbody.gameObject);
        }
        // détruit l'objet simple
        else{
            Destroy(other.gameObject);
        }
    }
}