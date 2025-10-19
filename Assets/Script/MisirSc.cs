using UnityEngine;

public class MisirSc : MonoBehaviour
{
    [SerializeField] float Damage;
    [SerializeField] ParticleSystem fxPrefab;

    public float lifeTime;      // Merminin yok olma süresi
    [SerializeField] bool DegdiMi = false;
    public int EkstraHasar = 0;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        Damage += EkstraHasar;
    }
    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("FlyingEnemy") && !DegdiMi))
        {
            DegdiMi = true;
            Vector3 hitPos = other.ClosestPoint(transform.position);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("YER")) Destroy(gameObject, 0.2f);
    }
}
