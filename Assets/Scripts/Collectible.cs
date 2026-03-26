using UnityEngine;

public class Collectible : MonoBehaviour{
    // valeur donnée au joueur
    [SerializeField] private int value = 1;

    // accès à la valeur
    public int Value => value;
}