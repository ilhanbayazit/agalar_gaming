using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    public int buildCost ;              // Bu seviyeyi kurma/y�kseltme maliyeti
    public int SatisFiyati ;
    public GameObject nextLevelPrefab;       // Sonraki seviye yoksa null
    public int EkstraHasar = 0;
    public int FindikParcalariEkstraHasar = 0;
    public int menzil;
    public int level;
}
