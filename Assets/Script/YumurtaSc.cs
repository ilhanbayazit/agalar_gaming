using System.Collections;
using UnityEngine;

public class YumurtaSc : MonoBehaviour
{
    [SerializeField] float CatlamaSuresi;

    private void Start()
    {
        StartCoroutine(Catla(CatlamaSuresi));
  
    }
    IEnumerator Catla(float x)
    {
        yield return new WaitForSeconds(x);

        int adet = Random.Range(2, 5); // [1,4]
        for (int i = 0; i < adet; i++)
        {
            Vector2 j = Random.insideUnitCircle * 0.4f; // hafif saçılma
            Vector3 pos = transform.position + new Vector3(j.x, 0f, j.y);
            GameManagerSc.Instance.KarincaSpawnFromPos(pos, 0);
            yield return null; // istersen: yeni frame'de spawnla
        }
        Destroy(gameObject);
    }




}
