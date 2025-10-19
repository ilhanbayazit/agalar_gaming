
using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdsManagerSc : MonoBehaviour
{
    public static AdsManagerSc Instance;


    private int _sceneLoadCount = 0;
    private const int ShowInterstitialAfterScenes = 3;


    void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitial();
            LoadRewarded();               // varsa senin ödüllü fonksiyonun
            LoadRewardedInterstitial();   // YENİ
        });

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

    #region GecisResklami

    InterstitialAd interstitial;
    string interstitialId = "ca-app-pub-7163425476823301/7281167798";
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

    #endregion


    #region Odullu Gecis Reklami

    RewardedInterstitialAd rInterstitial;
#if UNITY_ANDROID
    string rInterstitialId = "ca-app-pub-7163425476823301/4187034934"; // TEST Rewarded-Interstitial
#elif UNITY_IPHONE
string rInterstitialId = "ca-app-pub-3940256099942544/6978759866"; // TEST Rewarded-Interstitial iOS
#else
string rInterstitialId = "unused";
#endif
    void LoadRewardedInterstitial()
    {
        if (rInterstitial != null) { rInterstitial.Destroy(); rInterstitial = null; }

        var req = new AdRequest();
        RewardedInterstitialAd.Load(rInterstitialId, req, (RewardedInterstitialAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.LogWarning("Rewarded-Interstitial load fail: " + err);
                return;
            }

            rInterstitial = ad;

            rInterstitial.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded-Interstitial kapandı, yeniden yükleniyor.");
                LoadRewardedInterstitial();
            };
            rInterstitial.OnAdFullScreenContentFailed += (AdError e) =>
            {
                Debug.LogWarning("Rewarded-Interstitial gösterim hatası: " + e);
                LoadRewardedInterstitial();
            };
        });
    }

    public void ShowRewardedInterstitial(Action onRewardEarned = null)
    {
        if (rInterstitial != null)
        {
            var ad = rInterstitial;  // tek kullanımlık
            rInterstitial = null;
            ad.Show((Reward r) =>
            {
                PlayerStats.Instance.AltinEkle(250);
                PlayerStats.Instance.CanEkle(5);
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded-Interstitial hazır değil, yükleniyor...");
            LoadRewardedInterstitial();
        }
    }


    #endregion


    #region Odullu Reklam
    RewardedAd rewarded;
#if UNITY_ANDROID
    string rewardedId = "ca-app-pub-7163425476823301/2831922364";
#elif UNITY_IPHONE
string rewardedId = "ca-app-pub-3940256099942544/1712485313"; 
#else
string rewardedId = "unused";
#endif
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
