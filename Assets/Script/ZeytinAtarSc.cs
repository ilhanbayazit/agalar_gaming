using UnityEditor;
using UnityEngine;

public class ZeytinAtarSc : MonoBehaviour
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
    Transform targetAimPoint;

    [Header("Mermi Ayarlari")]
    [SerializeField] Vector3 MermiOfSet;
    [SerializeField] float ZeytinHizi = 10f;

    // Update is called once per frame
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
        targetAimPoint = ResolveAimPoint(currentTarget);
    }

    Transform ResolveAimPoint(Transform t)
    {
        if (t == null) return null;

        // 1) DusmanSc içinde public Transform AimPoint varsa kullan
        var ds = t.GetComponent<DusmanSc>();
        if (ds != null)
        {
            var f = ds.GetType().GetField("AimPoint");
            if (f != null)
            {
                var val = f.GetValue(ds) as Transform;
                if (val != null) return val;
            }
        }

        // 2) Çocuklardan adı "AimPoint" olanı ara
        foreach (var tr in t.GetComponentsInChildren<Transform>(true))
            if (tr.name == "AimPoint")
                return tr;

        // 3) Bulunamadı -> null (LookAtTarget fallback kullanacak)
        return null;
    }

    void LookAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 hedefPos = targetAimPoint.position;

        Vector3 dir = hedefPos - Rotator.position;
        if (dir.sqrMagnitude < 1e-6f) return;

        Quaternion hedefRot = Quaternion.LookRotation(dir.normalized, Vector3.up) * Quaternion.Euler(AciOfset);
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
    void ZeytinAt()
    {
        Vector3 dir = currentTarget.position - firePoint.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion finalRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        finalRot *= Quaternion.Euler(MermiOfSet);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, finalRot);

        // Rigidbody üzerinden hedefe doğru kuvvet uygula
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 dir1 = currentTarget.position - Rotator.position;
            dir1.Normalize();

            // Hedefe doğru kuvvet uygula
            rb.AddForce(dir1 * ZeytinHizi, ForceMode.VelocityChange);

        }
    }



    void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }
}
