using UnityEngine;

public class CollectDroid : MonoBehaviour{
    // compteur pour le bonus
    private int capsulesCollectedForBonus = 0;

    private MoveDroid moveDroid;
    private StatsDroid statsDroid;

    // valeurs de base
    private float baseSpeed;
    private float baseJumpForce;
    private float bonusTimer = 0f;

    private void Start(){
        // récupère les scripts
        moveDroid = GetComponent<MoveDroid>();
        statsDroid = GetComponent<StatsDroid>();

        // sécurité si MoveDroid manque
        if (moveDroid == null){
            Debug.LogError("[CollectDroid] MoveDroid introuvable !");
            enabled = false;
            return;
        }

        // sauvegarde des stats de base
        baseSpeed = moveDroid.speed;
        baseJumpForce = moveDroid.jumpForce;
    }

    private void Update(){
        // gère la durée du bonus
        if (bonusTimer > 0f){
            bonusTimer -= Time.deltaTime;

            // fin du bonus
            if (bonusTimer <= 0f){
                moveDroid.speed = baseSpeed;
                moveDroid.jumpForce = baseJumpForce;
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        // ignore si pas un collectible
        if (!other.CompareTag("Collectible"))
            return;

        // récupère la valeur du collectible
        Collectible collectible = other.GetComponent<Collectible>();
        int value = (collectible != null) ? collectible.Value : 1;

        // détruit l'objet ramassé
        Destroy(other.gameObject);

        // ajoute les capsules au droid
        if (statsDroid != null){
            statsDroid.CollectCapsule(value);
        }

        // ajoute au compteur bonus
        capsulesCollectedForBonus += value;

        // active le bonus
        if (capsulesCollectedForBonus >= 4){
            moveDroid.speed = baseSpeed * 2f;
            moveDroid.jumpForce = baseJumpForce * 2f;
            bonusTimer = 10f;
            capsulesCollectedForBonus = 0;
        }
    }
}