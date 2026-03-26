using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Fireball : MonoBehaviour{
    // dégâts de la fireball
    [Header("Dégâts")]
    [SerializeField] private int damage = 1;

    // durée avant destruction
    [Header("Durée de vie")]
    [SerializeField] private float lifetime = 4f;

    // bloque la fireball sur un Z
    [Header("Verrouillage Z")]
    [SerializeField] private bool lockZ = true;
    [SerializeField] private float lockedZ = 0f;

    // tag à ignorer
    [Header("Tags ignorés")]
    [SerializeField] private string bossTag = "Boss";

    private Rigidbody rb;

    private void Awake(){
        // récupère rigidbody
        rb = GetComponent<Rigidbody>();

        // active gravité et collisions fiables
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
#else
        rb.drag = 0f;
        rb.angularDrag = 0f;
#endif

        // met le collider en trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Start(){
        // détruit après lifetime
        Destroy(gameObject, lifetime);

        // force le Z au départ
        ForceZNow();
    }

    public void Setup(int newDamage, float newLifetime, float zPlane){
        // applique les nouveaux réglages
        damage = newDamage;
        lifetime = newLifetime;
        lockedZ = zPlane;

        // relance le timer
        CancelInvoke();
        Destroy(gameObject, lifetime);

        // force le Z
        ForceZNow();
    }

    private void LateUpdate(){
        // force le Z en continu
        if (lockZ) ForceZNow();
    }

    private void ForceZNow(){
        // verrouille la position Z
        Vector3 p = transform.position;
        p.z = lockedZ;
        transform.position = p;

        // verrouille la vitesse Z
#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
        Vector3 v = rb.linearVelocity;
        v.z = 0f;
        rb.linearVelocity = v;
#else
        Vector3 v = rb.velocity;
        v.z = 0f;
        rb.velocity = v;
#endif
    }

    private void OnTriggerEnter(Collider other){
        // ignore le boss
        if (other.CompareTag(bossTag)) return;

        // touche le droid
        StatsDroid stats = other.GetComponentInParent<StatsDroid>();
        if (stats != null){
            stats.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // touche un mur/sol
        if (!other.isTrigger){
            Destroy(gameObject);
        }
    }
}