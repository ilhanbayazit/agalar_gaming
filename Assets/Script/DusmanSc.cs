using System.Collections.Generic;
using UnityEngine;

public class DusmanSc : MonoBehaviour
{
    GameObject WayPointsParent;
    List<Transform> WayPoints = new List<Transform>();
    int currentIndex = 0;                // Şu anki hedef waypoint
    public float speed = 3f;             // Hız
    public float arriveDistance = 0.5f;  // Hedefe yaklaşma mesafesi

    private void Start()
    {
        WayPointsParent = GameObject.Find("WaypointParent");
        for (int i = 0; i < WayPointsParent.transform.childCount; i++)
        {
            WayPoints.Add(WayPointsParent.transform.GetChild(i));
        }
    }

    private void Update()
    {
        HedefeGit();
    }

    private void HedefeGit()
    {
        if (WayPoints.Count == 0) return;

        Vector3 hedef = WayPoints[currentIndex].position;

        transform.position = Vector3.MoveTowards(transform.position, hedef, speed * Time.deltaTime);

        float mesafe = Vector3.Distance(transform.position, hedef);

        if (mesafe < arriveDistance)
        {
            currentIndex++;

            if (currentIndex >= WayPoints.Count)
            {
                Destroy(gameObject);
            }
        }
    }


}
