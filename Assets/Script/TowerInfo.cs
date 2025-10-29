using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    public string Id;
    public string Ad;
    public Sprite Icon;          // grid'de küçük görsel
    public Sprite BuyukGorsel;   // sağdaki ana görsel
    public int Hasar;
    public float SaldiriHizi;
    public int buildCost ;              // Bu seviyeyi kurma/yükseltme maliyeti
    public int SatisFiyati ;
    public GameObject nextLevelPrefab;       // Sonraki seviye yoksa null
    public int EkstraHasar = 0;
    public int FindikParcalariEkstraHasar = 0;
    public int menzil;
    public int level;

    private void Start()
    {
        Hasar += EkstraHasar;
    }

}
