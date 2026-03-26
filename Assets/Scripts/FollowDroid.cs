using UnityEngine;

public class FollowDroid : MonoBehaviour{
    // référence au droid à suivre
    [SerializeField] private GameObject droid;

    // décalage de la caméra
    [SerializeField] private Vector3 offset;

    void LateUpdate(){
        // sécurité si le droid n'existe pas
        if (droid == null) return;

        // place l'objet avec un offset par rapport au droid
        transform.position = droid.transform.position + offset;
    }
}
