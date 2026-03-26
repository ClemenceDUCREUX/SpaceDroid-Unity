using System.Collections;
using UnityEngine;

public class Meteorite : MonoBehaviour{
    // Direction de chute de la météorite
    [Header("Déplacement")]
    [SerializeField] private Vector3 fallDirection = new Vector3(0f, -1f, 0f);

    // Force appliquée au lancement
    [SerializeField] private float forceStrength = 5f;

    // Temps avant destruction automatique
    [Header("Durée de vie de la météorite")]
    [SerializeField] private float lifeTime = 5f;

    // Temps pendant lequel une plateforme est désactivée
    [Header("Plateformes")]
    [SerializeField] private float disableTime = 3f;

    private Rigidbody rb;
    private Coroutine lifeRoutine;

    private void Start(){
        // Récupère le Rigidbody
        rb = GetComponent<Rigidbody>();

        // Applique la force de chute
        if (rb != null){
            rb.AddForce(fallDirection.normalized * forceStrength, ForceMode.VelocityChange);
        }

        // Démarre le timer de destruction automatique
        lifeRoutine = StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer(){
        // Attend lifeTime secondes
        yield return new WaitForSeconds(lifeTime);

        // Détruit la météorite
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision){
        GameObject hitObject = collision.gameObject;

        // Ignore totalement la KillZone
        if (hitObject.CompareTag("KillZone"))
            return;

        // Si on touche le Droid → dégâts instantanés
        StatsDroid stats = hitObject.GetComponentInParent<StatsDroid>();
        if (stats != null){
            stats.TakeDamage(stats.life);
            return;
        }

        // Si on touche une plateforme
        if (hitObject.CompareTag("Platform")){
            // Annule la destruction automatique
            if (lifeRoutine != null)
                StopCoroutine(lifeRoutine);

            // Désactive la plateforme temporairement
            StartCoroutine(DisablePlatformAndDestroy(hitObject));
        }
        else{
            // Sinon (mur, décor, ennemi...) → destruction
            Destroy(hitObject);
        }
    }

    private IEnumerator DisablePlatformAndDestroy(GameObject platform){
        // Désactive la plateforme
        platform.SetActive(false);

        // Attend disableTime secondes
        yield return new WaitForSeconds(disableTime);

        // Réactive la plateforme
        if (platform != null)
            platform.SetActive(true);

        // Détruit la météorite
        Destroy(gameObject);
    }
}