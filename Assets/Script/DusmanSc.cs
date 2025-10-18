using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DusmanSc : MonoBehaviour
{
    GameObject WayPointsParent;
    [SerializeField] List<Transform> WayPoints = new List<Transform>();
    int currentIndex = 0;                // Şu anki hedef waypoint
    public float speed ;             // Hız
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
    private bool SekiyorMu;

    private void Start()
    {
        spawnStart = transform.position;
        Stats = GameObject.Find("OyuncuBilgileriCanvas");
        hedefFill = (float)can / maxCan;
    }


    private float toplamYol = 0f;

    public void WaypointleriAyarla(List<Transform> wp)
    {
        WayPoints = wp;
        currentIndex = 0;

        toplamYol = 0f;
        if (wp != null && wp.Count > 0)
        {
            toplamYol += Vector3.Distance(spawnStart, wp[0].position);
            for (int i = 1; i < wp.Count; i++)
                toplamYol += Vector3.Distance(wp[i - 1].position, wp[i].position);
        }

    }

    public float IlerlemeSkoru()
    {
        if (WayPoints == null || WayPoints.Count == 0) return float.PositiveInfinity;

        int idx = Mathf.Clamp(currentIndex, 0, WayPoints.Count - 1);

        float kalan = 0f;
        kalan += Vector3.Distance(transform.position, WayPoints[idx].position);
        for (int i = idx; i < WayPoints.Count - 1; i++)
            kalan += Vector3.Distance(WayPoints[i].position, WayPoints[i + 1].position);

        return kalan;
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

    bool oldu = false;

    public void HasarAl(float hasar)
    {
        CanvasAc();
        can -= hasar;
        can = Mathf.Clamp(can, 0, maxCan);

        hedefFill = (float)can / maxCan;
        KirmiziBar.fillAmount = hedefFill;

        if (can <= 0)
        {
            if (!oldu)
            {
                Stats.GetComponent<PlayerStats>().AltinEkle(Odul);
                oldu = true;
            }
            OlmeEffecti();
            SesManagerSc.Instance.Cal("OlumSesi", 0.3f);
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

    float normalSpeed;
    Coroutine sekmeCR;
    public void YoldaSek(float kuvvet, float SekmeSuresi)
    {
        if (!gameObject.activeInHierarchy) return;

        if (sekmeCR != null) StopCoroutine(sekmeCR);
        sekmeCR = StartCoroutine(Sekme());

        IEnumerator Sekme()
        {
            if (normalSpeed == 0f) normalSpeed = Mathf.Abs(speed);
            speed = -normalSpeed * kuvvet;
            yield return new WaitForSeconds(SekmeSuresi);
            speed = normalSpeed;
            sekmeCR = null;
        }
    }









}
