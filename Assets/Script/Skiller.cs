using UnityEngine;

public class Skiller : MonoBehaviour
{
    [SerializeField] LayerMask hedefKatman;
    [SerializeField] float maxMesafe = 500f;

    [Header("İmleç (UI Image)")]
    [SerializeField] RectTransform mobilImlec;

    Camera cam;
    float zeminY = 0f;
    bool KurdanAtsinMi = false;
    Vector2 sonEkranPos;

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
        KurdanAtsinMi = true;
        if (mobilImlec) mobilImlec.gameObject.SetActive(true);
        ImlecGuncelle(Input.mousePosition);
    }

    void Update()
    {
        if (!KurdanAtsinMi) return;

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
            var nokta = EkranTikToDunya(sonEkranPos);
            var ky = GetComponent<KurdanYagmuru>();
            if (ky && nokta != Vector3.negativeInfinity) ky.Baslat(nokta);
            Temizle();
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
                var nokta = EkranTikToDunya(sonEkranPos);
                var ky = GetComponent<KurdanYagmuru>();
                if (ky && nokta != Vector3.negativeInfinity) ky.Baslat(nokta);
                Temizle();
            }
        }
#endif
    }

    Vector3 EkranTikToDunya(Vector2 ekranPos)
    {
        if (!cam) { CamAyarla(); if (!cam) return Vector3.negativeInfinity; }
        var ray = cam.ScreenPointToRay(ekranPos);

        if (Physics.Raycast(ray, out var hit, maxMesafe, hedefKatman, QueryTriggerInteraction.Collide))
            return hit.point;

        var plane = new Plane(Vector3.up, new Vector3(0, zeminY, 0));
        if (plane.Raycast(ray, out float d))
            return ray.GetPoint(d);

        return Vector3.negativeInfinity;
    }

    void ImlecGuncelle(Vector2 screenPos)
    {
        if (!mobilImlec) return;
        var c = mobilImlec.GetComponentInParent<Canvas>();
        if (c && c.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)mobilImlec.parent, screenPos, c.worldCamera, out var lp);
            mobilImlec.anchoredPosition = lp;
        }
        else mobilImlec.position = screenPos;
    }

    void Temizle()
    {
        KurdanAtsinMi = false;
        if (mobilImlec) mobilImlec.gameObject.SetActive(false);
    }
}
