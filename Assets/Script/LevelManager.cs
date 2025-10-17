using System;
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
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerStats = PlayerStats.Instance;
        gameManager = gamemngr.GetComponent<GameManagerSc>();
        aktifPlan = PlanSeviyesi(Level);
        dalgaCR = StartCoroutine(BaslatDalgalarFrom(0));
    }


    // ---------------- Dinamik veri yapıları ----------------
    enum DusmanTuru { Kene, Karinca, Sivri, Orumcek, HamamBocegi, BokBocegi, Yusufcuk, Ari }

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
            currentWaveIx = w;
            if (playerStats) playerStats.SetWave(w + 1, waves.Count);

            var wave = waves[w];
            foreach (var p in wave.planlar)
                StartCoroutine(HatSpawn(p));

            float tahmin = 0f;
            foreach (var p in wave.planlar)
                tahmin = Mathf.Max(tahmin, p.ofset + TahminSureTek(p.adet, p.aralik));

            yield return new WaitForSeconds(tahmin + 5f);
            yield return new WaitForSeconds(wave.waveArasi);

            if (w == waves.Count - 1)
            {
                var enemyRoot = GameObject.Find("Dusmanlar");
                if (enemyRoot != null)
                    yield return new WaitUntil(() => enemyRoot.transform.childCount == 0);

                OyunBittiTetikle();
                yield break;
            }
        }
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

            default: return PlanLevel3();
        }
    }

    // ----- LEVEL 1 (tek yol: 0, sadece Karınca; 5/10/20/30) -----
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

    // ----- LEVEL 2 (Sivri + Karınca, iki yol) -----
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

    // ----- LEVEL 3 (Kene + Karınca + Sivri; W1'de kene yok) -----
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

    // ----- LEVEL 4 (karınca + kene + örümcek ağırlıklı, karışık; az sivri) -----
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

        WG(12f,Hb(0,3,3.2f,0.6f), Or(0,2,2.8f,0.9f), Ka(0,8,2.6f,0.4f)),

        WG(12f, Hb(0,4,3.0f,0.6f), Or(0,3,2.6f,0.9f), Ka(0,9,2.4f,0.4f), Ke(0,6,1.9f,0.7f) ),

        WG(6f, Hb(0,5,2.8f,0.6f), Or(0,3,2.4f,0.9f), Ka(0,10,2.2f,0.4f), Ke(0,8,1.8f,0.7f), Si(0,2,2.0f,0.5f)),

       WG(13f, Hb(0,6,2.6f,0.6f), Or(0,4,2.3f,0.9f), Ka(0,12,2.0f,0.4f), Ke(0,10,1.7f,0.7f), Si(0,2,1.9f,0.5f) ),

        WG(6f,Hb(0,7,2.5f,0.6f), Or(0,5,2.2f,0.9f), Ka(0,14,1.9f,0.4f), Ke(0,12,1.6f,0.7f), Si(0,3,1.8f,0.5f)),

        WG(6f,Hb(0,14,1.4f,0.6f), Or(0,6,2.0f,0.9f), Ka(0,16,1.8f,0.4f), Ke(0,14,1.5f,0.7f), Si(0,4,1.7f,0.5f) ),
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
            Bo(0,20,2.7f,0f),
             Bo(1,20,2.7f,0f)
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
            Hb(0,14,2.0f,5f), Or(0,6,2.0f,0.9f), Ka(0,16,1.8f,0.4f), Ke(0,14,1.5f,0.7f), Si(0,4,1.7f,0.5f), Ar(0,6,1.9f,10f)

        ),
    };
    }

}
