using UnityEngine;
public class BulletSc : MonoBehaviour
{
    [SerializeField] float Damage;
    [SerializeField] ParticleSystem fxPrefab;

    public float lifeTime;      // Merminin yok olma süresi
    [SerializeField] bool DegdiMi = false;
    public int EkstraHasar=0;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        Damage += EkstraHasar;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("FlyingEnemy") || other.CompareTag("Enemy")) && !DegdiMi)
        {
            DegdiMi = true;

            // HIT NOKTASI
            Vector3 hitPos = other.ClosestPoint(transform.position);
            // FX'i sahneye bas ve süre bitince yok et
            if (fxPrefab != null)
            {
                var fx = Instantiate(fxPrefab, hitPos, Quaternion.identity);
                fx.Play();
                var m = fx.main;
                Destroy(fx.gameObject, m.duration + m.startLifetime.constantMax);
            }

            var d = other.GetComponent<DusmanSc>();
            if (d) d.HasarAl(Damage);
            Destroy(gameObject);
        }
    }

}


