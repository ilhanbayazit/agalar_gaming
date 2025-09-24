using UnityEditor;
using UnityEngine;

public class FindikAtar : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    [SerializeField] float range = 10f;               // Menzil
    [SerializeField] float shotsPerSecond; // saniyede kaç atış
    [SerializeField] GameObject bulletPrefab;        // Mermi prefabı
    [SerializeField] Transform firePoint;            // Merminin çıkış noktası
    [SerializeField] float DonusHizi = 5f;       // Hedefe dönüş hızı
    [SerializeField] Transform Rotator;
    [SerializeField] Vector3 AciOfset;

    private float fireTimer = 0f;
    private Transform currentTarget;

    [Header("Mermi Ayarlari")]
    [SerializeField] Vector3 MermiOfSet;

    void Update()
    {
        FindClosestEnemy();
        if (currentTarget != null)
        {
            LookAtTarget();
            Fire();
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Eğer düşman menzildeyse
            if (distance <= range && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        currentTarget = closestEnemy;
        // Menzildeki en yakın düşmanı bulma
    }


    void LookAtTarget()    // Topun hedefe dönmesini sağlar
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - Rotator.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion hedefRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        hedefRot *= Quaternion.Euler(AciOfset);
        Rotator.rotation = Quaternion.Slerp(Rotator.rotation, hedefRot, DonusHizi * Time.deltaTime);
    }


    void Fire()
    {
        if (currentTarget == null) return;

        fireTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(0.0001f, shotsPerSecond);

        while (fireTimer >= interval)
        {
            fireTimer -= interval;
            ZeytinAt();
        }

    }
    // Ayarlar
    [SerializeField] float AtesAciDeg = 45f;   // Senin belirlediğin yükseklik açısı (derece)
    [SerializeField] float MinHiz = 1f;        // Güvenlik / alt sınır
    [SerializeField] float MaxHiz = 120f;      // Güvenlik / üst sınır
    [SerializeField] float GucCarpani = 1f;    // İstersen genel güç çarpanı

    void ZeytinAt()
    {
        if (currentTarget == null) return;

        Vector3 p0 = firePoint.position;
        Vector3 p1 = currentTarget.position;

        // XZ düzleminde yatay yön
        Vector3 to = p1 - p0;
        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float R = toXZ.magnitude;                 // yatay mesafe
        if (R < 0.001f) return;

        float dy = p1.y - p0.y;                   // yükseklik farkı
        float g = Mathf.Abs(Physics.gravity.y);

        float theta = Mathf.Deg2Rad * AtesAciDeg; // atan açı (radyan)
        float cosT = Mathf.Cos(theta);
        float sinT = Mathf.Sin(theta);
        float tanT = Mathf.Tan(theta);

        // v^2 = g R^2 / (2 cos^2θ (R tanθ - Δy))
        float denom = (R * tanT - dy);
        if (denom < 0.001f) denom = 0.001f;       // çökmeyi engelle

        float v2 = (g * R * R) / (2f * cosT * cosT * denom);
        if (v2 <= 0f) return;

        float v = Mathf.Sqrt(v2);
        v = Mathf.Clamp(v * GucCarpani, MinHiz, MaxHiz);

        // İlk hız vektörü (yatayda hedefe, dikeyde açıya göre)
        Vector3 dirXZ = toXZ.normalized;
        Vector3 v0 = dirXZ * (v * cosT) + Vector3.up * (v * sinT);

        // İsteğe bağlı: görsel yönelim
        Quaternion rot = Quaternion.LookRotation(v0.normalized, Vector3.up);
        rot *= Quaternion.Euler(MermiOfSet); // kendi ofsetini koru

        GameObject bullet = Instantiate(bulletPrefab, p0, rot);

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            // İtki = m * v0  (Impulse ⇒ anlık hız kazandırır, kütleye göre gerçekçi)
            rb.AddForce(v0 * rb.mass, ForceMode.Impulse);
        }
    }
    void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }

}
