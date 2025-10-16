using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdsManagerSc : MonoBehaviour
{
    public static AdsManagerSc Instance;

    InterstitialAd interstitial;
    string interstitialId = "ca-app-pub-7163425476823301/7281167798";
    private int _sceneLoadCount = 0;
    private const int ShowInterstitialAfterScenes = 3;

    void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        MobileAds.Initialize(initStatus => { LoadInterstitial(); });
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _sceneLoadCount++;
        if (_sceneLoadCount >= ShowInterstitialAfterScenes)
        {
            ShowInterstitial();
            _sceneLoadCount = 0;
        }
    }
    void LoadInterstitial()
    {
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialId, adRequest, (InterstitialAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.LogWarning("Interstitial load fail: " + err);
                return;
            }

            interstitial = ad;

            // Olaylar
            interstitial.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial kapandı, yeniden yükleniyor.");
                LoadInterstitial();
            };
            interstitial.OnAdFullScreenContentFailed += (AdError e) =>
            {
                Debug.LogWarning("Interstitial gösterim hatası: " + e);
                LoadInterstitial();
            };
        });
    }
    public bool IsReady() => interstitial != null;
    public void ShowInterstitial()
    {
        if (interstitial != null)
        {
            interstitial.Show();
            interstitial = null; // tek kullanımlık; kapandıktan sonra yeniden yüklenecek
        }
        else
        {
            LoadInterstitial();
        }
    }
}
