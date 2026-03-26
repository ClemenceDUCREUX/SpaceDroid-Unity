using UnityEngine;

public class RotateDroid : MonoBehaviour{
    // vitesse de rotation
    [SerializeField] private float rotationSpeed;

    void Update(){
        // fait tourner le droid en continu
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}