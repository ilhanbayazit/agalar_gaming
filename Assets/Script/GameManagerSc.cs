using System.Collections.Generic;
using UnityEngine;

public class GameManagerSc : MonoBehaviour
{

    [SerializeField] GameObject Karinca;
    [SerializeField] GameObject Kene;
    [SerializeField] GameObject Sivri;
    [SerializeField] GameObject Orumcek;
    [SerializeField] GameObject HamamBocegi;
    [SerializeField] GameObject Ari;
    [SerializeField] GameObject Yusufcuk;
    [SerializeField] GameObject BokBocegi;


    Transform DusmanlarParent;


    void Awake()
    {
        foreach (var r in Routes)
            SeritleriOlustur(r);
    }


    private void Start()
    {
        DusmanlarParent = GetirVeyaOlusturDusmanlarParent();
    }
    private void Update()
    {
        //     DusmanSpawnHizlandirma();
        dusmanspawnkontrol();
        OyunHizlandirma();
        
    }
    Transform GetirVeyaOlusturDusmanlarParent()
    {
        var obj = GameObject.Find("Dusmanlar");
        if (obj) return obj.transform;

        var yeni = new GameObject("Dusmanlar");
        return yeni.transform;
    }

    #region Spawn
    public void KarincaSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(Karinca, r.SpawnPoint.position, Karinca.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, false));
    }

    public void KeneSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(Kene, r.SpawnPoint.position, Kene.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, false));
    }

    public void OrumcekSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(Orumcek, r.SpawnPoint.position, Orumcek.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, false));
    }

    public void SivriSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(Sivri, r.SpawnPoint.position, Sivri.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, true));
    }
    public void HamamBocegiSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(HamamBocegi, r.SpawnPoint.position, HamamBocegi.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, false));
    }
    public void YusufcukSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(Yusufcuk, r.SpawnPoint.position, Yusufcuk.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, true));
    }

    public void AriSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(Ari, r.SpawnPoint.position, Ari.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, true));
    }
    public void BokBocegiSpawn(int routeIndex)
    {
        var r = Routes[routeIndex];
        int serit = RastgeleSeritNotRepeat(r);
        var go = Instantiate(BokBocegi, r.SpawnPoint.position, BokBocegi.transform.localRotation, DusmanlarParent);
        var d = go.GetComponent<DusmanSc>();
        d.WaypointleriAyarla(SeciliLane(r, serit, false));
    }

    #endregion



    #region YolOlusturma
    [Header("Merkez Yol")]
    public List<RouteConfig> Routes = new List<RouteConfig>(); // Inspector'da 2 yol ekle
    [SerializeField] float seritGenisligi = 1.6f; // şerit aralığı
    [SerializeField] int seritSayisi = 3;      // >= 1, Inspector’dan ayarlanır
    int RastgeleSeritNotRepeat(RouteConfig r)
    {
        int n = Mathf.Max(1, seritSayisi);

        if (n == 1)
        {
            r.lastLane = 0;
            return 0;
        }

        if (r.lastLane < 0)
        {
            r.lastLane = Random.Range(0, n);
            return r.lastLane;
        }

        int pick;
        do { pick = Random.Range(0, n); } while (pick == r.lastLane);

        r.lastLane = pick;
        return pick;
    }

    void SeritleriOlustur(RouteConfig r)
    {
        if (r.lanesRoot == null)
        {
            r.lanesRoot = new GameObject($"{r.Ad}_LanesRoot").transform;
            r.lanesRoot.SetPositionAndRotation(r.MerkezWaypointParent.position, r.MerkezWaypointParent.rotation);
        }

        var merkez = CocuklariListele(r.MerkezWaypointParent);

        r.lanes.Clear();
        r.lanesFly.Clear();

        // Şerit indeksini merkeze göre simetrik dağıt:
        // i=0..N-1 için ofsetKatsayi = (i - (N-1)/2f)
        for (int i = 0; i < seritSayisi; i++)
        {
            float ofsetKatsayi = i - (seritSayisi - 1) * 0.5f;
            float lateral = ofsetKatsayi * seritGenisligi;

            // Yer şeridi
            var lane = KlonlaVeKopyala(merkez, $"{r.Ad}_Lane_{i}", lateral, 0f, r.lanesRoot);
            r.lanes.Add(lane);

            // Uçan şeridi
            var laneFly = KlonlaVeKopyala(merkez, $"{r.Ad}_LaneFly_{i}", lateral, r.flyHeight, r.lanesRoot);
            r.lanesFly.Add(laneFly);
        }
    }

    List<Transform> SeciliLane(RouteConfig r, int idx, bool isFly)
    {
        if (r.lanes.Count == 0) return null;

        idx = Mathf.Clamp(idx, 0, r.lanes.Count - 1);
        return isFly ? r.lanesFly[idx] : r.lanes[idx];
    }

    List<Transform> CocuklariListele(Transform parent)
    {
        var list = new List<Transform>(parent.childCount);
        for (int i = 0; i < parent.childCount; i++)
            list.Add(parent.GetChild(i));
        return list;
    }
    List<Transform> KlonlaVeKopyala(List<Transform> merkez, string ad, float lateralOffset, float yOffset, Transform root)
    {
        var holder = new GameObject(ad).transform;
        holder.SetParent(root, false);

        var sonuc = new List<Transform>(merkez.Count);
        for (int i = 0; i < merkez.Count; i++)
        {
            Vector3 pos = merkez[i].position;
            Vector3 sag = HesaplaSagVektor(merkez, i);
            if (Mathf.Abs(lateralOffset) > 0f) pos += sag * lateralOffset;
            if (Mathf.Abs(yOffset) > 0f) pos += Vector3.up * yOffset;

            var empty = new GameObject($"{ad}_WP_{i}").transform;
            empty.position = pos;
            empty.rotation = merkez[i].rotation;
            empty.SetParent(holder, true);

            sonuc.Add(empty);
        }
        return sonuc;
    }

    Vector3 HesaplaSagVektor(List<Transform> merkez, int i)
    {
        Vector3 ileri;
        if (i == 0) ileri = (merkez[1].position - merkez[0].position);
        else if (i == merkez.Count - 1) ileri = (merkez[i].position - merkez[i - 1].position);
        else
        {
            var prev = (merkez[i].position - merkez[i - 1].position).normalized;
            var next = (merkez[i + 1].position - merkez[i].position).normalized;
            ileri = (prev + next);
        }
        ileri = (ileri.sqrMagnitude > 0.0001f) ? ileri.normalized : Vector3.forward;
        return Vector3.Cross(Vector3.up, ileri).normalized;
    }
    #endregion



    void dusmanspawnkontrol()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            KarincaSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            KeneSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SivriSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OrumcekSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            HamamBocegiSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            BokBocegiSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            AriSpawn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            YusufcukSpawn(0);
        }
    }
    void DusmanSpawn()
    {
        int sayi = Random.Range(1, 5);
        switch (sayi)
        {
            case 1:
                KarincaSpawn(1);
                return;
            case 2:
                KeneSpawn(0);
                return;
            case 3:
                SivriSpawn(1);
                return;
            case 4:
                OrumcekSpawn(1);
                return;
            case 5:
                HamamBocegiSpawn(1);
                break;
            default:
                return;
        }

    }
    void OyunHizlandirma()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale = 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (Time.timeScale < 0.6) return;
            Time.timeScale -= 0.5f;
        }
    }


}
