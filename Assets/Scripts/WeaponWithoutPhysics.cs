using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponWithoutPhysics : MonoBehaviour{
    [Header("Mouvement")]
    [SerializeField] private float speed = 10f;      // vitesse du laser
    [SerializeField] private float lifeTime = 2f;    // durée de vie

    [Header("Dégâts")]
    [SerializeField] private float weaponMultiplicator = 1f; // multiplicateur de dégâts

    private int direction = 1;   // sens du tir
    private Collider col;

    private void Awake(){
        col = GetComponent<Collider>(); // récupère le collider
        col.isTrigger = true;           // collision en trigger
    }

    private void Start(){
        // récupère la direction du droid
        GameObject droid = GameObject.FindGameObjectWithTag("Droid");
        if (droid != null){
            MoveDroid moveDroid = droid.GetComponent<MoveDroid>();
            if (moveDroid != null)
                direction = moveDroid.lastHorizontalDirection;
        }

        StartCoroutine(LifeTimer()); // lance le timer de destruction
    }

    private void Update(){
        // déplacement constant
        transform.position += Vector3.right * direction * speed * Time.deltaTime;
    }

    private IEnumerator LifeTimer(){
        yield return new WaitForSeconds(lifeTime); // attend
        Destroy(gameObject);                       // détruit le laser
    }

    private void OnTriggerEnter(Collider other){
        // ignore le droid
        if (other.GetComponentInParent<StatsDroid>() != null){
            Physics.IgnoreCollision(col, other, true);
            return;
        }

        // ignore les zones de détection d’ennemi
        if (other.CompareTag("EnemyDetection") || other.GetComponent<MoveEnemy>() != null)
            return;

        // touche un ennemi
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null){
            float damage = weaponMultiplicator;

            GameObject droid = GameObject.FindGameObjectWithTag("Droid");
            if (droid != null){
                StatsDroid stats = droid.GetComponent<StatsDroid>();
                if (stats != null)
                    damage = stats.attack * weaponMultiplicator;
            }

            enemy.TakeDamage(damage); // inflige les dégâts
            Destroy(gameObject);      // détruit le laser
            return;
        }

        // touche autre chose
        Destroy(gameObject);
    }
}