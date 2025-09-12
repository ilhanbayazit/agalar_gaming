using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManagerSc : MonoBehaviour
{
    [SerializeField] GameObject KurdanAtar;
    [SerializeField] GameObject Tuzluk;
    [SerializeField] GameObject canvas;
    bool BosMu=true;


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
        if (canvas.activeSelf )
        {
            canvas.SetActive(false);
        }
        else if(BosMu)
        {
            canvas.SetActive(true);
            StopAllCoroutines(); 
            StartCoroutine(CanvasKapat());
        }

    }
    IEnumerator CanvasKapat()
    {
        yield return new WaitForSeconds(1.2f); 
        if (canvas.activeSelf)             
        {
            canvas.SetActive(false);
        }
    }

    public void SpawnEv()
    {
        Instantiate(Tuzluk, transform.position, Quaternion.identity);
        canvas.SetActive(false);
        BosMu = false;
    }
    public void SpawnCannon()
    {
        Instantiate(KurdanAtar, transform.position, Quaternion.identity);
        canvas.SetActive(false);
        BosMu = false;

    }




}
