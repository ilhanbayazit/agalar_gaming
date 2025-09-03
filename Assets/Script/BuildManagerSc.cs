using UnityEngine;

public class BuildManagerSc : MonoBehaviour
{
    [SerializeField] GameObject Cannon;
    public float beklemeSuresi = 3f;
    float zamanlayici = 0f;
    bool tetiklendi = false;

    void OnTriggerStay(Collider other)
    {
        if (tetiklendi) return;

        if (other.CompareTag("Player")) // Player tag'lı objeyi kontrol ediyoruz
        {
            zamanlayici += Time.deltaTime;

            if (zamanlayici >= beklemeSuresi)
            {
                tetiklendi = true;
                SpawnCannon();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zamanlayici = 0f; // Player çıkarsa süre sıfırlanır
        }
    }

    void SpawnCannon()
    {
        Instantiate(Cannon, transform.position, Quaternion.identity);
    }




}
