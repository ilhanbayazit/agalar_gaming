using UnityEngine;

public class Skiller : MonoBehaviour
{
    [SerializeField] LayerMask hedefKatman;
    [SerializeField] float maxMesafe = 500f;

    [Header("İmleçler (UI Image)")]
    [SerializeField] RectTransform imlecKurdan;
    [SerializeField] RectTransform imlecBayrak;

    Camera cam;
    float zeminY = 0f;
    Vector2 sonEkranPos;

    enum AktifSkill { None, Kurdan, Bayrak }
    AktifSkill aktif = AktifSkill.None;

    void Awake() => CamAyarla();

    void CamAyarla()
    {
        if (cam) return;
        cam = Camera.main;
        if (!cam && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
        if (!cam) Debug.LogError("[Skiller] Kamera yok. Bir kamerayı 'MainCamera' etiketleyin veya Inspector’dan atayın.");
    }
 
    public void AKurdanAtBtn()
    {
        aktif = AktifSkill.Kurdan;
        ImlecAcKapat(true);
        ImlecGuncelle(Input.mousePosition);
        
    }

    public void ABayrakAtBtn()
    {
        aktif = AktifSkill.Bayrak;
        ImlecAcKapat(true);
        ImlecGuncelle(Input.mousePosition);
    }

    void Update()
    {
        if (aktif == AktifSkill.None) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            sonEkranPos = Input.mousePosition;
            ImlecGuncelle(sonEkranPos);
        }
        if (Input.GetMouseButton(0))
        {
            sonEkranPos = Input.mousePosition;
            ImlecGuncelle(sonEkranPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            SpawnVeTemizle();
        }
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                sonEkranPos = t.position;
                ImlecGuncelle(sonEkranPos);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                SpawnVeTemizle();
            }
        }
#endif
    }

    void SpawnVeTemizle()
    {
        var nokta = EkranTikToDunya(sonEkranPos);
        if (nokta == Vector3.negativeInfinity) { Temizle(); return; }

        switch (aktif)
        {
            case AktifSkill.Kurdan:
                GetComponent<KurdanYagmuru>()?.Baslat(nokta);
                break;
            case AktifSkill.Bayrak:
                GetComponent<BayrakAtma>()?.Baslat(nokta);
                break;
        }
        Temizle();
    }

    Vector3 EkranTikToDunya(Vector2 ekranPos)
    {
        if (!cam) { CamAyarla(); if (!cam) return Vector3.negativeInfinity; }
        var ray = cam.ScreenPointToRay(ekranPos);

        if (Physics.Raycast(ray, out var hit, maxMesafe, hedefKatman, QueryTriggerInteraction.Collide))
            return hit.point;

        var plane = new Plane(Vector3.up, new Vector3(0, zeminY, 0));
        return plane.Raycast(ray, out float d) ? ray.GetPoint(d) : Vector3.negativeInfinity;
    }

    // --- İmleç yönetimi ---
    void ImlecGuncelle(Vector2 screenPos)
    {
        var aktifRT = AktifRT();
        if (!aktifRT) return;

        var c = aktifRT.GetComponentInParent<Canvas>();
        if (c && c.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)aktifRT.parent, screenPos, c.worldCamera, out var lp);
            aktifRT.anchoredPosition = lp;
        }
        else aktifRT.position = screenPos;
    }

    RectTransform AktifRT()
    {
        return aktif switch
        {
            AktifSkill.Kurdan => imlecKurdan,
            AktifSkill.Bayrak => imlecBayrak,
            _ => null
        };
    }

    void ImlecAcKapat(bool ac)
    {
        if (imlecKurdan) imlecKurdan.gameObject.SetActive(ac && aktif == AktifSkill.Kurdan);
        if (imlecBayrak) imlecBayrak.gameObject.SetActive(ac && aktif == AktifSkill.Bayrak);
    }

    void Temizle()
    {
        aktif = AktifSkill.None;
        ImlecAcKapat(false);
    }
}
