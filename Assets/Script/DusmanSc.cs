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


    [SerializeField] Vector3 lookOffsetEuler = new Vector3(0f, 0f, 0f);
    [SerializeField] bool IsFly;

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
        if (IsFly)
        {
            hedef.y += 2;
        }

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

        if (IsFly)
        {
            hedef.y += 2;
        }

        Vector3 hedefPos = new Vector3(hedef.x, transform.position.y, hedef.z);
        Quaternion hedefRot = Quaternion.LookRotation(hedefPos - transform.position);

        hedefRot *= Quaternion.Euler(lookOffsetEuler);

        transform.rotation = Quaternion.Slerp(transform.rotation, hedefRot, rotationSpeed * Time.deltaTime);
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


    public void YoldaSek(float kuvvet,float SekmeSuresi)
    {
        Debug.Log("cagrildim");
        if (!gameObject.activeInHierarchy) return;

        StartCoroutine(Sekme());

        IEnumerator Sekme()
        {
            Debug.Log("cagrildim");
            float old = speed;
            speed = -speed * kuvvet;          // hızı -kuvvet ile çarp
            yield return new WaitForSeconds(SekmeSuresi); // 0.5 sn böyle kalsın
            speed = old;                       // normale dön
        }
    }








}
