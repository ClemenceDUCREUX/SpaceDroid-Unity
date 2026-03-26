using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveDroid : MonoBehaviour{
    // déplacement et saut
    [Header("Déplacement / Saut")]
    public float jumpForce = 5f;
    public float speed = 5f;

    // grenade
    [Header("Grenade (touche G)")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float coolDownG = 0.5f;

    // laser
    [Header("Laser (touche H)")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float coolDownH = 0.2f;

    // dernière direction
    [Header("Direction")]
    [HideInInspector] public int lastHorizontalDirection = 1;

    // rebond obstacle
    [Header("Rebond sur obstacle")]
    [SerializeField] private string obstacleTag = "Obstacle";
    [SerializeField] private float obstacleKnockback = 6f;
    [SerializeField] private float obstacleUpBoost = 1.0f;
    [SerializeField] private float obstacleCooldown = 0.15f;

    // timer rebond
    private float lastObstacleHitTime = -999f;

    // actions input
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction attackGAction;
    private InputAction attackHAction;

    // rigidbody
    private Rigidbody rb;

    // flags
    private bool jumpKeyWasPressed = false;
    private bool isGrounded = false;
    private bool isCoolingDownG = false;
    private bool isCoolingDownH = false;

    // plateforme sous le droid
    private LinearMovement platformUnderDroid = null;

    private void Start(){
        // récupère rigidbody
        rb = GetComponent<Rigidbody>();

        // récupère actions
        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        moveAction = InputSystem.actions.FindAction("Player/Move");
        attackGAction = InputSystem.actions.FindAction("Player/AttackG");
        attackHAction = InputSystem.actions.FindAction("Player/AttackH");
    }

    private void Update(){
        // saute si au sol
        if (jumpAction != null && jumpAction.triggered && isGrounded)
            jumpKeyWasPressed = true;

        // lit le mouvement
        Vector2 move = Vector2.zero;
        if (moveAction != null)
            move = moveAction.ReadValue<Vector2>();

        // garde la dernière direction
        if (move.x > 0.01f) lastHorizontalDirection = 1;
        else if (move.x < -0.01f) lastHorizontalDirection = -1;

        // déplace horizontalement
        transform.Translate(Vector3.right * move.x * speed * Time.deltaTime);

        // tir grenade
        if (attackGAction != null && attackGAction.triggered)
            StartCoroutine(FireG());

        // tir laser
        if (attackHAction != null && attackHAction.triggered)
            StartCoroutine(FireH());
    }

    private void FixedUpdate(){
        // applique le saut
        if (jumpKeyWasPressed)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        // reset saut
        jumpKeyWasPressed = false;

        // suit la plateforme
        if (platformUnderDroid != null){
            Vector3 delta = platformUnderDroid.CurrentPosition - platformUnderDroid.PreviousPosition;
            transform.position += delta;
        }
    }

    private void OnCollisionEnter(Collision collision){
        // ignore les grenades
        if (collision.gameObject.GetComponent<WeaponWithPhysics>() != null)
            return;

        // rebond sur obstacle
        if (collision.gameObject.CompareTag(obstacleTag)){
            TryKnockbackFromObstacle(collision);
            return;
        }

        // passe grounded à vrai
        isGrounded = true;

        // détecte plateforme mobile
        LinearMovement lm = collision.gameObject.GetComponent<LinearMovement>();
        if (lm != null)
            platformUnderDroid = lm;
    }

    private void OnCollisionExit(Collision collision){
        // ignore les grenades
        if (collision.gameObject.GetComponent<WeaponWithPhysics>() != null)
            return;

        // ignore les obstacles
        if (collision.gameObject.CompareTag(obstacleTag))
            return;

        // enlève grounded
        isGrounded = false;

        // enlève plateforme
        LinearMovement lm = collision.gameObject.GetComponent<LinearMovement>();
        if (lm == platformUnderDroid)
            platformUnderDroid = null;
    }

    private void TryKnockbackFromObstacle(Collision collision){
        // cooldown rebond
        if (Time.time - lastObstacleHitTime < obstacleCooldown)
            return;

        // enregistre le temps
        lastObstacleHitTime = Time.time;

        // direction via normale
        Vector3 dir = Vector3.zero;
        if (collision.contactCount > 0)
            dir = collision.contacts[0].normal;
        else
            dir = new Vector3(-lastHorizontalDirection, 0f, 0f);

        // ajoute boost vertical
        dir.y = Mathf.Max(dir.y, 0f) + obstacleUpBoost;
        dir = dir.normalized;

        // applique la vitesse
#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
        rb.linearVelocity = new Vector3(dir.x * obstacleKnockback, dir.y * obstacleKnockback, rb.linearVelocity.z);
#else
        rb.velocity = new Vector3(dir.x * obstacleKnockback, dir.y * obstacleKnockback, rb.velocity.z);
#endif
    }

    private IEnumerator FireG(){
        // bloque si cooldown
        if (isCoolingDownG || grenadePrefab == null)
            yield break;

        // spawn grenade
        Instantiate(grenadePrefab, transform.position, grenadePrefab.transform.rotation);

        // active cooldown
        isCoolingDownG = true;
        yield return new WaitForSeconds(coolDownG);
        isCoolingDownG = false;
    }

    private IEnumerator FireH(){
        // bloque si cooldown
        if (isCoolingDownH)
            yield break;

        // bloque si prefab manquant
        if (laserPrefab == null)
            yield break;

        // position du spawn
        Vector3 spawnPos = transform.position + Vector3.up * 0.1f;

        // spawn laser
        Instantiate(laserPrefab, spawnPos, laserPrefab.transform.rotation);

        // active cooldown
        isCoolingDownH = true;
        yield return new WaitForSeconds(coolDownH);
        isCoolingDownH = false;
    }
}