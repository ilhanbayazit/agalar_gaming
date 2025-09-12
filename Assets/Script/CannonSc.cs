using UnityEngine;
using UnityEngine.UIElements;

public class CannonSc : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    public float range = 15f;               // Menzil
    public float fireRate = 1f;            // Ateş etme aralığı (saniye)
    public GameObject bulletPrefab;        // Mermi prefabı
    public Transform firePoint;            // Merminin çıkış noktası
    public float DonusHizi = 5f;       // Hedefe dönüş hızı
    [SerializeField] float okHizi = 10f;

    public Transform Rotator;
    public Vector3 AciOfset;

    private float fireTimer = 0f;
    private Transform currentTarget;

       

 

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


    [SerializeField] Vector3 MermiOfSet;
    void Fire()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer < fireRate || currentTarget == null) return; // ✅ Hedef yoksa ateş etme
        fireTimer = 0f;

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








}
