using Unity.VisualScripting;
using UnityEngine;

public class KurdanYagmuru : MonoBehaviour
{
    [Header("Prefab(lar)")]
    [SerializeField] GameObject[] prefabs;     // İstersen tek prefab koy, istersen bir dizi

    [Header("Adet & Konum")]
    [SerializeField] int adet = 10;            // Kaç tane doğsun (bir kere)
    [SerializeField] Vector2 alanXZ = new(3, 3);
    [SerializeField] float yukseklik;

    
   public void Baslat(Vector3 merkezNoktasi)
    {
        for (int i = 0; i < adet; i++)
        {
            float x = Random.Range(-alanXZ.x * 0.5f, alanXZ.x * 0.5f);
            float z = Random.Range(-alanXZ.y * 0.5f, alanXZ.y * 0.5f);
            Vector3 pos = new Vector3(merkezNoktasi.x + x, merkezNoktasi.y + yukseklik, merkezNoktasi.z + z);

            if (prefabs == null || prefabs.Length == 0) return;
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            prefab.GetComponent<Rigidbody>().useGravity = true;
            var e = prefab.transform.eulerAngles;
            e.x += 180f;
            var go = Instantiate(prefab, pos, Quaternion.Euler(e));

        }
    }


}
