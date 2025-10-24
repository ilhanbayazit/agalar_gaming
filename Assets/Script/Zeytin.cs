using UnityEngine;

public class Zeytim : MonoBehaviour
{
    [SerializeField] float Damage;
    [SerializeField] float SekmeGucu;
    [SerializeField] float SekmeSuresi;
    [SerializeField] float lifeTime;
    public int EkstraHasar = 0;
    bool DegdiMi = false;
    void Start()
    {
        Destroy(gameObject, lifeTime);
        Damage += EkstraHasar;

    }

    void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Enemy") && !DegdiMi)
        {
            DegdiMi = true;
            other.GetComponent<DusmanSc>().HasarAl(Damage);
            other.GetComponent<DusmanSc>().YoldaSek(SekmeGucu,SekmeSuresi);
            Destroy(gameObject);

        }
    }

}
