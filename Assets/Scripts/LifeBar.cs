using UnityEngine;

public class LifeBar : MonoBehaviour{
    private Enemy enemy;          // référence de l’ennemi
    private Vector3 startScale;   // taille de départ de la barre

    private void Start(){
        // récupère l’ennemi dans les parents
        enemy = GetComponentInParent<Enemy>();

        // sécurité si aucun ennemi trouvé
        if (enemy == null){
            Debug.LogError("[LifeBar] Aucun Enemy trouvé !");
            enabled = false;
            return;
        }

        // mémorise la taille initiale
        startScale = transform.localScale;
    }

    private void Update(){
        // calcule le pourcentage de vie
        float ratio = enemy.Life / enemy.MaxLife;
        ratio = Mathf.Clamp01(ratio);

        // ajuste la barre sur l’axe X
        transform.localScale = new Vector3(
            startScale.x * ratio,
            startScale.y,
            startScale.z
        );
    }
}