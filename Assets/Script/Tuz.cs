using System;
using Unity.VisualScripting;
using UnityEngine;

public class Tuz : MonoBehaviour
{
    [SerializeField] float Damage;

    public float lifeTime;
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
            DegdiMi = true;
            var d = other.GetComponent<DusmanSc>();
            if (d) d.HasarAl(Damage);
            Destroy(gameObject);
        }
    }

}


