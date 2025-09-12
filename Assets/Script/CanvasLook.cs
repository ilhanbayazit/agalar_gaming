using UnityEngine;

public class CanvasLook : MonoBehaviour
{
    Camera cam;
    public Vector3 yOfset;
    void Awake() => BulKamera();

    void LateUpdate()
    {
        if (cam == null) { BulKamera(); if (cam == null) return; }

        Vector3 dir = cam.transform.position - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(-dir, Vector3.up);

        // sadece X ve Y, Z kilit
        Vector3 euler = targetRot.eulerAngles;
        euler.z = 0f;

        // kullanıcı ofsetini ekle
        euler += yOfset;

        transform.rotation = Quaternion.Euler(euler);
    }

    void BulKamera()
    {
        cam = Camera.main;
        if (cam == null)
            cam = FindFirstObjectByType<Camera>(); // Unity 2022+
        // Eski sürümler için: cam = FindObjectOfType<Camera>();
    }
}
