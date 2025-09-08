using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EvSc : MonoBehaviour
{
    [SerializeField] GameObject karakterPrefab; // Oluşturulacak karakter prefabı
    Vector3 spawnPoint;      // Karakterin oluşacağı nokta

    float spawnSuresi = 2f;
    bool buldu = false;
    private void Start()
    {
        FindSpawnLocation();
        if (buldu)
        {
            InvokeRepeating(nameof(KarakterOlustur), 0f, spawnSuresi);
        }
    }

    void KarakterOlustur()
    {
        Instantiate(karakterPrefab, spawnPoint, Quaternion.identity);
    }

   void FindSpawnLocation()
   {
        
        do
        {
            Vector3 yeniPozisyon = RastgelePozisyon(transform.position, 3f);
            if (RoadUzerindeMi(yeniPozisyon))
            {
                spawnPoint = yeniPozisyon;
                buldu = true;
                Debug.Log("buldu");
            }
            else
            {
                yeniPozisyon = RastgelePozisyon(transform.position, 3f);
                Debug.Log(yeniPozisyon);
            }
        } while (!buldu);
     

    }
    Vector3 RastgelePozisyon(Vector3 merkez, float yaricap = 3f)
    {
        float randomX = Random.Range(-yaricap, yaricap);
        float randomZ = Random.Range(-yaricap, yaricap);

        // Y değeri sabit kalıyor, sadece X ve Z değişiyor
        return new Vector3(merkez.x + randomX, merkez.y, merkez.z + randomZ);
    }

    bool RoadUzerindeMi(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Road"))
                return true;
        }
        return false;
    }

}
