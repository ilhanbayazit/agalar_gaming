using System.Collections.Generic;
using UnityEngine;

public class DostSc : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] string waypointParentName = "WaypointParent";
    [SerializeField] float speed = 3f;            // Hız
    [SerializeField] float arriveDistance = 0.5f; // Hedefe yaklaşma mesafesi

    private List<Transform> waypoints = new List<Transform>();
    private List<Transform> tersListe = new List<Transform>();
    private int currentIndex = 0;
    private Transform hedef;

    void Start()
    {
        Waypointbul();
        int nearestIndex = FindNearestWaypoint(); // <-- DEĞİŞTİ
        OlusturTersListe(nearestIndex);

        // İlk hedef
        currentIndex = 0;
        hedef = tersListe[currentIndex];
    }

    void Update()
    {
        if (tersListe.Count == 0) return;
        HedefeGit(hedef.position);
        MesafeKontrolu();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

        }
    }

    void HedefeGit(Vector3 hedefKonumu) // 🔹 Hedefe doğru hareket eden fonksiyon
    {
        transform.position = Vector3.MoveTowards(transform.position, hedefKonumu, speed * Time.deltaTime);
    }

    void MesafeKontrolu() // 🔹 Mesafe kontrol fonksiyonu
    {
        float mesafe = Vector3.Distance(transform.position, hedef.position);

        // Eğer hedefe ulaştıysak
        if (mesafe <= arriveDistance)
        {
            currentIndex++;

            // Eğer son hedefe ulaştıysak düşmanı yok et
            if (currentIndex >= tersListe.Count)
            {
                Destroy(gameObject);
                return;
            }

            hedef = tersListe[currentIndex];
        }
    }

    void OlusturTersListe(int nearestIndex)
    {
        tersListe.Clear(); // Once temizle

        for (int i = nearestIndex; i >= 0; i--)
            tersListe.Add(waypoints[i]);
    }

    void Waypointbul()   // WaypointParent'ı bul ve waypointleri al
    {

        GameObject parent = GameObject.Find(waypointParentName);
        if (parent == null)
        {
            Debug.LogError($"'{waypointParentName}' bulunamadı!");
            enabled = false;
            return;
        }

        for (int i = 0; i < parent.transform.childCount; i++)
            waypoints.Add(parent.transform.GetChild(i));

        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoint bulunamadı!");
            enabled = false;
            return;
        }
    }

    int FindNearestWaypoint() // <-- YENİ
    {
        if (waypoints.Count == 1) return 0;

        float best = float.PositiveInfinity;
        int bestSegLower = 0;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 a = waypoints[i].position;
            Vector3 b = waypoints[i + 1].position;
            Vector3 p = ClosestPointOnSegment(transform.position, a, b);
            float d2 = (transform.position - p).sqrMagnitude;
            if (d2 < best)
            {
                best = d2;
                bestSegLower = i; // segmentin alt indexi
            }
        }

        return bestSegLower;
    }

    Vector3 ClosestPointOnSegment(Vector3 p, Vector3 a, Vector3 b) // <-- YENİ
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }




}
