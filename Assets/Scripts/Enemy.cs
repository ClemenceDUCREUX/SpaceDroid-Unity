using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour{
    // réglages contact droid
    [Header("Contact avec le Droid")]
    [SerializeField] private float knockbackSpeed = 8f;
    [SerializeField] private int touchAttack = 1;
    [SerializeField] private float damageCooldown = 0.5f;

    // réglages vie ennemi
    [Header("Vie de l'ennemi")]
    [SerializeField] private float maxLife = 10f;
    public float Life { get; private set; }
    public float MaxLife => maxLife;

    // action boss à la mort
    [Header("Boss - Actions à la mort")]
    [SerializeField] private bool removeObstaclesOnDeathIfBoss = true;
    [SerializeField] private string obstacleTag = "Obstacle";
    [SerializeField] private bool destroyObstacles = false;

    // attaque boss fireballs
    [Header("Boss - Attaque fireballs")]
    [SerializeField] private bool enableBossFireballs = true;
    [SerializeField] private GameObject fireballPrefab;

    // portée d'attaque boss
    [Header("Boss - Portée d'attaque")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private bool drawAttackRangeGizmo = true;

    // point de spawn des fireballs
    [Tooltip("Point de tir (Empty) au-dessus/devant le boss. Recommandé.")]
    [SerializeField] private Transform firePoint;

    // réglages fireball
    [Header("Fireball - Réglages")]
    [SerializeField] private float fireballInterval = 1.5f;
    [SerializeField] private float fireballFlightTime = 1.0f;
    [SerializeField] private float fireballLifeTime = 4f;
    [SerializeField] private int fireballDamage = 1;
    [SerializeField] private float fireballStartDelay = 0.5f;

    // plan Z du jeu
    [Header("Plan Z du jeu")]
    [SerializeField] private float zPlane = 0f;

    // sécurité spawn fireball
    [Header("Spawn sécurité")]
    [SerializeField] private float spawnUpOffset = 0.0f;
    [SerializeField] private float ignoreBossCollisionTime = 0.2f;

    private Rigidbody enemyRb;
    private float lastTouchTime = -999f;
    private Coroutine fireballRoutine;

    private void Awake(){
        // récupère rigidbody et vie
        enemyRb = GetComponent<Rigidbody>();
        Life = maxLife;
    }

    private void Start(){
        // lance l'attaque si boss
        if (CompareTag("Boss") && enableBossFireballs && fireballPrefab != null){
            fireballRoutine = StartCoroutine(BossFireballLoop());
        }
    }

    private IEnumerator BossFireballLoop(){
        // délai avant premier tir
        yield return new WaitForSeconds(fireballStartDelay);

        // boucle tant que vivant
        while (Life > 0f){
            // cherche le droid
            GameObject droidObj = GameObject.FindGameObjectWithTag("Droid");
            if (droidObj != null){
                // positions sur le même Z
                Vector3 bossPos = transform.position; bossPos.z = zPlane;
                Vector3 droidPos = droidObj.transform.position; droidPos.z = zPlane;

                // calcule distance
                float distance = Vector3.Distance(bossPos, droidPos);

                // stop si trop loin
                if (distance > attackRange){
                    yield return new WaitForSeconds(fireballInterval);
                    continue;
                }

                // calcule position spawn
                Vector3 spawnPos = (firePoint != null ? firePoint.position : transform.position + Vector3.up * 1.0f);
                spawnPos += Vector3.up * spawnUpOffset;
                spawnPos.z = zPlane;

                // calcule cible
                Vector3 targetPos = droidPos + Vector3.up * 0.5f;
                targetPos.z = zPlane;

                // calcule trajectoire balistique
                Vector3 delta = targetPos - spawnPos;
                delta.z = 0f;

                float t = Mathf.Max(0.1f, fireballFlightTime);
                float g = Mathf.Abs(Physics.gravity.y);

                Vector3 launch = new Vector3(
                    delta.x / t,
                    (delta.y / t) + (0.5f * g * t),
                    0f
                );

                // spawn fireball
                GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

                // ignore collision boss au départ
                Collider bossCol = GetComponent<Collider>();
                Collider fbCol = fb.GetComponent<Collider>();
                if (bossCol != null && fbCol != null){
                    Physics.IgnoreCollision(bossCol, fbCol, true);
                    StartCoroutine(ReenableCollisionAfterDelay(bossCol, fbCol, ignoreBossCollisionTime));
                }

                // configure fireball si script présent
                Fireball fireball = fb.GetComponent<Fireball>();
                if (fireball != null){
                    fireball.Setup(fireballDamage, fireballLifeTime, zPlane);
                }
                else{
                    Destroy(fb, fireballLifeTime);
                }

                // applique vitesse une fois
                Rigidbody fbRb = fb.GetComponent<Rigidbody>();
                if (fbRb != null){
                    fbRb.useGravity = true;

#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
                    fbRb.linearVelocity = launch;
#else
                    fbRb.velocity = launch;
#endif
                }
            }
            // attente entre tirs
            yield return new WaitForSeconds(fireballInterval);
        }
    }

    private IEnumerator ReenableCollisionAfterDelay(Collider a, Collider b, float delay){
        // réactive collision après délai
        yield return new WaitForSeconds(delay);
        if (a != null && b != null)
            Physics.IgnoreCollision(a, b, false);
    }

    public void TakeDamage(float amount){
        // enlève de la vie
        Life -= amount;
        if (Life < 0f) Life = 0f;

        // mort
        if (Life <= 0f){
            // stop attaque boss
            if (fireballRoutine != null)
                StopCoroutine(fireballRoutine);

            // enlève obstacles si boss
            if (removeObstaclesOnDeathIfBoss && CompareTag("Boss"))
                RemoveObstacles();

            // ajoute un kill
            GameManager.AddKill();

            // détruit l'ennemi
            Destroy(gameObject);
        }
    }

    private void RemoveObstacles(){
        // cherche obstacles par tag
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);
        for (int i = 0; i < obstacles.Length; i++){
            if (obstacles[i] == null) continue;

            // détruit ou désactive
            if (destroyObstacles) Destroy(obstacles[i]);
            else obstacles[i].SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision){
        // détecte grenade
        WeaponWithPhysics weapon = collision.collider.GetComponentInParent<WeaponWithPhysics>();
        if (weapon != null){
            // récupère attaque du droid
            GameObject droidObj = GameObject.FindGameObjectWithTag("Droid");
            if (droidObj != null){
                StatsDroid droidStats = droidObj.GetComponent<StatsDroid>();
                if (droidStats != null){
                    float damage = droidStats.attack * weapon.weaponMultiplicator;
                    TakeDamage(damage);
                }
            }
            return;
        }

        // détecte contact droid
        StatsDroid stats = collision.collider.GetComponentInParent<StatsDroid>();
        if (stats == null) return;

        // cooldown dégâts
        if (Time.time - lastTouchTime < damageCooldown) return;
        lastTouchTime = Time.time;

        // applique knockback
        Rigidbody droidRb = stats.GetComponent<Rigidbody>();
        if (droidRb != null && enemyRb != null)
        {
            Vector3 dir = (droidRb.transform.position - transform.position);
            dir.y = Mathf.Abs(dir.y) + 0.5f;
            dir = dir.normalized;

#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
            droidRb.linearVelocity = dir * knockbackSpeed;
            enemyRb.linearVelocity = -dir * (knockbackSpeed * 0.5f);
#else
            droidRb.velocity = dir * knockbackSpeed;
            enemyRb.velocity = -dir * (knockbackSpeed * 0.5f);
#endif
        }

        // inflige dégâts au droid
        stats.TakeDamage(touchAttack);
    }

    private void OnDrawGizmosSelected(){
        // dessine la portée
        if (!drawAttackRangeGizmo) return;
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position;
        center.z = zPlane;

        Gizmos.DrawWireSphere(center, attackRange);
    }
}