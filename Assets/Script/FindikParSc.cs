using UnityEngine;

public class FindikParSc : MonoBehaviour
{
    [SerializeField] float Damage;
    public float lifeTime;     
    [SerializeField] bool DegdiMi = false;
    public int EkstraHasar;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        Damage += EkstraHasar;
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
