using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

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
    private Transform currentTarget;
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
        GameObject[] flyingEnemies = GameObject.FindGameObjectsWithTag("FlyingEnemy");

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= range && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        foreach (GameObject enemy in flyingEnemies) // ikinci listen
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
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
            KurdanAt();
            GeriTep();
        }

    }



    void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }

    void KurdanAt()
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
            rb.AddForce(dir1 * okHizi, ForceMode.VelocityChange);

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
