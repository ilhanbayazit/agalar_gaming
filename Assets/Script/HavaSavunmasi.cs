using UnityEngine;

public class HavaSavunmasi : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    [SerializeField] float range = 10f;               // Menzil
    [SerializeField] float shotsPerSecond; // saniyede kaç atış
    [SerializeField] GameObject bulletPrefab;        // Mermi prefabı
    [SerializeField] Transform firePoint;            // Merminin çıkış noktası
    [SerializeField] float DonusHizi = 5f;
    [SerializeField] float AtesHizi = 18f;       // Hedefe dönüş hızı
    [SerializeField] Vector3 AciOfset;
    TowerInfo towerinfo;

    private float fireTimer = 0f;
    private Transform currentTarget;
    Transform targetAimPoint;

    [Header("Mermi Ayarlari")]
    [SerializeField] Vector3 MermiOfSet;
    private void Start()
    {
        towerinfo = gameObject.GetComponent<TowerInfo>();
    }
    void Update()
    {
        //    FindClosestEnemy();
        FindUzakEnemy();
        if (currentTarget != null)
        {
            LookAtTarget();
            Fire();
        }
    }

    void FindUzakEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("FlyingEnemy");

        Transform best = null;
        float bestScore = float.PositiveInfinity; // en AZ kalan yol
        float rangeSqr = range * range;

        foreach (var e in enemies)
        {
            if ((e.transform.position - transform.position).sqrMagnitude > rangeSqr) continue;
            var sc = e.GetComponent<DusmanSc>(); if (sc == null) continue;
            float s = sc.IlerlemeSkoru();
            if (s < bestScore) { bestScore = s; best = e.transform; }
        }
        currentTarget = best;
        targetAimPoint = ResolveAimPoint(currentTarget);
    }
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("FlyingEnemy");
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

    [SerializeField] Transform RotatorY;   // yaw
    [SerializeField] Transform RotatorX;   // pitch (RotatorY child)

    void LookAtTarget()
    {
        if (!targetAimPoint || !RotatorY || !RotatorX) return;

        Vector3 d = targetAimPoint.position - RotatorY.position;
        if (d.sqrMagnitude < 1e-6f) return;

        // Y ekseni (yaw) — sadece Y döndür
        float yawDeg = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg + AciOfset.y;
        Quaternion hedefYaw = Quaternion.Euler(0f, yawDeg, 0f);
        RotatorY.rotation = Quaternion.Slerp(RotatorY.rotation, hedefYaw, DonusHizi * Time.deltaTime);

        // X ekseni (pitch) — sadece X döndür (RotatorX, RotatorY'nin child'ı olmalı)
        Vector3 ld = RotatorY.InverseTransformDirection(d); // yerel ileri/üst eksenlere göre
        float pitchDeg = -Mathf.Atan2(ld.y, ld.z) * Mathf.Rad2Deg + AciOfset.x;
        Quaternion hedefPitch = Quaternion.Euler(pitchDeg, 0f, 0f);
        RotatorX.localRotation = Quaternion.Slerp(RotatorX.localRotation, hedefPitch, DonusHizi * Time.deltaTime);
    }
    void Fire()
    {
        if (targetAimPoint == null) return;

        fireTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(0.0001f, shotsPerSecond);

        while (fireTimer >= interval)
        {
            fireTimer -= interval;
            MisirAt();
        }

    }
    void MisirAt()
    {
        Vector3 dir = targetAimPoint.position - firePoint.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion finalRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        finalRot *= Quaternion.Euler(MermiOfSet);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, finalRot);
        if (bullet.TryGetComponent<BulletSc>(out var ab))
        {
            ab.EkstraHasar = towerinfo.EkstraHasar;
        }
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 dir1 = targetAimPoint.position - firePoint.position;
            dir1.Normalize();
            rb.AddForce(dir1 * AtesHizi, ForceMode.VelocityChange);
        }
    }

}
