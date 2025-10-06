using UnityEngine;
using UnityEngine.EventSystems;

public class Skiller : MonoBehaviour
{
    [SerializeField] Camera cam;          // Boşsa MainCamera kullanılır
    [SerializeField] LayerMask hedefKatman; // Yalnızca tıklanabilir katman(lar)
    [SerializeField] float maxMesafe = 500f;
    float zeminY = 0f;
    bool KurdanAtsinMi = false;
    void Awake() => CamAyarla();


    void CamAyarla()
    {
        if (cam) return;
        cam = Camera.main;
        if (!cam && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
        if (!cam) Debug.LogError("[TikKonumAl] Sahnede kamera bulunamadı! Kamerayı inspector’dan atayın veya bir kamerayı 'MainCamera' olarak etiketleyin.");
    }

    public void AKurdanAtBtn()
    {
        KurdanAtsinMi = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && KurdanAtsinMi)
        {
            var nokta = EkranTikToDunya();
            gameObject.GetComponent<KurdanYagmuru>().Baslat(nokta);
            KurdanAtsinMi = false;
        }

    }

    Vector3 EkranTikToDunya()
    {
        if (!cam) { CamAyarla(); if (!cam) return Vector3.negativeInfinity; }
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, maxMesafe, hedefKatman, QueryTriggerInteraction.Collide))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 1f);
            // Debug.Log($"Hit: {hit.collider.name}");
            return hit.point;
        }
        // Yedek: düz zemin (collider yoksa)
        var plane = new Plane(Vector3.up, new Vector3(0, zeminY, 0));
        if (plane.Raycast(ray, out float d))
        {
            Debug.DrawRay(ray.origin, ray.direction * d, Color.yellow, 1f);
            return ray.GetPoint(d);
        }
        return Vector3.negativeInfinity; // bulunamadı
    }
}
