using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform Hedef;
    public Transform Rotator;   // dönen kafa (Scale=1,1,1)
    public float DonusHizi = 10f;
    public Vector3 AciOfset = new Vector3(0f, 90f, 0f);  // modelin Y=90 düzeltmesi için genelde -90 ya da +90
   
    void Update()
    {
        HedefeBak();
    }

    void HedefeBak()
    {
        if (!Hedef) return;

        Vector3 dir = Hedef.position - Rotator.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion hedefRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        hedefRot *= Quaternion.Euler(AciOfset);
        Rotator.rotation = Quaternion.Slerp(Rotator.rotation, hedefRot, DonusHizi * Time.deltaTime);
    }

}
