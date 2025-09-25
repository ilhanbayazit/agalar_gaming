using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FistikSc : MonoBehaviour
{
    [SerializeField] float Damage;

    public float lifeTime;
    [SerializeField] bool DegdiMi = false;
    float mermican;
    void Start()
    {
        Destroy(gameObject, lifeTime);
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
}
