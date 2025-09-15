using UnityEngine;

public class FindikSc : MonoBehaviour
{
    [SerializeField] float Damage;

    public float lifeTime;      // Merminin yok olma süresi
    bool DegdiMi = false;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider;

        if (!DegdiMi)
        {
            DegdiMi = true;
            if (other.GetComponent<DusmanSc>()!=null)
            {
                other.GetComponent<DusmanSc>().HasarAl(Damage);
            }
            Parcalan();
        }
        else
        {
            return;
        }
    }


    [Header("Parça Ayarları")]
    [SerializeField] GameObject[] ParcaPrefabs;   // 5 farklı prefab (Inspector’dan ver)
    [SerializeField] float YayilimYaricapi = 0.25f;
    [SerializeField] Vector2 ItkiAraligi = new Vector2(3f, 6f);
    [SerializeField] float YukariBias = 0.3f;
    public void Parcalan()
    {
        Vector3 pos = transform.position;

        int adet = ParcaPrefabs != null ? ParcaPrefabs.Length : 0;
        for (int i = 0; i < adet; i++)
        {
            var prefab = ParcaPrefabs[i];
            if (prefab == null) continue;
            Vector3 spawnPos = pos + Random.insideUnitSphere * YayilimYaricapi;
            Debug.Log("olustu");
            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

            if (go.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 dir = Random.onUnitSphere;
                dir.y += YukariBias;
                dir.Normalize();

                float itki = Random.Range(ItkiAraligi.x, ItkiAraligi.y);
                dir.Normalize();

                rb.AddForce(dir * itki * rb.mass, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
