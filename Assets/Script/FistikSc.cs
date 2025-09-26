using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FistikSc : MonoBehaviour
{
    [SerializeField] float Damage;
    Rigidbody rb;

    public float lifeTime;
    [SerializeField] bool DegdiMi = false;
    float mermican;
    void Start()
    {
        Destroy(gameObject, lifeTime);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.CompareTag("FlyingEnemy") || other.CompareTag("Enemy")) )
        {
            if (other.GetComponent<DusmanSc>() != null)
            {
                if (other.GetComponent<DusmanSc>().can > Damage) 
                {
                    other.GetComponent<DusmanSc>().HasarAl(Damage);
                    Destroy(gameObject);
                }
                else
                {
                    mermican = other.GetComponent<DusmanSc>().can;
                    other.GetComponent<DusmanSc>().HasarAl(Damage);
                    Damage -= mermican;
                    if (Damage <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }

        }
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
