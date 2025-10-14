using UnityEditor;
using UnityEngine;

public class TuzAtarSc : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    [SerializeField] private float range;               // Menzil
    [SerializeField] float shotsPerSecond = 2f; // saniyede kaç atış
    [SerializeField] private Transform firePoint;            // Merminin çıkış noktası
    [SerializeField] private float DonusHizi = 5f;       // Hedefe dönüş hızı

    public Transform Rotator;
    public Vector3 AciOfset;

    private float fireTimer = 0f;
    private Transform currentTarget;
    Transform targetAimPoint;


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
            var x = GetComponent<TuzDokme>();
            if (x != null)
            {
                GeriTep();
                x.AtTuzToplu();
            }
        }
    }

 
    #region Sekme
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
