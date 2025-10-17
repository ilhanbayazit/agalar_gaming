using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdsManagerSc : MonoBehaviour
{
    public static AdsManagerSc Instance;

    InterstitialAd interstitial;
    string interstitialId = "ca-app-pub-7163425476823301/7281167798";
    private int _sceneLoadCount = 0;
    private const int ShowInterstitialAfterScenes = 3;

    RewardedAd rewarded;
#if UNITY_ANDROID
    string rewardedId = "ca-app-pub-7163425476823301/2831922364"; 
#elif UNITY_IPHONE
string rewardedId = "ca-app-pub-3940256099942544/1712485313"; 
#else
string rewardedId = "unused";
#endif

    void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        MobileAds.Initialize(initStatus => { LoadInterstitial(); LoadRewarded(); });

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
    #region Odullu Reklam
    void LoadRewarded()
    {
        if (rewarded != null) { rewarded.Destroy(); rewarded = null; }

        var req = new AdRequest();
        RewardedAd.Load(rewardedId, req, (RewardedAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.LogWarning("Rewarded load fail: " + err);
                return;
            }

            rewarded = ad;

            rewarded.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded kapandı, yeniden yükleniyor.");
                LoadRewarded();
            };
            rewarded.OnAdFullScreenContentFailed += (AdError e) =>
            {
                Debug.LogWarning("Rewarded gösterim hatası: " + e);
                LoadRewarded();
            };
        });
    }
    public void ShowRewarded(Action onRewardEarned = null)
    {
        if (rewarded != null)
        {
            var ad = rewarded;      
            rewarded = null;
            ad.Show((Reward r) =>
            {
                PlayerStats.Instance.AltinEkle(250);
                PlayerStats.Instance.CanEkle(5);
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            LoadRewarded();
            Debug.Log("Rewarded hazır değil, yükleniyor.");
        }
    }

    #endregion
}
