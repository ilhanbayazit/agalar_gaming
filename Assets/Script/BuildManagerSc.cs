using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManagerSc : MonoBehaviour
{
    [SerializeField] GameObject KurdanAtar;
    [SerializeField] GameObject Tuzluk;
    [SerializeField] GameObject ZeytinAtar;
    [SerializeField] GameObject canvas;
    bool BosMu=true;


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

    public void SpawnTuzlukAtar()
    {
        Instantiate(Tuzluk, transform.position, Quaternion.identity);
        canvas.SetActive(false);
        BosMu = false;
    }
    public void SpawnKurdanAtar()
    {
        Instantiate(KurdanAtar, transform.position, Quaternion.identity);
        canvas.SetActive(false);
        BosMu = false;
    }
    public void SpawnZeytinAtar()
    {
        Instantiate(ZeytinAtar, transform.position, Quaternion.identity);
        canvas.SetActive(false);
        BosMu = false;
    }



}
