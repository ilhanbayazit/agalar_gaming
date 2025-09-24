using UnityEditor;
using UnityEngine;

public class CekirdekAtar : MonoBehaviour
{

    [Header("Cannon Ayarları")]
    public float range = 15f;               // Menzil
    [SerializeField] float shotsPerSecond = 2f; // saniyede kaç atış
    public GameObject bulletPrefab;        // Mermi prefabı
    [SerializeField] Transform[] firePoints; // FirePoint1, FirePoint2, FirePoint3


    public float DonusHizi = 5f;       // Hedefe dönüş hızı
    [SerializeField] float CekirdekHizi = 10f;

    [SerializeField] Transform Rotator;
    [SerializeField] Transform Namlu;
    [SerializeField] float NamluDonusHizi;       // Hedefe dönüş hızı


    public Vector3 AciOfset;

    private float fireTimer = 0f;
    private Transform currentTarget;
    [SerializeField] Vector3 MermiOfSet;

    void Update()
    {
        FindUzakEnemy();

        if (currentTarget != null)
        {
            LookAtTarget();
            Fire();
        }

    }

    void FindUzakEnemy()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] flyingEnemies = GameObject.FindGameObjectsWithTag("FlyingEnemy");

        Transform best = null;
        float bestScore = float.NegativeInfinity;
        float rangeSqr = range * range;

        foreach (var e in flyingEnemies)
        {
            // opsiyonel menzil
            if ((e.transform.position - transform.position).sqrMagnitude > rangeSqr) continue;

            var sc = e.GetComponent<DusmanSc>();
            if (sc == null) continue;

            float s = sc.IlerlemeSkoru();
            if (s > bestScore)
            {
                bestScore = s;
                best = e.transform;
            }
        }

        foreach (var e in enemies)
        {
            // opsiyonel menzil
            if ((e.transform.position - transform.position).sqrMagnitude > rangeSqr) continue;

            var sc = e.GetComponent<DusmanSc>();
            if (sc == null) continue;

            float s = sc.IlerlemeSkoru();
            if (s > bestScore)
            {
                bestScore = s;
                best = e.transform;
            }


            currentTarget = best;
            // Menzildeki en yakın düşmanı bulma
        }

        currentTarget = best;
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
        NamluDondur();
        while (fireTimer >= interval)
        {
            fireTimer -= interval;
            CekirdekAt();
        }
    }

    int fireIndex = 0;
    void CekirdekAt()
    {
        if (firePoints == null || firePoints.Length == 0 || currentTarget == null) return;

        Transform fp = null;
        int n = firePoints.Length;
        for (int i = 0; i < n; i++)
        {
            var cand = firePoints[fireIndex];
            fireIndex = (fireIndex + 1) % n;
            if (cand != null) { fp = cand; break; }
        }
        if (!fp) return;

        Vector3 dir = currentTarget.position - fp.position;
        if (dir.sqrMagnitude < 1e-6f) return;
        dir.Normalize();

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(MermiOfSet);

        GameObject bullet = Instantiate(bulletPrefab, fp.position, rot);
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
            rb.AddForce(dir * CekirdekHizi, ForceMode.VelocityChange);
    }



    void NamluDondur()
    {
        if (!Namlu) return;
        Namlu.Rotate(Vector3.right, NamluDonusHizi * Time.deltaTime, Space.Self);
    }


    void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }

}
