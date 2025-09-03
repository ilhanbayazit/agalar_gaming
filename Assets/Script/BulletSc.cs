using UnityEngine;

public class BulletSc : MonoBehaviour
{
    public float bulletSpeed = 20f;   // Merminin hızı
    public float lifeTime = 3f;      // Merminin yok olma süresi


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

        }
    }

}
