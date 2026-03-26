using System.Collections;
using UnityEngine;

public class MeteorSpawnManager : MonoBehaviour{
    [Header("Prefab de météorite")]
    [SerializeField] private GameObject meteorPrefab; // prefab de base

    [Header("Zone de spawn")]
    [SerializeField] private float rangeX = 10f; // largeur de la zone

    [Header("Temps entre deux météorites")]
    [SerializeField] private float minTime = 1f; // délai minimum
    [SerializeField] private float maxTime = 3f; // délai maximum

    [Header("Tailles des météorites")]
    [SerializeField] private float smallScale = 0.7f;  // petite taille
    [SerializeField] private float mediumScale = 1.0f; // taille normale
    [SerializeField] private float largeScale = 1.3f;  // grande taille

    [Header("Probabilités (%)")]
    [SerializeField] private int smallChance = 50;  // chance petite
    [SerializeField] private int mediumChance = 35; // chance moyenne
    // le reste = grande

    private void Start(){
        // vérifie le prefab
        if (meteorPrefab == null){
            Debug.LogError("[MeteorSpawnManager] Aucun prefab assigné");
            return;
        }

        // lance la pluie
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop(){
        while (true){
            // attente aléatoire
            float delay = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(delay);

            // position de base
            Vector3 basePos = transform.position;

            // X aléatoire
            float halfRange = rangeX * 0.5f;
            float randomX = Random.Range(basePos.x - halfRange, basePos.x + halfRange);

            // position finale
            Vector3 spawnPos = new Vector3(randomX, basePos.y, basePos.z);

            // création de la météorite
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, meteorPrefab.transform.rotation);

            // applique une taille aléatoire
            ApplyRandomSize(meteor);
        }
    }

    private void ApplyRandomSize(GameObject meteor){
        // tirage aléatoire
        int roll = Random.Range(0, 100);

        float scale;

        // petite taille
        if (roll < smallChance)
            scale = smallScale;
        // taille moyenne
        else if (roll < smallChance + mediumChance)
            scale = mediumScale;
        // grande taille
        else
            scale = largeScale;

        // applique la taille
        meteor.transform.localScale = Vector3.one * scale;

        // ajuste la masse
        Rigidbody rb = meteor.GetComponent<Rigidbody>();
        if (rb != null)
            rb.mass *= scale;
    }
}