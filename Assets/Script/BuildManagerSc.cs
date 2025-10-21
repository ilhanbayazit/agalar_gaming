using NUnit.Framework;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildManagerSc : MonoBehaviour, IPointerClickHandler
{
    public static BuildManagerSc aktif;
    [SerializeField] GameObject KurdanAtar;
    [SerializeField] GameObject Tuzluk;
    [SerializeField] GameObject ZeytinAtar;
    [SerializeField] GameObject FindikAtar;
    [SerializeField] GameObject CekirdekAtar;
    [SerializeField] GameObject FistikAtar;
    [SerializeField] GameObject HavaSavunmasi;

    [SerializeField] GameObject canvas;
    GameObject panelSatinAlim;
    GameObject panelYukseltSat;

    [SerializeField] GameObject plyrsts;
    TowerInfo aktifInfo;
    PlayerStats Stats;
    GameObject Bina;
    bool BosMu = true;

    [SerializeField] Image yukseltImage;
    Sprite yukseltImageDefault;
    [SerializeField] Sprite kilitfoto;

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
        atama();
        yukseltImageDefault = yukseltImage != null ? yukseltImage.sprite : null;
    }


    void OnMouseDown()
    {
        if (aktif != null && aktif != this) return;
        CanvasGuncelle();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (aktif != null && aktif != this) return;
        CanvasGuncelle();
    }

    void CanvasGuncelle()
    {
        if (!canvas) return;
        if (PauseCanvaas.Instance.OyunDurduMu) return;
        if (aktif != null && aktif != this) return;

        canvas.SetActive(true);
        if (panelSatinAlim) panelSatinAlim.SetActive(BosMu);
        if (panelYukseltSat) panelYukseltSat.SetActive(!BosMu);

        aktif = this;

        // nextLevelPrefab durumuna göre ikon + renk (para yeterse yeşil, yetmezse kırmızı)
        if (!BosMu && yukseltImage != null)
        {
            var cur = Bina ? Bina.GetComponent<TowerInfo>() : null;

            if (cur == null || cur.nextLevelPrefab == null)
            {
                yukseltImage.sprite = kilitfoto;
                // kilitliyken nötr renk
                var c = yukseltImage.color; yukseltImage.color = new Color(1f, 1f, 1f, c.a);
            }
            else
            {
                if (yukseltImageDefault != null)
                    yukseltImage.sprite = yukseltImageDefault;

                var nextInfo = cur.nextLevelPrefab.GetComponent<TowerInfo>();
                if (nextInfo != null)
                {
                    bool afford = Stats.AltinSayisi >= nextInfo.buildCost;
                    var c = yukseltImage.color;
                    yukseltImage.color = afford ? new Color(0.8f, 1f, 0.8f, c.a)   // yeşil ton
                                                : new Color(1f, 0.8f, 0.8f, c.a); // kırmızı ton
                }
            }
        }

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
    public void SpawnHavaSavunmasi() => Spawn(HavaSavunmasi);


    void Spawn(GameObject prefab)
    {
        var info = prefab.GetComponent<TowerInfo>();
        if (info == null) return;
        if (Stats.AltinSayisi < info.buildCost) return;
        Bina = Instantiate(prefab, transform.position, Quaternion.identity);
        aktifInfo = Bina.GetComponent<TowerInfo>();
        BosMu = false;
        canvas.SetActive(false);
        var rg = Bina.AddComponent<RangeGoster>();
        rg.SetRadius(info.menzil);
        Stats.AltinSil(info.buildCost);
        BuildingEffect();
        PanelKapat();
    }

    // --- Yukselt() ---
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
        BuildingEffect();
        aktifInfo = Bina.GetComponent<TowerInfo>();
        Stats.AltinSil(nextInfo.buildCost);

        // ikon + renk güncelle (bir sonraki seviyenin bedeline göre)
        if (yukseltImage != null)
        {
            var cur2 = Bina ? Bina.GetComponent<TowerInfo>() : null;
            if (cur2 == null || cur2.nextLevelPrefab == null)
            {
                yukseltImage.sprite = kilitfoto;
                var c0 = yukseltImage.color; yukseltImage.color = new Color(1f, 1f, 1f, c0.a);
            }
            else
            {
                if (yukseltImageDefault != null)
                    yukseltImage.sprite = yukseltImageDefault;

                var nextInfo2 = cur2.nextLevelPrefab.GetComponent<TowerInfo>();
                if (nextInfo2 != null)
                {
                    bool afford2 = Stats.AltinSayisi >= nextInfo2.buildCost;
                    var c = yukseltImage.color;
                    yukseltImage.color = afford2 ? new Color(0.8f, 1f, 0.8f, c.a)
                                                 : new Color(1f, 0.8f, 0.8f, c.a);
                }
            }
        }

        PanelKapat();
    }

    [SerializeField] ParticleSystem fxPrefab;
    public void BuildingEffect()
    {
        if (fxPrefab != null)
        {
            var fx = Instantiate(fxPrefab, transform.position, Quaternion.identity);
            fx.Play();
            var main = fx.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            Destroy(fx.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }

    #region Kilit Mekanizmasi

    [System.Serializable]
    public struct Slot
    {
        public int minLevel;
        public Button button;
        public Sprite locked;
    }

    [SerializeField] Slot[] slots;
    Sprite[] slotDefaultSprites;
    void atama()
    {
        slotDefaultSprites = new Sprite[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            var b = slots[i].button;
            slotDefaultSprites[i] = (b && b.image) ? b.image.sprite : null; // mevcut (unlocked) sprite'ı kaydet
        }
        Refresh(LevelManager.instance.Level);
    }
    public void Refresh(int playerLevel)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var s = slots[i];
            if (s.button == null) continue;

            bool open = playerLevel >= s.minLevel;
            var img = s.button.image;

            if (img != null)
                img.sprite = open
                    ? (slotDefaultSprites[i] ?? img.sprite)     // açık: orijinal sprite
                    : (s.locked != null ? s.locked : img.sprite); // kilitli: locked sprite

            s.button.interactable = open;

            // silikleşmeyi kapat (disabledColor'ı beyaz yap)
            var c = s.button.colors;
            c.disabledColor = Color.white;
            s.button.colors = c;
        }


    }


    #endregion

}
