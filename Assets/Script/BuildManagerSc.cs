using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManagerSc : MonoBehaviour
{
    [SerializeField] GameObject Cannon;
    [SerializeField] GameObject Ev;
    [SerializeField] GameObject canvas;



    //public float beklemeSuresi = 3f;
    //float zamanlayici = 0f;
    //bool tetiklendi = false;

    //void OnTriggerStay(Collider other)
    //{
    //    if (tetiklendi) return;

    //    if (other.CompareTag("Player")) // Player tag'lı objeyi kontrol ediyoruz
    //    {
    //        zamanlayici += Time.deltaTime;

    //        if (zamanlayici >= beklemeSuresi)
    //        {
    //            tetiklendi = true;
    //            SpawnCannon();
    //        }
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        zamanlayici = 0f; // Player çıkarsa süre sıfırlanır
    //    }
    //}


    void OnMouseDown()
    {
        if (canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(true);
        }

    }

    public void SpawnEv()
    {
        Instantiate(Ev, transform.position, Quaternion.identity);
        canvas.SetActive(false);

    }
    public void SpawnCannon()
    {
        Instantiate(Cannon, transform.position, Quaternion.identity);
        canvas.SetActive(false);

    }




}
