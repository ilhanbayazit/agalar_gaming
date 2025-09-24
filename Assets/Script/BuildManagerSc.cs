using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManagerSc : MonoBehaviour
{
    [SerializeField] GameObject KurdanAtar;
    [SerializeField] GameObject Tuzluk;
    [SerializeField] GameObject ZeytinAtar;
    [SerializeField] GameObject FindikAtar;
    [SerializeField] GameObject CekirdekAtar;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject plyrsts;
    PlayerStats Stats;
    GameObject Bina;
    bool BosMu = true;

    private void Start()
    {
        Stats = plyrsts.GetComponent<PlayerStats>();
    }
    void OnMouseDown()
    {
        if (canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
        else if (BosMu)
        {
            canvas.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(CanvasKapat());
        }
    }

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Destroy(Bina);
            BosMu = true;
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

    public void SpawnKurdanAtar()
    {
        if (Stats.AltinSayisi >= 100)
        {
            Bina = Instantiate(KurdanAtar, transform.position, Quaternion.identity);
            canvas.SetActive(false);
            BosMu = false;
            Stats.AltinSil(100);
        }
    }
    public void SpawnZeytinAtar()
    {
        if (Stats.AltinSayisi >= 150)
        {
            Bina = Instantiate(ZeytinAtar, transform.position, Quaternion.identity);
            canvas.SetActive(false);
            BosMu = false;
            Stats.AltinSil(150);
        }
    }
    public void SpawnCekirdekAtar()
    {
        if (Stats.AltinSayisi >= 90)
        {
            Bina = Instantiate(CekirdekAtar, transform.position, Quaternion.identity);
            canvas.SetActive(false);
            BosMu = false;
            Stats.AltinSil(90);
        }
    }

    public void SpawnFindikAtar()
    {
        if (Stats.AltinSayisi >= 200)
        {
            Bina = Instantiate(FindikAtar, transform.position, Quaternion.identity);
            canvas.SetActive(false);
            BosMu = false;
            Stats.AltinSil(200);
        }
    }

    public void SpawnTuzluk()
    {
        if (Stats.AltinSayisi >= 120)
        {
            Bina = Instantiate(Tuzluk, transform.position, Quaternion.identity);
            canvas.SetActive(false);
            BosMu = false;
            Stats.AltinSil(120);
        }

    }



}
