using UnityEngine;

public class TowerInfo : MonoBehaviour
{
    public int buildCost = 100;              // Bu seviyeyi kurma/yükseltme maliyeti
    public int SatisFiyati = 70;
    public GameObject nextLevelPrefab;       // Sonraki seviye yoksa null
    public int EkstraHasar = 0;
}
