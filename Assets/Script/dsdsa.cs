using UnityEngine;

public class dsdsa : MonoBehaviour
{
    public Transform a;
    public Transform b;


    // Update is called once per frame
    void Update()
    {
        // İki obje arasındaki yönü hesapla
        Vector3 dir = b.position - a.position;

        // Yalnızca Z açısını alıyoruz
        float hedefZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // A objesinin Z rotasyonunu hedefe ayarla
        a.rotation = Quaternion.Euler(a.eulerAngles.x, a.eulerAngles.y, hedefZ);
    }

}
