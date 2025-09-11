using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DusmanSc : MonoBehaviour
{
    GameObject WayPointsParent;
    List<Transform> WayPoints = new List<Transform>();
    int currentIndex = 0;                // Şu anki hedef waypoint
    public float speed = 3f;             // Hız
    public float arriveDistance = 0.5f;  // Hedefe yaklaşma mesafesi


    [Header("Can Ayarları")]
    [SerializeField] float maxCan = 100;
    [SerializeField] float can = 100;
    private float hedefFill;

    [Header("UI")]
    [SerializeField] GameObject canvas;  // Anında değişen bar
    [SerializeField] Image KirmiziBar;  // Anında değişen bar
    [SerializeField] Image BeyazBar;    // Yavaş değişen bar

    private void Start()
    {
        hedefFill = (float)can / maxCan;
        WayPointsParent = GameObject.Find("WaypointParent");
        for (int i = 0; i < WayPointsParent.transform.childCount; i++)
        {
            WayPoints.Add(WayPointsParent.transform.GetChild(i));
        }

    }

    private void Update()
    {
        HedefeGit();
        GuncelleBarlar();
        HedefeBak();
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
                return;
            }
        }
    }
   
    void HedefeBak()
    {
        int rotationSpeed = 5;
        if (WayPoints.Count == 0) return;
        if (currentIndex >= WayPoints.Count) return;
        Vector3 hedef = WayPoints[currentIndex].position;

        Vector3 hedefPos = new Vector3(hedef.x, transform.position.y, hedef.z);
        Quaternion hedefRot = Quaternion.LookRotation(hedefPos - transform.position);
        hedefRot *= Quaternion.Euler(0f, 180f, 0f);

        transform.rotation = Quaternion.Slerp( transform.rotation,hedefRot,rotationSpeed * Time.deltaTime);
    }


    public void HasarAl(float hasar)
    {
        CanvasAc();
        can -= hasar;
        can = Mathf.Clamp(can, 0, maxCan);

        hedefFill = (float)can / maxCan;
        KirmiziBar.fillAmount = hedefFill;

        if (can <= 0)
        {
            Destroy(gameObject);
        }
    }

    void GuncelleBarlar()
    {
        BeyazBar.fillAmount = Mathf.Lerp(BeyazBar.fillAmount, hedefFill, Time.deltaTime * 3f);
    }

    void CanvasAc()
    {

        if (!canvas.activeSelf)
        {
            canvas.SetActive(true);
        }
     
    }


}
