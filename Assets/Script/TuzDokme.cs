using UnityEngine;

public class TuzDokme : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] Transform pourPoint;
    [SerializeField] GameObject tuzPrefab;

    [Header("Zamanlama")]
    [SerializeField] float interval = 2f;   // 2 sn'de bir
    [SerializeField] int taneSayisi = 20;   // Kaç tane atsın

    [Header("Atış Ayarları")]
    [SerializeField] float force;         // Kuvvet
    [SerializeField] float maxSpreadAngle = 8f;  // Koni açısı (derece)

    // Senin modelinde ağız yönü X ekseni (sağa) ise:
    [SerializeField] Vector3 localForwardAxis = new Vector3();

    float timer;

    // --- GÜNCELLENEN ATMA FONKSİYONU ---
    public void AtTuzToplu()
    {
        // Parent hazırla
        if (tuzlarParent == null) tuzlarParent = GetirVeyaOlusturTuzlarParent();

        Vector3 fwd = pourPoint.TransformDirection(localForwardAxis).normalized;

        for (int i = 0; i < taneSayisi; i++)
        {
            Vector3 dir = RandomDirectionInCone(fwd, maxSpreadAngle);

            var go = Instantiate(tuzPrefab, pourPoint.position, Random.rotation);
            go.transform.SetParent(tuzlarParent, true); // world pos'u koru

            if (go.TryGetComponent<Rigidbody>(out var rb))
                rb.AddForce(dir * force, ForceMode.Impulse);
        }
    }

    Transform tuzlarParent;
    Transform GetirVeyaOlusturTuzlarParent()
    {
        var obj = GameObject.Find("Tuzlar");
        if (obj) return obj.transform;

        var yeni = new GameObject("Tuzlar");
        return yeni.transform;
    }

    Vector3 RandomDirectionInCone(Vector3 fwd, float angleDeg)
    {
        float angle = Random.Range(0f, angleDeg);
        float azimuth = Random.Range(0f, 360f);

        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        rot *= Quaternion.AngleAxis(azimuth, fwd);
        return rot * fwd;
    }

    void zamanlaat()
    {
        if (!pourPoint || !tuzPrefab) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            AtTuzToplu();
        }
    }
}
