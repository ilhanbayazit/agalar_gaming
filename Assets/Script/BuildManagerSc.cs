using System.Collections;
using TMPro.Examples;
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
    [SerializeField] GameObject FistikAtar;

    [SerializeField] GameObject canvas;
    GameObject panelSatinAlim;
    GameObject panelYukseltSat;

    [SerializeField] GameObject plyrsts;
    TowerInfo aktifInfo;
    PlayerStats Stats;
    GameObject Bina;
    bool BosMu = true;
    static BuildManagerSc aktif;
    void Awake()
    {
        panelSatinAlim = CocukBul(canvas.transform, "SatinAlimEkrani");
        panelYukseltSat = CocukBul(canvas.transform, "YukseltmeVeSatma");
    }
    static GameObject CocukBul(Transform parent, string ad)
    {
        foreach (var t in parent.GetComponentsInChildren<Transform>(true))
            if (t.name == ad) return t.gameObject;
        return null;
    }

    void Start()
    {
        Stats = plyrsts.GetComponent<PlayerStats>();
    }
    void OnMouseDown()
    {
        if (aktif != null && aktif != this) return;
        CanvasGuncelle();
    }
    void CanvasGuncelle()
    {
        if (!canvas) return;

        // Başka bir BuildManagerSc paneli açıkken açmayı engelle
        if (aktif != null && aktif != this) return;

        canvas.SetActive(true);
        if (panelSatinAlim) panelSatinAlim.SetActive(BosMu);
        if (panelYukseltSat) panelYukseltSat.SetActive(!BosMu);

        // Artık bu aktif
        aktif = this;

        StopAllCoroutines();
        StartCoroutine(CanvasKapat());
    }



    public void PanelKapat()
    {
        if (canvas && canvas.activeSelf) canvas.SetActive(false);
        if (aktif == this) aktif = null;
    }
    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Destroy(Bina);
            BosMu = true;
        }
    }

    public void BinaSat()
    {

        if (Bina)
        {
            Stats.AltinEkle(Bina.GetComponent<TowerInfo>().SatisFiyati);
            Destroy(Bina);
        }
        Bina = null;
        BosMu = true;
        CanvasGuncelle();

    }

    IEnumerator CanvasKapat()
    {
        yield return new WaitForSeconds(2.2f);
        if (canvas && canvas.activeSelf) canvas.SetActive(false);
        if (aktif == this) aktif = null;
    }

    public void SpawnKurdanAtar() => Spawn(KurdanAtar);
    public void SpawnZeytinAtar() => Spawn(ZeytinAtar);
    public void SpawnCekirdekAtar() => Spawn(CekirdekAtar);
    public void SpawnFindikAtar() => Spawn(FindikAtar);
    public void SpawnTuzluk() => Spawn(Tuzluk);
    public void SpawnFistikAtar() => Spawn(FistikAtar);

    void Spawn(GameObject prefab)
    {
        var info = prefab.GetComponent<TowerInfo>();

        if (info == null) return;
        if (Stats.AltinSayisi < info.buildCost) return;

        Bina = Instantiate(prefab, transform.position, Quaternion.identity);
        aktifInfo = Bina.GetComponent<TowerInfo>();
        BosMu = false;
        canvas.SetActive(false);
        Stats.AltinSil(info.buildCost);
    }

    public void Yukselt()
    {
        if (Bina == null) return;

        var cur = Bina.GetComponent<TowerInfo>();
        if (cur == null) return;

        var nextPrefab = cur.nextLevelPrefab;
        if (nextPrefab == null) return;

        var nextInfo = nextPrefab.GetComponent<TowerInfo>();
        if (nextInfo == null) return;
        if (Stats.AltinSayisi < nextInfo.buildCost) return;

        Destroy(Bina);
        Bina = Instantiate(nextPrefab, transform.position, Quaternion.identity);

        aktifInfo = Bina.GetComponent<TowerInfo>();
        Stats.AltinSil(nextInfo.buildCost);
        //      CanvasGuncelle(); // istersen paneli güncelle

        PanelKapat();
    }
}
