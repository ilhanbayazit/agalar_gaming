using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject gamemngr;
    [SerializeField] int Level = 3;
    [SerializeField] int aktifYolSayisi;

    GameManagerSc gameManager;

    void Start()
    {
        gameManager = gamemngr.GetComponent<GameManagerSc>();
        var plan = PlanSeviyesi(Level);
        StartCoroutine(BaslatDalgalar(plan));
    }

    // ---------------- Dinamik veri yapıları ----------------
    enum DusmanTuru { Kene, Karinca, Sivri, Orumcek }

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
        public float waveArasi = 3f; // bu dalga bittikten sonra bekleme
    }

    // ---------------- Ana akış ----------------
    IEnumerator BaslatDalgalar(List<Wave> waves)
    {
        for (int w = 0; w < waves.Count; w++)
        {
            var wave = waves[w];

            // her planı paralel başlat
            foreach (var p in wave.planlar)
                StartCoroutine(HatSpawn(p));

            // dalga bitiş tahmini (en uzun plan)
            float tahmin = 0f;
            foreach (var p in wave.planlar)
                tahmin = Mathf.Max(tahmin, p.ofset + TahminSureTek(p.adet, p.aralik));

            yield return new WaitForSeconds(tahmin + 5f); // tampon
            yield return new WaitForSeconds(wave.waveArasi);
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
        }
    }

    float TahminSureTek(int adet, float aralik) => adet > 0 ? (adet - 1) * aralik : 0f;

    // ---------------- Plan üreticiler ----------------
    SpawnPlan Ke(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Kene, a, i, o);
    SpawnPlan Ka(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Karinca, a, i, o);
    SpawnPlan Si(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Sivri, a, i, o);
    SpawnPlan Or(int yol, int a, float i, float o = 0f) => new SpawnPlan(yol, DusmanTuru.Orumcek, a, i, o);
    Wave W(params SpawnPlan[] p) { var w = new Wave(); w.planlar.AddRange(p); return w; }

    List<Wave> PlanSeviyesi(int level)
    {
        switch (level)
        {
            case 1: return PlanLevel1(); // tek yol (ör: aktifYolSayisi=1)
            case 2: return PlanLevel2(); // 2 yol
            case 3: return PlanLevel3(); // 2 yol, W1'de kene yok
            case 4: return PlanLevel4(); // 2+ yol, karınca+kene+örümcek ağırlık
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
            W( Ka(0,3,3.6f,0.8f), Si(0,3,2.6f,0.8f),  Ka(1,3,3.6f,0.8f), Si(1,3,2.6f,0.8f) ),
            W( Ke(0,5,2.2f,0.8f), Ka(0,4,3.2f,0.8f), Si(0,4,2.4f,0.8f),
               Ke(1,5,2.2f,0.8f), Ka(1,4,3.2f,0.8f), Si(1,4,2.4f,0.8f) ),
            W( Ke(0,8,2.0f,0.6f), Ka(0,5,3.0f,0.6f), Si(0,5,2.2f,0.6f),
               Ke(1,8,2.0f,0.6f), Ka(1,5,3.0f,0.6f), Si(1,5,2.2f,0.6f) ),
            W( Ke(0,10,1.8f,0.6f), Ka(0,6,2.8f,0.6f), Si(0,5,2.0f,0.6f),
               Ke(1,10,1.8f,0.6f), Ka(1,6,2.8f,0.6f), Si(1,5,2.0f,0.6f) ),
            W( Ke(0,12,1.6f,0.5f), Ka(0,8,2.6f,0.5f), Si(0,5,1.8f,0.5f),
               Ke(1,12,1.6f,0.5f), Ka(1,8,2.6f,0.5f), Si(1,5,1.8f,0.5f) ),
            W( Ke(0,16,1.4f,0.4f), Ka(0,10,2.4f,0.4f), Si(0,6,1.6f,0.4f),
               Ke(1,16,1.4f,0.4f), Ka(1,10,2.4f,0.4f), Si(1,6,1.6f,0.4f) ),
            W( Ke(0,18,1.2f,0.4f), Ka(0,12,2.2f,0.4f), Si(0,8,1.4f,0.4f),
               Ke(1,18,1.2f,0.4f), Ka(1,12,2.2f,0.4f), Si(1,8,1.4f,0.4f) ),
            W( Ke(0,20,1.0f,0.3f), Ka(0,14,2.0f,0.3f), Si(0,10,1.2f,0.3f),
               Ke(1,20,1.0f,0.3f), Ka(1,14,2.0f,0.3f), Si(1,10,1.2f,0.3f) ),
        };
    }

    // ----- LEVEL 4 (karınca + kene + örümcek ağırlıklı, karışık; az sivri) -----
    List<Wave> PlanLevel4()
    {
        return new List<Wave>
        {
            // W1 – yumuşak giriş (sivri yok)
             W( Ka(0,14,2.4f,0.4f), Or(0,5,2.4f,0.9f), Si(0,2,2.2f,0.5f),
                Ka(1,14,2.4f,0.8f), Or(1,5,2.4f,1.1f), Si(1,2,2.2f,0.9f) ),


            // W2 – karışım artıyor (az sivri)
            W( Ka(0,7,2.8f,0.4f), Or(0,2,2.8f,0.9f), Si(0,3,2.6f,0.5f),
               Ka(1,7,2.8f,0.8f), Or(1,2,2.8f,1.1f), Si(1,3,2.6f,0.9f) ),

            // W3 – üçlü yük (sivri düşük)
            W( Ka(0,8,2.6f,0.4f), Ke(0,8,1.8f,0.6f), Or(0,3,2.6f,0.8f), Si(0,2,2.4f,0.5f),
               Ka(1,8,2.6f,0.8f), Ke(1,8,1.8f,1.0f), Or(1,3,2.6f,1.0f), Si(1,2,2.4f,0.9f) ),

            // W4 – belirgin zorluk (örümcek 4, sivri sınırlı)
            W( Ka(0,10,2.4f,0.4f), Ke(0,10,1.6f,0.6f), Or(0,4,2.4f,0.9f), Si(0,2,2.2f,0.5f),
               Ka(1,10,2.4f,0.8f), Ke(1,10,1.6f,1.0f), Or(1,4,2.4f,1.1f), Si(1,2,2.2f,0.9f) ),

            // W5 – yüksek tempo (örümcek 5)
            W( Ka(0,12,2.2f,0.4f), Ke(0,12,1.5f,0.6f), Or(0,5,2.2f,0.9f), Si(0,3,2.0f,0.5f),
               Ka(1,12,2.2f,0.8f), Ke(1,12,1.5f,1.0f), Or(1,5,2.2f,1.1f), Si(1,3,2.0f,0.9f) ),

            // W6 – yoğun (örümcek 6)
            W( Ka(0,14,2.0f,0.4f), Ke(0,14,1.4f,0.6f), Or(0,6,2.0f,0.9f), Si(0,4,1.8f,0.5f),
               Ka(1,14,2.0f,0.8f), Ke(1,14,1.4f,1.0f), Or(1,6,2.0f,1.1f), Si(1,4,1.8f,0.9f) ),

            // W7 – tepe (örümcek 7)
            W( Ka(0,16,1.9f,0.4f), Ke(0,16,1.3f,0.6f), Or(0,7,1.9f,0.9f), Si(0,4,1.7f,0.5f),
               Ka(1,16,1.9f,0.8f), Ke(1,16,1.3f,1.0f), Or(1,7,1.9f,1.1f), Si(1,4,1.7f,0.9f) ),

            // W8 – final (örümcek 8, karınca+kene yoğun; sivri sınırlı)
            W( Ka(0,18,1.8f,0.4f), Ke(0,18,1.2f,0.6f), Or(0,8,1.8f,0.9f), Si(0,5,1.6f,0.5f),
               Ka(1,18,1.8f,0.8f), Ke(1,18,1.2f,1.0f), Or(1,8,1.8f,1.1f), Si(1,5,1.6f,0.9f) ),
        };
    }
}
