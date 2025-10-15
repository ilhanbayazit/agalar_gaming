using System;
using System.Collections;
using UnityEngine;

public class KurdanYagmuru : MonoBehaviour
{
    [Header("Prefab(lar)")]
    [SerializeField] GameObject[] prefabs;     // Bir veya birden fazla prefab

    [Header("Adet & Konum")]
    [SerializeField] int adet = 10;            // Kaç tane doğsun
    [SerializeField] Vector2 alanXZ = new(3, 3);
    [SerializeField] float yukseklik = 10f;

    [Header("Zamanlama")]
    [SerializeField] float toplamSure = 2f;    // Tüm doğuşlar bu süre içinde, rastgele anlarda

    public void Baslat(Vector3 merkezNoktasi) => StartCoroutine(SpawnRoutine(merkezNoktasi));

    IEnumerator SpawnRoutine(Vector3 merkezNoktasi)
    {
        Debug.Log("sa");
        if (prefabs == null || prefabs.Length == 0) yield break;

        // 0..toplamSure aralığında rastgele zaman noktaları üret ve sırala
        float[] zamanlar = new float[Mathf.Max(1, adet)];
        for (int i = 0; i < zamanlar.Length; i++)
            zamanlar[i] = UnityEngine.Random.Range(0f, Mathf.Max(0.0001f, toplamSure));
        Array.Sort(zamanlar);

        float last = 0f;
        for (int i = 0; i < zamanlar.Length; i++)
        {
            float wait = Mathf.Max(0f, zamanlar[i] - last);
            last = zamanlar[i];
            if (wait > 0f) yield return new WaitForSeconds(wait);
            SpawnOne(merkezNoktasi);
        }
    }

    void SpawnOne(Vector3 merkezNoktasi)
    {
        var prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];

        float x = UnityEngine.Random.Range(-alanXZ.x * 0.5f, alanXZ.x * 0.5f);
        float z = UnityEngine.Random.Range(-alanXZ.y * 0.5f, alanXZ.y * 0.5f);
        Vector3 pos = new Vector3(merkezNoktasi.x + x, merkezNoktasi.y + yukseklik, merkezNoktasi.z + z);

        var e = prefab.transform.eulerAngles;
        e.x += 180f;

        var go = Instantiate(prefab, pos, Quaternion.Euler(e));

        if (!go.TryGetComponent<Rigidbody>(out var rb))
            rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;
    }
}
