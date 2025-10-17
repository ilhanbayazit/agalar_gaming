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
        if (other.CompareTag("Bok"))
        {
            Destroy(gameObject);
            return;
        }
        if ((other.CompareTag("Enemy")) && !DegdiMi)
        {
            other.GetComponent<DusmanSc>().HasarAl(Damage);
            Destroy(gameObject);
            DegdiMi = true;
        }
    }
}
