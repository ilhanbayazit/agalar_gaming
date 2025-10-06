using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class DusmanSc : MonoBehaviour
{
    GameObject WayPointsParent;
    [SerializeField] List<Transform> WayPoints = new List<Transform>();
    int currentIndex = 0;                // Şu anki hedef waypoint
    public float speed = 3f;             // Hız
    public float arriveDistance = 0.5f;  // Hedefe yaklaşma mesafesi


    [Header("Can Ayarları")]
    [SerializeField] float maxCan = 100;
    [SerializeField] public float can = 100;
    private float hedefFill;

    [Header("UI")]
    [SerializeField] GameObject canvas;  // Anında değişen bar
    [SerializeField] Image KirmiziBar;  // Anında değişen bar
    [SerializeField] Image BeyazBar;    // Yavaş değişen bar


    [SerializeField] Vector3 lookOffsetEuler = new Vector3(0f, 0f, 0f);
    [SerializeField] public bool IsFly;

    [Header("Stats")]
    GameObject Stats;
    [SerializeField] int Odul;
    [SerializeField] int Hasar;

    [Header("Effect")]
    [SerializeField] ParticleSystem fxPrefab;
    [SerializeField] Vector3 BoyutCarpan = new Vector3(1,1,1);
    [SerializeField] Vector3 KonumOfSet;

    Vector3 spawnStart;

    private void Start()
    {
        spawnStart = transform.position;
        Stats = GameObject.Find("OyuncuBilgileriCanvas");
        hedefFill = (float)can / maxCan;
    }

    public void WaypointleriAyarla(List<Transform> wp)
    {
        WayPoints = wp;
        currentIndex = 0;
    }


    public float IlerlemeSkoru()
    {
        if (WayPoints == null || WayPoints.Count == 0) return float.NegativeInfinity;

        int idx = Mathf.Clamp(currentIndex, 0, WayPoints.Count - 1);
        float t = 0f; // [0,1] arası segment ilerleme

        if (idx == 0)
        {
            float seg = Vector3.Distance(spawnStart, WayPoints[0].position);
            float rem = Vector3.Distance(transform.position, WayPoints[0].position);
            t = seg > 0.0001f ? 1f - Mathf.Clamp01(rem / seg) : 0f;
        }
        else
        {
            Vector3 a = WayPoints[idx - 1].position;
            Vector3 b = WayPoints[idx].position;
            float seg = Vector3.Distance(a, b);
            float rem = Vector3.Distance(transform.position, b);
            t = seg > 0.0001f ? 1f - Mathf.Clamp01(rem / seg) : 0f;
        }

        return idx + t; // büyük olan daha ileride
    }

    private void Update()
    {
        HedefeGit();
        GuncelleBarlar();
        HedefeBak();
      if(Stats==null) Debug.Log("bis");
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
                Stats.GetComponent<PlayerStats>().CanSayisiAzal(Hasar);
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
            Stats.GetComponent<PlayerStats>().AltinEkle(Odul);
            OlmeEffecti();
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

    void OlmeEffecti()
    {
        if (fxPrefab != null)
        {
            var fx = Instantiate(fxPrefab, transform.position+ KonumOfSet, Quaternion.identity);
            fx.Play();
            var main = fx.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            fx.transform.localScale = Vector3.Scale(fx.transform.localScale, BoyutCarpan);
            var m = fx.main;
            Destroy(fx.gameObject, main.duration + main.startLifetime.constantMax);

        }
    }
    public void YoldaSek(float kuvvet, float SekmeSuresi)
    {
        if (!gameObject.activeInHierarchy) return;

        StartCoroutine(Sekme());

        IEnumerator Sekme()
        {
            float old = speed;
            speed = -speed * kuvvet;          // hızı -kuvvet ile çarp
            yield return new WaitForSeconds(SekmeSuresi); // 0.5 sn böyle kalsın
            speed = old;                      // normale dön
        }

    }








}
