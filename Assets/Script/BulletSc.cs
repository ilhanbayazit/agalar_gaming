using UnityEngine;

public class BulletSc : MonoBehaviour
{
    [SerializeField] float Damage;

    public float lifeTime ;      // Merminin yok olma süresi
   [SerializeField] bool DegdiMi = false;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.CompareTag("FlyingEnemy") || other.CompareTag("Enemy"))&&!DegdiMi)
        {
            DegdiMi = true;
            if (other.GetComponent<DusmanSc>() != null)
            {
                other.GetComponent<DusmanSc>().HasarAl(Damage);
            }
            Destroy(gameObject);
       
        }
    }

}
