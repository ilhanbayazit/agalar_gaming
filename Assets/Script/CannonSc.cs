using UnityEngine;

public class CannonSc : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    public float range = 15f;               // Menzil
    public float fireRate = 1f;            // Ateş etme aralığı (saniye)
    public GameObject bulletPrefab;        // Mermi prefabı
    public Transform firePoint;            // Merminin çıkış noktası
    public float rotationSpeed = 5f;       // Hedefe dönüş hızı
    [SerializeField] float okHizi = 10f;

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


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    // Menzildeki en yakın düşmanı bulma
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
    }

    // Topun hedefe dönmesini sağlar
    Quaternion targetRotation;
    void LookAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 hedefPos = currentTarget.position;
        hedefPos.y = transform.position.y;

        Quaternion hedefRot = Quaternion.LookRotation(hedefPos - transform.position);
        hedefRot *= Quaternion.Euler(0, 90f, 0);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            hedefRot,
            rotationSpeed * Time.deltaTime
        );
    }



    void Fire()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer < fireRate || currentTarget == null) return; // ✅ Hedef yoksa ateş etme
        fireTimer = 0f;

        Vector3 hedefPos = currentTarget.position;
        Vector3 bar = hedefPos - bulletPrefab.transform.position;
        float hedefZ = Mathf.Atan2(bar.y, bar.x) * Mathf.Rad2Deg;
        float turretY = transform.eulerAngles.y;

        Quaternion finalRot = Quaternion.Euler(0, turretY, -hedefZ+90);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, finalRot);

        // Rigidbody üzerinden hedefe doğru kuvvet uygula
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            // Hedefe doğru yön vektörü
            Vector3 dir = (currentTarget.position - firePoint.position).normalized;

            // Hedefe doğru kuvvet uygula
            rb.AddForce(dir * okHizi, ForceMode.VelocityChange);
        }
    }








}
