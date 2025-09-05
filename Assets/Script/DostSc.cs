using System.Collections.Generic;
using UnityEditor.Rendering;
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
       
        int nearestIndex = FindNearestWaypoint();
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

    int FindNearestWaypoint()
    {
        float minDistance = Mathf.Infinity;
        int nearestIndex = 0;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    void OlusturTersListe(int nearestIndex)
    {
        tersListe.Clear(); // Önce temizle

        for (int i = nearestIndex; i >= 0; i--)
            tersListe.Add(waypoints[i]);
    }


    void TersListeYazdir()
    {

        // DEBUG: Ters listeyi konsola yaz
        Debug.Log("Ters Liste:");
        for (int i = 0; i < tersListe.Count; i++)
            Debug.Log($"Index {i} -> {tersListe[i].name}");

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


 
}
