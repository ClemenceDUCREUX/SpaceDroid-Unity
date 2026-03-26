using UnityEngine;

public class LinearMovement : MonoBehaviour{
    [Header("Paramètres de mouvement")]
    public Vector3 targetPosition;   // position cible
    public float speed = 1f;         // vitesse de déplacement

    private Vector3 initialPosition; // position de départ
    private Vector3 startPosition;   // début du trajet
    private Vector3 endPosition;     // fin du trajet

    private int totalSteps;          // nombre d’étapes
    private int currentStep;         // étape actuelle
    private Vector3 stepDelta;       // déplacement par étape

    public Vector3 PreviousPosition { get; private set; } // position précédente
    public Vector3 CurrentPosition  { get; private set; } // position actuelle

    private void Start(){
        // sauvegarde la position initiale
        initialPosition = transform.position;

        // définit le premier trajet
        startPosition = initialPosition;
        endPosition = targetPosition;

        // initialise les positions
        PreviousPosition = startPosition;
        CurrentPosition = startPosition;

        // calcule le mouvement
        ComputeMovementData();
    }

    private void ComputeMovementData(){
        // calcule la distance à parcourir
        float distance = Vector3.Distance(startPosition, endPosition);

        // sécurité si distance ou vitesse invalide
        if (distance <= 0f || speed <= 0f){
            totalSteps = 0;
            stepDelta = Vector3.zero;
            return;
        }

        // distance parcourue par FixedUpdate
        float distancePerStep = speed * Time.fixedDeltaTime;

        // nombre d’étapes nécessaires
        totalSteps = Mathf.Max(1, Mathf.RoundToInt(distance / distancePerStep));

        // déplacement par étape
        stepDelta = (endPosition - startPosition) / totalSteps;
        currentStep = 0;

        // remet la plateforme au point de départ
        transform.position = startPosition;
    }

    private void FixedUpdate(){
        // rien à faire si pas de mouvement
        if (totalSteps <= 0) return;

        // sauvegarde l’ancienne position
        PreviousPosition = transform.position;

        // avance la plateforme
        transform.position += stepDelta;
        CurrentPosition = transform.position;

        // incrémente l’étape
        currentStep++;

        // inverse le sens à la fin du trajet
        if (currentStep >= totalSteps){
            Vector3 temp = startPosition;
            startPosition = endPosition;
            endPosition = temp;

            // recalcule le trajet
            ComputeMovementData();
        }
    }
}