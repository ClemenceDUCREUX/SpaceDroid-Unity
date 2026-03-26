using UnityEngine;

public class RotatePlanet : MonoBehaviour{
    // vitesse de rotation
    public float speed = 20f;

    void Update(){
        // récupère la rotation actuelle
        Vector3 rot = transform.eulerAngles;

        // bloque la rotation sur X et Z
        rot.x = 0f;
        rot.z = 0f;

        // fait tourner autour de Y
        rot.y += speed * Time.deltaTime;

        // applique la rotation
        transform.eulerAngles = rot;
    }
}