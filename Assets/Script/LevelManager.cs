using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject gamemngr;
    [SerializeField] public int Level;
    [SerializeField] int aktifYolSayisi;
    [SerializeField] float varsayilanWaveArasi = 5f;

    PlayerStats playerStats;
    GameManagerSc gameManager;
    public static LevelManager instance;

    List<Wave> aktifPlan;
    Coroutine dalgaCR;
    int currentWaveIx = 0;

    float minFill = 0.05f; // başlangıçta görünür doluluk
    int ekSpawnSayaci = 0;
    GameObject baslatGO;     // transform.GetChild(0)
    Image baslatImg;         // baslatGO üzerindeki Image
    bool startIsteği;        // butondan "başlat" isteği
    [SerializeField] List<GameObject> baslatGOs = new List<GameObject>();
    List<Image> baslatImgs = new List<Image>();

    private void Awake()
    {
        instance = this;
    }
    public void EkSpawnBasladi()
    {
        ekSpawnSayaci++;
    }
    public void EkSpawnBitti()
    {
        ekSpawnSayaci = Mathf.Max(0, ekSpawnSayaci - 1);
    }


    void Start()
    {
        playerStats = PlayerStats.Instance;
        gameManager = gamemngr.GetComponent<GameManagerSc>();
        aktifPlan = PlanSeviyesi(Level);

        if (baslatGOs.Count == 0)
        {
            foreach (var img in GetComponentsInChildren<Image>(true))
                if (img.gameObject.name.Contains("WaveStart"))
                    baslatGOs.Add(img.gameObject);
        }

        baslatImgs.Clear();
        foreach (var go in baslatGOs)
        {
            if (!go) continue;
            var c = go.GetComponent<Canvas>();
            if (c) c.worldCamera = Camera.main;
            var img = go.GetComponent<Image>();
            if (img) baslatImgs.Add(img);
            go.SetActive(true);
        }

        // Başlangıçta MAX doldur (görünsün)
        BaslatUI_SetFill(1f);
        dalgaCR = StartCoroutine(BaslatDalgalarFrom(0));

    }



    public void DalgayiBaslat()
    {
        startIsteği = true; // sadece istek işaretle
    }


    enum DusmanTuru { Kene, Karinca, Sivri, Orumcek, HamamBocegi, BokBocegi, Yusufcuk, Ari, KraliceKarinca }

    [System.Serializable]
    struct SpawnPlan
    {
        public int yol;        // 0..(aktifYolSayisi-1) veya -1 = TÜM yollar
        public DusmanTuru tur; // Kene/Karinca/Sivri/Orumcek
        public int adet;       // kaç tane
        public float aralik;   // spawn aralığı (sn)
        public float ofset;    // başlangıç gecikmesi (sn)

        public SpawnPlan(int yol, DusmanTuru tur, int adet, float aralik, float ofset = 0f)
        { this.yol = yol; this.tur = tur; this.adet = adet; this.aralik = aralik; this.ofset = ofset; }
    }

    class Wave
    {
        public List<SpawnPlan> planlar = new List<SpawnPlan>();
        public float waveArasi = 5f;   //bu dalga bittikten sonra bekleme
    }

    // ---------------- Ana akış ----------------
    IEnumerator BaslatDalgalarFrom(int startIndex)
    {
        var waves = aktifPlan;

        for (int w = startIndex; w < waves.Count; w++)
        {
            if (w == startIndex)
                yield return BekleVeGoster(0f, true); // ilk wave: sadece tık

            currentWaveIx = w;
            if (playerStats) playerStats.SetWave(w + 1, waves.Count);

            var wave = waves[w];
            foreach (var p in wave.planlar)
                StartCoroutine(HatSpawn(p));

            float tahmin = 0f;
            foreach (var p in wave.planlar)
                tahmin = Mathf.Max(tahmin, p.ofset + TahminSureTek(p.adet, p.aralik));

            yield return new WaitForSeconds(tahmin + 5f);

            while (ekSpawnSayaci > 0) // (kraliçe vb. ek spawner kilidi)
                yield return null;

            if (w < waves.Count - 1)
            {
                yield return BekleVeGoster(wave.waveArasi, false); // sonraki wave isteği (tüm butonlar senkron)
                continue;
            }

            var enemyRoot = GameObject.Find("Dusmanlar");
            if (enemyRoot != null)
                yield return new WaitUntil(() => enemyRoot.transform.childCount == 0 && ekSpawnSayaci == 0);

            OyunBittiTetikle();
            yield break;
        }
    }

    IEnumerator BekleVeGoster(float sure, bool clickOnly)
    {
        BaslatUI_SetActive(true);
        BaslatUI_SetFill(1f);   // Wave başında MAX

        startIsteği = false;

        if (clickOnly)
        {
            // İlk wave: sadece tık bekle (doluluk max kalır)
            while (!startIsteği) yield return null;
        }
        else
        {
            // Sayaç: MAX→0'a doğru azalsın (istersen sabit 1 de bırakabilirsin)
            float t = 0f;
            while (t < sure && !startIsteği)
            {
                t += Time.deltaTime;
                BaslatUI_SetFill(Mathf.Lerp(1f, 0f, Mathf.Clamp01(t / sure)));
                yield return null;
            }
        }

        BaslatUI_SetActive(false);
        startIsteği = false;
    }

    void BaslatUI_SetActive(bool state)
    {
        foreach (var go in baslatGOs)
            if (go) go.SetActive(state);
    }
    void BaslatUI_SetFill(float fill)
    {
        float v = Mathf.Max(minFill, fill);
        foreach (var img in baslatImgs)
            if (img) img.fillAmount = v;
    }

    public void RestartFromPrevWave()
    {
        int hedef = Mathf.Max(0, currentWaveIx - 1);
        if (dalgaCR != null) StopAllCoroutines();

        var enemyRoot = GameObject.Find("Dusmanlar");
        if (enemyRoot)
        {
            for (int i = enemyRoot.transform.childCount - 1; i >= 0; i--)
                Destroy(enemyRoot.transform.GetChild(i).gameObject);
        }

        dalgaCR = StartCoroutine(BaslatDalgalarFrom(hedef));
    }

    private void OyunBittiTetikle()
    {
        PauseCanvaas.Instance.KazanmaPanelAc();
        SesManagerSc.Instance.Cal("KazanmaSesi", 1f);
        if (Level == SaveSistemiSc.Instance.GetCurrentLevel())
        {
            SaveSistemiSc.Instance.LevelAtla();
        }

    }

    IEnumerator HatSpawn(SpawnPlan p)
    {
        if (p.ofset > 0f) yield return new WaitForSeconds(p.ofset);

        // yol=-1 ise tüm aktif yollara kopyala (paralel)
        if (p.yol < 0)
        {
            for (int r = 0; r < aktifYolSayisi; r++)
                StartCoroutine(HatSpawn(new SpawnPlan(r, p.tur, p.adet, p.aralik, 0f)));
            yield break;
        }
        // güvenli yol indeksi (mod)
        int yolIx = ((p.yol % aktifYolSayisi) + aktifYolSayisi) % aktifYolSayisi;
        for (int i = 0; i < p.adet; i++)
        {
            SpawnCagir(p.tur, yolIx);
            if (i < p.adet - 1 && p.aralik > 0f)
                yield return new WaitForSeconds(p.aralik);
        }
    }

    void SpawnCagir(DusmanTuru tur, int yol)
    {
        switch (tur)
        {
            case DusmanTuru.Kene: gameManager.KeneSpawn(yol); break;
            case DusmanTuru.Karinca: gameManager.KarincaSpawn(yol); break;
            case DusmanTuru.Sivri: gameManager.SivriSpawn(yol); break;
            case DusmanTuru.Orumcek: gameManager.OrumcekSpawn(yol); break;
            case DusmanTuru.HamamBocegi: gameManager.HamamBocegiSpawn(yol); break;
            case DusmanTuru.Ari: gameManager.AriSpawn(yol); break;
            case DusmanTuru.Yusufcuk: gameManager.YusufcukSpawn(yol); break;
            case DusmanTuru.BokBocegi: gameManager.BokBocegiSpawn(yol); break;
            case DusmanTuru.KraliceKarinca: gameManager.KraliceSpawn(yol); break;
        }
    }

    float TahminSureTek(int adet, float aralik) => adet > 0 ? (adet - 1) * aralik : 0f;

    // ---------------- Plan üreticiler ----------------
    SpawnPlan Ke(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Kene, a, i, o);
    SpawnPlan Ka(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Karinca, a, i, o);
    SpawnPlan Si(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Sivri, a, i, o);
    SpawnPlan Or(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Orumcek, a, i, o);
    SpawnPlan Hb(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.HamamBocegi, a, i, o);
    SpawnPlan Bo(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.BokBocegi, a, i, o);
    SpawnPlan Yu(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Yusufcuk, a, i, o);
    SpawnPlan Ar(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Ari, a, i, o);
    SpawnPlan KK(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.KraliceKarinca, a, i, o);

    Wave W(params SpawnPlan[] p)
    {
        var w = new Wave { waveArasi = varsayilanWaveArasi };
        w.planlar.AddRange(p);
        return w;
    }
    Wave WG(float gap, params SpawnPlan[] p)
    {
        var w = new Wave { waveArasi = gap };
        w.planlar.AddRange(p);
        return w;
    }

    List<Wave> PlanSeviyesi(int level)
    {
        switch (level)
        {
            case 1: return PlanLevel1();
            case 2: return PlanLevel2();
            case 3: return PlanLevel3();
            case 4: return PlanLevel4();
            case 5: return PlanLevel5();
            case 6: return PlanLevel6();
            case 7: return PlanLevel7();
            case 8: return PlanLevel8();

            default: return null;
        }
    }

    List<Wave> PlanLevel1()
    {
        var waves = new List<Wave>
        {
            W( Ka(0,  5, 3.0f, 0f) ),
            W( Ka(0, 10, 2.0f, 0f) ),
            W( Ka(0, 20, 1.0f, 0f) ),
            W( Ka(0, 30, 0.5f, 0f) ),
        };
        foreach (var w in waves) w.waveArasi = 5f;
        return waves;
    }

    List<Wave> PlanLevel2()
    {
        return new List<Wave>
        {
            W( Si(0,3,2.4f,1.2f), Ka(0,1,3.4f,1.2f),  Si(1,1,2.4f,1.2f), Ka(1,3,3.4f,1.2f) ),
            W( Si(0,4,2.1f,1.0f), Ka(0,2,3.1f,1.0f),  Si(1,3,2.1f,1.0f), Ka(1,3,3.1f,1.0f) ),
            W( Si(0,6,1.9f,0.8f), Ka(0,3,2.8f,0.8f),  Si(1,4,1.9f,0.8f), Ka(1,4,2.8f,0.8f) ),
            W( Si(0,8,1.6f,0.6f), Ka(0,6,2.4f,0.6f),  Si(1,6,1.6f,0.6f), Ka(1,6,2.4f,0.6f) ),
            W( Si(0,16,1.15f,0.35f), Ka(0,16,1.80f,0.35f),  Si(1,16,1.15f,0.35f), Ka(1,16,1.80f,0.35f) ),
            W( Si(0,20,1.05f,0.25f), Ka(0,20,1.70f,0.25f),  Si(1,20,1.05f,0.25f), Ka(1,20,1.70f,0.25f) ),
        };
    }

    List<Wave> PlanLevel3()
    {
        return new List<Wave>
        {
            WG(10f, Ka(0,6,3.6f,0.8f),Ke(0,4,2.0f,12f)),

            WG(11f, Ka(0,2,1f,0.8f),
               Ka(1,4,3.2f,0.8f) ),

            WG(13f, Ke(0,4,2.0f,0.6f), Ka(0,5,3.0f,0.6f), Si(0,5,2.2f,0.6f),
               Ke(1,4,2.0f,0.6f), Ka(1,5,3.0f,0.6f), Si(1,5,2.2f,0.6f) ),

            WG(10f, Ke(0,5,1.8f,0.6f), Ka(0,6,2.8f,0.6f), Si(0,5,2.0f,0.6f),
               Ke(1,5,1.8f,0.6f), Ka(1,6,2.8f,0.6f), Si(1,5,2.0f,0.6f) ),

            WG(10f, Ke(0,12,1.6f,0.5f), Ka(0,8,2.6f,0.5f), Si(0,5,1.8f,0.5f),
               Ke(1,12,1.6f,0.5f), Ka(1,8,2.6f,0.5f), Si(1,5,1.8f,0.5f) ),

            WG(10f, Ke(0,10,1.4f,0.4f), Ka(0,15,2.4f,0.4f), Si(0,4,1f,0.4f),
               Ke(1,10,1.4f,0.4f), Ka(1,15,2.4f,0.4f), Si(1,4,1f,0.4f) ),

            WG(10f, Ke(0,15,1.2f,0.4f), Ka(0,16,2.2f,0.4f), Si(0,5,1.4f,0.4f),
               Ke(1,15,1.2f,0.4f), Ka(1,16,2.2f,0.4f), Si(1,5,1f,0.4f) ),

            WG(7f, Ke(0,20,1.0f,0.3f), Ka(0,14,2.0f,0.3f), Si(0,6,1f,0.3f),
               Ke(1,20,1.0f,0.3f), Ka(1,14,2.0f,0.3f), Si(1,6,1f,0.3f)),

        };
    }

    List<Wave> PlanLevel4()
    {
        return new List<Wave>
        {
            WG(10f, Ka(0,14,2.4f,0.4f), Si(0,2,2.2f,0.5f),
                Ka(1,14,2.4f,0.8f), Si(1,2,2.2f,0.9f) ),

            WG(10f, Ka(0,7,1f,0.4f), Or(0,2,2.8f,0.9f),
               Ka(1,7,1f,0.8f), Or(1,2,2.8f,1.1f) ),

            WG(6f, Ka(0,8,2.6f,0.4f), Ke(0,2,1.8f,0.6f), Or(0,3,2.6f,0.8f), Si(0,2,2.4f,0.5f),
               Ka(1,8,2.6f,0.8f), Ke(1,3,1.8f,1.0f), Or(1,3,2.6f,1.0f), Si(1,2,2.4f,0.9f) ),

            WG(6f, Ka(0,10,2.4f,0.4f), Ke(0,3,1.6f,0.6f), Or(0,8,2.4f,0.9f), Si(0,2,2.2f,0.5f),
               Ka(1,10,2.4f,0.8f), Ke(1,6,1.6f,1.0f), Or(1,3,2.4f,1.1f), Si(1,2,2.2f,0.9f) ),

            WG(10f, Ka(0,12,2.2f,0.4f), Ke(0,12,1.5f,0.6f), Or(0,5,2.2f,0.9f), Si(0,3,2.0f,0.5f),
               Ka(1,12,2.2f,0.8f), Ke(1,12,1.5f,1.0f), Or(1,5,2.2f,1.1f), Si(1,3,2.0f,0.9f) ),

            WG(10f, Ka(0,14,2.0f,0.4f), Ke(0,14,1.4f,0.6f), Or(0,6,2.0f,0.9f), Si(0,4,1.8f,0.5f),
               Ka(1,14,2.0f,0.8f), Ke(1,14,1.4f,1.0f), Or(1,6,2.0f,1.1f), Si(1,4,1.8f,0.9f) ),

            WG(7f, Ka(0,16,1.9f,0.4f), Ke(0,16,1.3f,0.6f), Or(0,7,1.9f,0.9f), Si(0,4,1.7f,0.5f),
               Ka(1,16,1.9f,0.8f), Ke(1,16,1.3f,1.0f), Or(1,7,1.9f,1.1f), Si(1,4,1.7f,0.9f) ),

            WG(6f, Ka(0,18,1.8f,0.4f), Ke(0,18,1.2f,0.6f), Or(0,8,1.8f,0.9f), Si(0,5,1.6f,0.5f),
               Ka(1,18,1.8f,0.8f), Ke(1,18,1.2f,1.0f), Or(1,8,1.8f,1.1f), Si(1,5,1.6f,0.9f) ),
        };
    }

    List<Wave> PlanLevel5()
    {
        return new List<Wave>
    {

        WG(7f,Or(0,2,3.2f,0.6f), Ka(0,5,3.0f,0.4f)),

         WG(6f,Hb(0,1,3.4f,6.6f), Ka(0,7,2.8f,0.4f)),

         WG(8f,Hb(0,3,3.2f,0.6f), Or(0,2,2.8f,0.9f), Ka(0,8,2.6f,0.4f)),

        WG(12f, Hb(0,4,3.0f,0.6f), Or(0,3,2.6f,0.9f), Ka(0,9,2.4f,0.4f), Ke(0,6,1.9f,0.7f) ),

        WG(6f, Hb(0,5,2.8f,0.6f), Or(0,3,2.4f,0.9f), Ka(0,10,2.2f,0.4f), Ke(0,8,1.8f,0.7f), Si(0,2,2.0f,0.5f)),

        WG(13f, Hb(0,6,2.6f,0.6f), Or(0,4,2.3f,0.9f), Ka(0,12,2.0f,0.4f), Ke(0,10,1.7f,0.7f), Si(0,2,1.9f,0.5f) ),

        WG(6f,Hb(0,7,2.5f,0.6f), Or(0,5,2.2f,0.9f), Ka(0,14,1.9f,0.4f), Ke(0,12,1.6f,0.7f), Si(0,3,1.8f,0.5f)),

        WG(8f,Hb(0,12,1.5f,0.6f), Or(0,8,2.2f,0.9f), Ka(0,10,1.9f,0.4f), Ke(0,30,0.5f,10f), Si(0,18,0.5f,15f)),
        
        WG(20f,Hb(0,14,1.4f,0.6f), Or(0,6,2.0f,0.9f), Ka(0,16,1.8f,0.4f), Ke(0,45,0.2f,20f), Si(0,10,1.7f,0.5f) ),

        WG(12f,KK(0,1,0f,2f),Ka(0,1,1f,40f))

    };
    }

    List<Wave> PlanLevel6()
    {
        return new List<Wave>
    {
        WG(6f,
            Bo(0,1,3.6f,5f), Ka(0,5,3.0f,0.4f),
            Ka(1,6,2f,0.8f)
        ),

        WG(12f,
            Ka(0,5,2.8f,0.4f),Bo(0,1,3.6f,2.6f),
            Hb(1,1,3.6f,6.9f), Ka(1,5,2.8f,0.8f)
        ),

        WG(12f,
            Hb(0,1,3.2f,6f), Or(0,1,2.8f,0.9f), Ka(0,8,2.6f,0.4f),  Bo(0,1,3.6f,2.6f),
            Bo(1,1,3.6f,2.6f),Or(1,1,2.8f,1.2f), Ka(1,8,2.6f,0.8f)
        ),

        WG(15f,
            Hb(0,4,3.0f,0.6f), Or(0,3,2.6f,0.9f), Ka(0,9,2.4f,0.4f), Ke(0,6,1.9f,0.7f),Bo(0,3,3.6f,2.6f),
             Ka(1,9,2.4f,0.8f), Ke(1,6,1.9f,1.1f)
        ),

        WG(18f,
            Hb(0,3,2.8f,0.6f), Or(0,3,2.4f,0.9f), Ka(0,10,2.2f,0.4f), Ke(0,8,1.8f,0.7f),  Bo(0,3,3.6f,2.6f),
            Hb(1,3,3.0f,15f), Or(1,3,2.4f,1.2f), Ka(1,10,2.2f,0.8f),  Si(1,20,0.5f,0.9f)
        ),

        WG(19f,
            Hb(0,5,2.6f,0.6f), Or(0,4,2.3f,0.9f), Ka(0,12,2.0f,0.4f), Ke(0,6,1.7f,0.7f), Si(0,2,1.9f,0.5f),
            Hb(1,5,2.8f,1.0f), Or(1,4,2.3f,1.2f), Ka(1,12,2.0f,0.8f), Ke(1,6,1.7f,1.1f), Si(1,2,1.9f,0.9f)
        ),

        WG(17f,
            Hb(0,3,5f,0.6f),Bo(0,3,4f,6f), Or(0,5,2.2f,0.9f), Ka(0,7,1.9f,0.4f), Ke(0,12,1.6f,0.7f), Si(0,3,1.8f,0.5f),
            Hb(1,3,5f,1.0f),Bo(1,3,4f,6f), Or(1,5,2.2f,1.2f), Ka(1,7,1.9f,0.8f),  Si(1,3,1.8f,0.9f)
        ),

        WG(18f,
            Hb(0,8,2.4f,0.6f), Or(0,6,2.0f,0.9f), Ka(0,8,1.8f,0.4f), Ke(0,14,1.5f,0.7f), Si(0,4,1.7f,0.5f),
            Hb(1,7,2.5f,1.0f), Or(1,6,2.0f,1.2f), Ka(1,8,1.8f,0.8f), Ke(1,14,1.5f,1.1f), Si(1,4,1.7f,0.9f)
        ),
         WG(14f,
            Bo(0,20,2f,0f),
             Bo(1,20,2f,0f)
        ),
    };
    }
    List<Wave> PlanLevel7()
    {
        return new List<Wave>
    {
        WG(8f,
             Ka(0,10,1.0f,0.4f), Or(0,2,2.8f,0.9f),Hb(0,1,1f,6f)
        ),

        WG(6f,
            Hb(0,1,3.4f,6.6f), Ka(0,7,2.8f,0.4f),Ar(0,2,2.4f,7f)
        ),

        WG(6f,
            Hb(0,2,3.2f,6f), Or(0,2,2.8f,0.9f), Ka(0,8,2.6f,0.4f),  Ar(0,1,2.4f,7f)
        ),

        WG(10f,
            Hb(0,4,3.0f,0.6f), Or(0,3,2.6f,0.9f), Ka(0,9,2.4f,0.4f), Ke(0,6,1.9f,0.7f),Bo(0,5,3f,0f)
        ),

        WG(15f,
            Hb(0,5,2.8f,0.6f), Or(0,3,2.4f,0.9f), Ka(0,10,2.2f,0.4f), Ke(0,8,1.8f,0.7f), Si(0,2,2.0f,0.5f), Ar(0,3,2.2f,7f),Bo(0,2,3f,8f)
        ),

        WG(12f,
            Hb(0,4,2.6f,0.6f), Or(0,4,2.3f,0.9f), Ka(0,6,2.0f,0.4f), Ke(0,10,1.7f,0.7f), Si(0,2,1.9f,0.5f), Ar(0,4,2.1f,10f),Bo(0,4,3f,8f)
        ),

        WG(13f,
            Hb(0,7,2.5f,0.6f), Or(0,5,2.2f,0.9f), Ka(0,14,1.9f,0.4f), Ke(0,12,1.6f,0.7f), Si(0,3,1.8f,0.5f), Ar(0,5,2.0f,9f),Bo(0,6,3f,08f)
        ),

        WG(14f,
            Hb(0,8,2.4f,0.6f), Or(0,6,2.0f,0.9f), Ka(0,16,1.8f,0.4f), Ke(0,14,1.5f,0.7f), Si(0,4,1.7f,0.5f), Ar(0,6,1.9f,10f),Bo(0,6,3f,08f)

        ),
         WG(14f,
            Hb(0,14,2.0f,5f), Or(0,6,2.0f,0.9f), Ka(0,16,1.8f,0.4f), Ke(0,14,1.5f,0.7f), Si(0,4,1.7f,0.5f), Ar(0,6,1.9f,10f),Bo(0,6,3f,15f)

        ),
    };
    }

    // ----- LEVEL 8 (3 yol, 12 wave; W1-2'de Sivri/Ari YOK; karınca farm odaklı) -----
    List<Wave> PlanLevel8()
    {
        return new List<Wave>
    {
        // W1 (senin verdiğin; Sivri/Ari/Kene yok)  ≈G: 6*10*3 + 3*15*3 + 1*20 + 1*30 = 180 + 135 + 20 + 30 = 365
        WG(6f,
            Ka(0,6,2.6f,0.4f), Or(0,3,2.8f,0.9f), Bo(0,1,2.6f,12f),
            Ka(1,6,2.5f,0.8f), Or(1,3,2.8f,1.2f), Hb(1,1,3.6f,6f),
            Ka(2,6,2.7f,0.6f), Or(2,3,2.8f,1.2f)
        ),

        // W2 – yer odaklı “farm” (Sivri/Ari yok), Karınca bol  ≈G: ~ (Ka ~360) + (Ke ~120) + (Or ~90) + (Hb ~30) ≈ 600+
        WG(6f,
            Ka(0,12,2.4f,0.4f), Ke(0,6,1.8f,0.7f), Or(0,2,2.6f,1.1f),
            Ka(1,12,2.3f,0.8f), Hb(1,1,3.4f,1.3f), Ke(1,8,1.8f,1.1f),
            Ka(2,12,2.3f,0.6f), Ke(2,6,1.8f,0.9f), Or(2,2,2.6f,1.2f)
        ),

        // W3 – hava tanıtımı (Sivri/Ari az)  ≈G: orta
        WG(6f,
            Ka(0,8,2.3f,0.4f), Ke(0,6,1.8f,0.7f), Si(0,2,2.2f,1.0f), Ar(0,1,2.3f,1.4f),
            Ka(1,8,2.3f,0.8f), Or(1,3,2.5f,1.0f),
            Ka(2,8,2.3f,0.6f), Ke(2,5,1.8f,0.9f), Ar(2,1,2.3f,1.5f)
        ),

        // W4 – Bok (900 HP) ağırlığı, hava yok  ≈G: yüksek (Bok 20’ler)
        WG(6f,
            Bo(0,5,2.2f,0.5f), Si(0,8,2.2f,0.8f), Ke(0,6,1.7f,1.0f),
            Bo(1,6,2.1f,0.9f), Ka(1,6,2.3f,1.1f), Or(1,3,2.3f,1.3f),
            Bo(2,5,2.2f,0.7f), Ka(2,7,2.2f,0.9f), Hb(2,1,3.2f,1.4f)
        ),

        // W5 – Kene patlaması (HP 120, altın 3): sayıyı artır ama akış serin  ≈G: orta
        WG(6f,
            Ke(0,18,1.5f,0.5f), Ka(0,6,2.2f,0.8f),
            Ke(1,20,1.5f,0.9f), Ka(1,5,2.3f,1.1f),
            Ke(2,16,1.6f,0.7f), Ka(2,6,2.1f,1.0f), Or(2,2,2.3f,1.3f)
        ),

        // W6 – Hamam (1200 HP) yoğun; hava yok  ≈G: orta-yüksek
   
        WG(6f,
            Hb(0,4,3.0f,0.6f), Ka(0,8,2.3f,0.4f), Or(0,3,2.4f,0.9f),
            Hb(1,3,3.2f,0.9f), Ke(1,8,1.7f,1.1f), Bo(1,3,2.3f,1.3f),
            Hb(2,4,3.0f,0.7f), Ka(2,7,2.3f,0.5f), Or(2,2,2.4f,1.0f)
        ),

        // W7 – Örümcek baskısı; sınırlı hava  ≈G: orta
        WG(6f,
            Or(0,6,2.1f,0.6f), Ka(0,8,2.0f,0.4f), Ke(0,6,1.7f,0.9f),
            Or(1,7,2.0f,0.9f), Ka(1,6,2.1f,1.1f), Bo(1,3,2.2f,1.3f),
            Or(2,6,2.1f,0.7f), Ka(2,7,2.0f,0.5f), Ke(2,5,1.7f,1.0f)
        ),

        // W8 – Karınca sürüsü (hava yok), oyuncu rahat farmlasın  ≈G: yüksek (saf Ka)
        WG(1f,
            Ka(0,18,1.9f,0.4f),Bo(0,3,2.2f,1.3f),
            Ka(1,18,1.8f,0.8f), Or(1,3,2.1f,1.2f),
            Ka(2,18,1.8f,0.6f),Bo(2,3,2.2f,1.3f)
        ),

        // W9 – Hava dalgası (Sivri/Ari yüksek), yer düşük  ≈G: orta
        WG(6f,
            Si(0,8,1.9f,0.6f), Ar(0,11,1.7f,0.9f), Ka(0,4,2.3f,1.2f),
            Si(1,7,1.8f,0.8f), Ar(1,11,1.7f,1.1f), Ke(1,3,1.9f,1.3f),
            Si(2,9,1.9f,0.7f), Ar(2,11,1.7f,1.0f), Or(2,2,2.2f,1.3f)
        ),

        // W10 – Yer baskısı (Bok + Karınca), hava yok  ≈G: yüksek
        WG(6f,
            Bo(0,7,2.0f,0.5f), Ka(0,12,1.9f,0.7f), Ke(0,4,1.8f,1.0f),
            Bo(1,8,1.9f,0.9f), Ka(1,10,1.9f,1.1f), Or(1,3,2.0f,1.3f),
            Bo(2,7,2.0f,0.7f), Ka(2,11,1.9f,0.9f), Hb(2,2,3.0f,1.4f)
        ),

        // W11 – “Miniboss” hissi: Hamam + Örümcek; hava çok sınırlı  ≈G: yüksek
        WG(7f,
            Hb(0,5,2.8f,0.6f), Or(0,5,2.0f,0.9f), Ka(0,6,2.0f,0.4f),Bo(0,5,1.9f,1.1f),
            Hb(1,4,2.9f,0.9f), Or(1,5,2.0f,1.1f), Ke(1,5,1.7f,1.3f),Bo(1,2,1.9f,1.1f),
            Hb(2,5,2.8f,0.7f), Or(2,4,2.0f,1.0f), Ka(2,5,2.0f,0.5f),Bo(2,2,0.1f,0f)
        ),

        // W12 – Final karışımı (hepsi var ama patlama yok; dengeli)  ≈G: yüksek
        WG(7f,
            Ka(0,6,1.9f,0.4f), Ke(0,12,1.6f,0.7f), Or(0,14,1.9f,1.0f), Si(0,4,1.8f,1.3f), Ar(0,4,1.9f,1.5f), Bo(0,6,1.9f,1.1f), Hb(0,3,1f,1.7f),
            Ka(1,6,1.9f,0.8f), Ke(1,10,1.6f,1.0f), Or(1,14,1.9f,1.2f), Si(1,3,1.8f,1.4f), Ar(1,4,1.9f,1.6f), Bo(1,6,1.9f,1.3f), Hb(1,3,1f,1.8f),
            Ka(2,6,1.9f,0.6f), Ke(2, 9,1.6f,0.9f), Or(2,14,1.9f,1.1f), Si(2,4,1.8f,1.3f), Ar(2,3,1.9f,1.5f), Bo(2,6,1.9f,1.2f), Hb(2,3,1f,1.7f)
        ),
    };
    }


}
