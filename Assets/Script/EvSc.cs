using System.Collections.Generic;
using UnityEngine;

public class EvSc : MonoBehaviour
{
    [SerializeField] GameObject karakterPrefab; // Oluşturulacak karakter prefabı
    [SerializeField] Transform spawnPoint;      // Karakterin oluşacağı nokta

    float spawnSuresi = 2f;
 
    private void Start()
    {
        InvokeRepeating(nameof(KarakterOlustur), 0f, spawnSuresi);
    }

    void KarakterOlustur()
    {
        Instantiate(karakterPrefab, spawnPoint.position, spawnPoint.rotation);
    }

   

}
