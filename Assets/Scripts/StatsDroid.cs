using UnityEngine;

public class StatsDroid : MonoBehaviour{
    // points de vie et dégâts
    public int life = 3;
    public float attack = 1f;

    // nombre de capsules
    public int capsules = 0;

    private void Start(){
        // affiche la vie au début
        GameManager.UpdateLifeText(life);

        // affiche les capsules au début
        GameManager.UpdateCapsulesText(capsules);
    }

    public void TakeDamage(int amount){
        // enlève des points de vie
        life -= amount;

        // met à jour l’UI
        GameManager.UpdateLifeText(life);

        // si plus de vie
        if (life <= 0){
            // déclenche le game over
            GameManager.GameOver();

            // détruit le droid
            Destroy(gameObject);
        }
    }

    public void CollectCapsule(int amount = 1){
        // ajoute des capsules
        capsules += amount;

        // met à jour l’UI
        GameManager.UpdateCapsulesText(capsules);
    }
}