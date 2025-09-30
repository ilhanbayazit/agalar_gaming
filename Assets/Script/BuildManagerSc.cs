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

    void Awake()
    {
        // İsimle derin arama (inactive çocuklarda da çalışır)
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
        CanvasGuncelle();
    }
    void CanvasGuncelle()
    {
        if (!canvas) return;
        canvas.SetActive(true);
        if (panelSatinAlim) panelSatinAlim.SetActive(BosMu);
        if (panelYukseltSat) panelYukseltSat.SetActive(!BosMu);
        StopAllCoroutines();
        StartCoroutine(CanvasKapat());
    }



    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Destroy(Bina);
            BosMu = true;
        }
    }

    public void BinaSil()
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
        yield return new WaitForSeconds(1.2f);
        if (canvas && canvas.activeSelf) canvas.SetActive(false);
    }

    public void SpawnKurdanAtar() => Spawn(KurdanAtar);
    public void SpawnZeytinAtar() => Spawn(ZeytinAtar);
    public void SpawnCekirdekAtar() => Spawn(CekirdekAtar);
    public void SpawnFindikAtar() => Spawn(FindikAtar);
    public void SpawnTuzluk() => Spawn(Tuzluk);

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

        CanvasGuncelle(); // istersen paneli güncelle
    }
}
