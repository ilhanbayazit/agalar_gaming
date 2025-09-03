using UnityEngine;

public class CannonSc : MonoBehaviour
{
    [Header("Cannon Ayarları")]
    public float range = 15f;               // Menzil
    public float fireRate = 1f;            // Ateş etme aralığı (saniye)
    public GameObject bulletPrefab;        // Mermi prefabı
    public Transform firePoint;            // Merminin çıkış noktası
    public float rotationSpeed = 5f;       // Hedefe dönüş hızı

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
    void LookAtTarget()
    {
        // Hedefe olan yönü hesapla
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f; // Y eksenini sabitliyoruz

        if (direction != Vector3.zero)
        {
            // Hedefe bak
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Y eksenine +180 derece ekle
            targetRotation *= Quaternion.Euler(0f, 180f, 0f);

            // Yumuşak dönüş
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    // Ateş etme fonksiyonu
    void Fire()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Eğer mermide Rigidbody varsa ileri doğru kuvvet uygula
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-firePoint.forward * 20f, ForceMode.Impulse);
            }

        }
    }

 
   


}
