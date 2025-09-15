using UnityEngine;

public class Zeytim : MonoBehaviour
{
    [SerializeField] float Damage;
    [SerializeField] float SekmeGucu;
    [SerializeField] float SekmeSuresi;
    [SerializeField] float lifeTime;      
    bool DegdiMi = false;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider;
        if ( other.CompareTag("Enemy") && !DegdiMi)
        {
            DegdiMi = true;
            other.GetComponent<DusmanSc>().HasarAl(Damage);
            other.GetComponent<DusmanSc>().YoldaSek(SekmeGucu,SekmeSuresi);
            Destroy(gameObject);

        }
    }

}
