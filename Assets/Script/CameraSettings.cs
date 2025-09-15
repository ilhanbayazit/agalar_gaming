using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class SabitTDKamera : MonoBehaviour
{
    [Header("Harita Orta Noktası")]
    public Vector3 center = new Vector3(0f, 92f, -65f); // senin merkez

    [Header("Mod")]
    public bool usePerspective = true;  // false => Ortho

    [Header("Perspective (önerilen sabit açı)")]
    [Range(20f, 80f)] public float tiltDeg = 50f; // aşağı eğim (X)
    [Range(0f, 360f)] public float yawDeg = 45f; // Y etrafında
    [Range(30f, 90f)] public float fov = 50f;
    [Tooltip("Merkezden uzaklık (gerekirse +-10 oyna)")]
    public float distance = 180f;                // 130x130 için güvenli başlangıç

    [Header("Orthographic (tam güvenli görünüm)")]
    public float orthoSize = 96f; // 65*√2 ≈ 92 ⇒ +margin
    public float orthoDistance = 120f;

    Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        Kur();
    }

    void OnValidate() { if (!cam) cam = GetComponent<Camera>(); Kur(); }

    void Kur()
    {
        if (!cam) return;

        if (usePerspective)
        {
            cam.orthographic = false;
            cam.fieldOfView = fov;

            // Rotasyon ve konum: merkeze bak
            var rot = Quaternion.Euler(tiltDeg, yawDeg, 0f);
            transform.rotation = rot;
            transform.position = center - transform.forward * distance;

            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 600f;
        }
        else
        {
            cam.orthographic = true;
            cam.orthographicSize = orthoSize;

            // Aynı açı, sabit mesafe
            var rot = Quaternion.Euler(tiltDeg, yawDeg, 0f);
            transform.rotation = rot;
            transform.position = center - transform.forward * orthoDistance;

            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 600f;
        }
    }
}
