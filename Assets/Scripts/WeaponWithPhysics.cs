using System.Collections;
using UnityEngine;

public class WeaponWithPhysics : MonoBehaviour{
    // multiplicateur de dégâts
    public float weaponMultiplicator = 2f;

    // impulsion de départ
    [SerializeField] private Vector3 initialImpulse = new Vector3(2f, 3f, 0f);

    // durée de vie
    [SerializeField] private float lifeTime = 5f;

    // rigidbody
    private Rigidbody rb;

    // collider
    private Collider col;

    // vérifie si la grenade a quitté le droid
    private bool hasLeftDroid = false;

    private void Awake(){
        // récupère rigidbody
        rb = GetComponent<Rigidbody>();

        // récupère collider
        col = GetComponent<Collider>();
    }

    private void Start(){
        // récupère la direction du droid
        GameObject droid = GameObject.FindGameObjectWithTag("Droid");
        if (droid != null){
            MoveDroid moveDroid = droid.GetComponent<MoveDroid>();
            if (moveDroid != null){
                int dir = moveDroid.lastHorizontalDirection;
                initialImpulse.x = Mathf.Abs(initialImpulse.x) * dir;
            }
        }

        // force un tir vers le haut
        if (initialImpulse.y <= 0f)
            initialImpulse.y = Mathf.Abs(initialImpulse.y) + 1f;

        // applique l’impulsion
        if (rb != null)
            rb.AddForce(initialImpulse, ForceMode.VelocityChange);

        // lance le timer
        StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer(){
        yield return new WaitForSeconds(lifeTime); // attend
        Destroy(gameObject); // détruit la grenade
    }

    private void OnTriggerExit(Collider other){
        // évite plusieurs fois
        if (hasLeftDroid) return;

        // détecte la sortie du droid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats != null && col != null){
            col.isTrigger = false; // repasse en collision normale
            hasLeftDroid = true;   // confirme la sortie
        }
    }

    private void OnCollisionEnter(Collision collision){
        // ignore le droid
        StatsDroid stats = collision.collider.GetComponentInParent<StatsDroid>();
        if (stats != null){
            Physics.IgnoreCollision(col, collision.collider, true);
            return;
        }

        // détruit sur collision
        Destroy(gameObject);
    }
}