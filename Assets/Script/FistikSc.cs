using System.Collections.Generic;
using UnityEngine;

public class FistikSc : MonoBehaviour
{
    [SerializeField] float Damage;
    Rigidbody rb;

    private HashSet<DusmanSc> vurulan;
    public float lifeTime;
    void Start()
    {
        Destroy(gameObject, lifeTime);
        rb = GetComponent<Rigidbody>();
        vurulan = new HashSet<DusmanSc>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!( other.CompareTag("Enemy"))) return;

        var d = other.GetComponent<DusmanSc>();
        if (d == null) return;
        if (vurulan.Contains(d)) return;   // aynı düşmanı tekrar vurma
        vurulan.Add(d);
        float vurulacak = Mathf.Min(Damage, d.can);
        d.HasarAl(vurulacak);
        Damage -= vurulacak;

        if (Damage <= 0f) Destroy(gameObject); // hasar bitti -> mermi yok olsun
    }


    [Range(0f, 1f)] public float yerTutmaOrani = 0.2f; // zıplamanın %20'si kalsın
    void OnCollisionEnter(Collision col)
    {
        if (col.contactCount == 0) return;

        var c = col.GetContact(0);
        var n = c.normal;

        // Yere benzer temas (yukarı bakan normal)
        if (n.y > 0.5f)
        {
            // Temas noktasındaki hız bileşeni (velocity kullanmadan)
            Vector3 vPoint = rb.GetPointVelocity(c.point);
            float vNormal = Vector3.Dot(vPoint, n); // normal doğrultusundaki hız

            // Yüzeyden kopma yönünde pozitif ise (yukarı sekiyor)
            if (vNormal > 0f)
            {
                float azalt = vNormal * (1f - yerTutmaOrani); // ne kadar kesilecek
                Vector3 karsiDarbe = -n * azalt * rb.mass;    // impuls
                rb.AddForce(karsiDarbe, ForceMode.Impulse);
            }
        }
    }

}
