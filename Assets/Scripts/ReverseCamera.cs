using UnityEngine;

public class ReverseCamera : MonoBehaviour{
    // référence à la caméra
    [SerializeField] private Transform cameraTransform;

    // état de la caméra
    private bool isReversed = false;

    // dernière plateforme touchée
    private GameObject lastPlatform = null;

    private void OnCollisionEnter(Collision collision){
        // objet touché
        GameObject other = collision.gameObject;

        // ignore si ce n’est pas une plateforme inverse
        if (!other.CompareTag("ReversePlatform"))
            return;

        // évite double déclenchement sur la même plateforme
        if (other == lastPlatform)
            return;

        // inverse l’état
        isReversed = !isReversed;

        // rotation inversée
        if (isReversed)
            cameraTransform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else
            cameraTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // mémorise la plateforme
        lastPlatform = other;
    }
}