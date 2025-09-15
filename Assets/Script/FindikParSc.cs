using UnityEngine;

public class FindikParSc : MonoBehaviour
{
    [SerializeField] float Damage;
    public float lifeTime;      // Merminin yok olma süresi
    [SerializeField] bool DegdiMi = false;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.CompareTag("FlyingEnemy") || other.CompareTag("Enemy")) && !DegdiMi)
        {
            DegdiMi = true;
            other.GetComponent<DusmanSc>().HasarAl(Damage);
            Destroy(gameObject);

        }
    }
}
