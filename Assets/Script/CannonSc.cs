using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class CannonSc : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    public float range = 15f;               // Menzil
    [SerializeField] float shotsPerSecond = 2f; // saniyede kaç atış
    public GameObject bulletPrefab;        // Mermi prefabı
    public Transform firePoint;            // Merminin çıkış noktası
    public float DonusHizi = 5f;       // Hedefe dönüş hızı
    [SerializeField] float okHizi = 10f;

    public Transform Rotator;
    public Vector3 AciOfset;

    private float fireTimer = 0f;
    Transform currentTarget;
    Transform targetAimPoint;
    [SerializeField] Vector3 MermiOfSet;
    TowerInfo towerinfo;

    private void Start()
    {
        towerinfo=GetComponent<TowerInfo>();
    }
    void Update()
    {
     FindUzakEnemy();
   //   FindClosestEnemy();

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

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= range && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        currentTarget = closestEnemy;
        targetAimPoint = ResolveAimPoint(currentTarget);
    }
    void FindUzakEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] flyingEnemies = GameObject.FindGameObjectsWithTag("FlyingEnemy");

        Transform best = null;
        float bestScore = float.PositiveInfinity; // en AZ kalan yol
        float rangeSqr = range * range;

        foreach (var e in flyingEnemies)
        {
            if ((e.transform.position - transform.position).sqrMagnitude > rangeSqr) continue;
            var sc = e.GetComponent<DusmanSc>(); if (sc == null) continue;

            float s = sc.IlerlemeSkoru();
            if (s < bestScore) { bestScore = s; best = e.transform; }
        }

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
        if (targetAimPoint == null) return;

        Vector3 hedefPos = targetAimPoint.position;

        Vector3 dir = hedefPos - Rotator.position;
        if (dir.sqrMagnitude < 1e-6f) return;

        Quaternion hedefRot = Quaternion.LookRotation(dir.normalized, Vector3.up) * Quaternion.Euler(AciOfset);
        Rotator.rotation = Quaternion.Slerp(Rotator.rotation, hedefRot, DonusHizi * Time.deltaTime);
    }



    void Fire()
    {
        if (targetAimPoint == null) return;

        fireTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(0.0001f, shotsPerSecond);

        while (fireTimer >= interval)
        {
            fireTimer -= interval;
            KurdanAt();
            GeriTep();
        }

    }




    void KurdanAt()
    {
        Vector3 dir = targetAimPoint.position - firePoint.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion finalRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        finalRot *= Quaternion.Euler(MermiOfSet);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, finalRot);
        if (bullet.TryGetComponent<BulletSc>(out var ab))
        {
         ab.EkstraHasar=towerinfo.EkstraHasar;
        }
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 dir1 = targetAimPoint.position - Rotator.position;
            dir1.Normalize();
            
            rb.AddForce(dir1 * okHizi, ForceMode.VelocityChange);
        }
    }

    #region Sekme

    [Header("Sekme Animasyonu")]
    [SerializeField] Transform sekmeObjesi;      // Geri tepecek parça (child)
    [SerializeField] Vector3 yerelEksen = Vector3.left; // Sekme yönü (sekmeObjesi'ne göre)
    [SerializeField] float sekmeMesafesi = 0.06f;
    [SerializeField] float ileriSure = 0.03f;
    [SerializeField] float donusSure = 0.08f;

    Coroutine cr;

    public void GeriTep()
    {
        if (!sekmeObjesi) return;
        if (cr != null) StopCoroutine(cr);
        cr = StartCoroutine(SekmeCR());
    }

    System.Collections.IEnumerator SekmeCR()
    {
        Vector3 startLocal = sekmeObjesi.localPosition;

        // İtiş
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, ileriSure);
            float a = Mathf.SmoothStep(0f, 1f, t);

            Vector3 parentDir = ParentSpaceDir(sekmeObjesi, yerelEksen); // anlık yön
            sekmeObjesi.localPosition = startLocal + parentDir * (a * sekmeMesafesi);
            yield return null;
        }

        // Geri dönüş
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, donusSure);
            float a = 1f - Mathf.SmoothStep(0f, 1f, t);

            Vector3 parentDir = ParentSpaceDir(sekmeObjesi, yerelEksen);
            sekmeObjesi.localPosition = startLocal + parentDir * (a * sekmeMesafesi);
            yield return null;
        }

        sekmeObjesi.localPosition = startLocal;
        cr = null;
    }

    // sekmeObjesi'nin local yönünü parent uzayına çevir
    static Vector3 ParentSpaceDir(Transform t, Vector3 localDir)
    {
        if (!t) return Vector3.zero;
        if (!t.parent) return localDir.normalized; // parent yoksa yerel=parent aynı
        Vector3 worldDir = t.TransformDirection(localDir.normalized);
        Vector3 parentDir = t.parent.InverseTransformDirection(worldDir);
        return parentDir.normalized;
    }

    #endregion

}
