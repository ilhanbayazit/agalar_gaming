using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KismiGudumForce : MonoBehaviour
{
    public Transform target;
    public float forwardThrust = 10f; // sürekli ileri itiş (Acceleration)
    public float turnTorque = 8f;  // Y (yaw) tork ölçeği
    public bool sadeceYRotasyon = true; // X/Z sabit, sadece Y değişsin

    Rigidbody rb;
    float fixedX, fixedZ;

    public void SetTarget(Transform t) => target = t;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 50f;

        if (sadeceYRotasyon)
        {
            // Başlangıçtaki X/Z açıyı sabitle
            var e = transform.eulerAngles;
            fixedX = e.x; fixedZ = e.z;

            // X ve Z rotasyonlarını fiziksel olarak kilitle
            rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void FixedUpdate()
    {
        if (!target) return;

        // Hedefe yatay düzlemde bak (Y yalnız dursun)
        Vector3 to = target.position - transform.position;
        if (sadeceYRotasyon) to.y = 0f;
        if (to.sqrMagnitude < 1e-6f) return;

        // --- Sadece Y (yaw) torku ile döndür ---
        Vector3 fwdH = new Vector3(transform.forward.x, 0f, transform.forward.z);
        if (fwdH.sqrMagnitude < 1e-6f) fwdH = Vector3.forward;
        fwdH.Normalize();

        Vector3 toH = new Vector3(to.x, 0f, to.z).normalized;
        float yawErrDeg = Vector3.SignedAngle(fwdH, toH, Vector3.up); // (-180..180)

        rb.AddTorque(Vector3.up * yawErrDeg * turnTorque, ForceMode.Acceleration);

        // --- İleri itiş (velocity set etmiyoruz) ---
        rb.AddForce(transform.forward * forwardThrust, ForceMode.Acceleration);

        // Emniyet: X/Z açılar sabit kalsın (rare edge cases)
        if (sadeceYRotasyon)
        {
            var e = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(fixedX, e.y, fixedZ);
        }
    }
}
