using UnityEngine;

public class BulletSc : MonoBehaviour
{
    public float lifeTime = 3f;      // Merminin yok olma süresi
    bool DegdiMi = false;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")&&!DegdiMi)
        {
            DegdiMi = true;
            other.GetComponent<DusmanSc>().HasarAl(50);
            Destroy(gameObject);

        }
    }

}
