using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour{
    // empêche plusieurs fins
    private bool hasFinished = false;

    // appelée à la fin de l’animation
    public void OnCreditsFinished(){
        if (hasFinished) return; // évite double appel
        hasFinished = true;      // marque comme terminé
        SceneManager.LoadScene(0); // retour menu
    }

    // bouton pour passer les crédits
    public void SkipCredits(){
        if (hasFinished) return; // évite double appel
        hasFinished = true;      // marque comme terminé
        SceneManager.LoadScene(0); // retour menu
    }
}
