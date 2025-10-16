using UnityEngine;

public class BayrakAtma : MonoBehaviour
{
    [SerializeField] GameObject Bayrak;
    [SerializeField] float yOfset = 0f;         // Gerekirse zeminden hafif yukarı

    public void Baslat(Vector3 x)
    {
        if (!Bayrak) return;
        var pos = x; pos.y += yOfset;

        var go = Instantiate(Bayrak, pos, Quaternion.identity);
        Destroy(go, 10f); // 10 sn sonra sil
    }


}
