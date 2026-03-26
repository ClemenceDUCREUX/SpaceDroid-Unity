using UnityEngine;

public class MoveEnemy : MonoBehaviour{
    // vitesse de déplacement
    [SerializeField] private float speed = 3f;

    // référence à l’ennemi
    [SerializeField] private GameObject enemy;

    // rigidbody de l’ennemi
    private Rigidbody enemyRb;

    private void Start(){
        // récupère le rigidbody
        enemyRb = enemy.GetComponent<Rigidbody>();

        // erreur si manquant
        if (enemyRb == null)
            Debug.LogError("L'objet Enemy n'a pas de Rigidbody !");
    }

    private void OnTriggerStay(Collider other){
        // vérifie si c’est le droid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats == null) return;

        // sécurité rigidbody
        if (enemyRb == null) return;

        // direction vers le droid
        Vector3 direction = (stats.transform.position - enemy.transform.position).normalized;

        // déplace l’ennemi
        enemyRb.linearVelocity = direction * speed;
    }

    private void OnTriggerExit(Collider other){
        // vérifie si c’est le droid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats == null) return;

        // sécurité rigidbody
        if (enemyRb == null) return;

        // stop l’ennemi
        enemyRb.linearVelocity = Vector3.zero;
    }
}