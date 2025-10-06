using UnityEngine;

public class BokSc : MonoBehaviour
{

    [SerializeField] Transform hedef;   
    [SerializeField] float hiz = 90f;   
    void Update()
    {
        var t = hedef ? hedef : transform;
        t.Rotate(hiz * Time.deltaTime, 0f, 0f, Space.Self);
    }

}
