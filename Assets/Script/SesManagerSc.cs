using System.Collections.Generic;
using UnityEngine;

public class SesManagerSc : MonoBehaviour
{
    public static SesManagerSc Instance;
    [SerializeField] AudioSource src;
    [SerializeField] AudioClip[] clips;
    Dictionary<string, AudioClip> map;

    float SesOrani;
    private void Awake()
    {
        Instance = this;
        map = new Dictionary<string, AudioClip>();
        foreach (var c in clips) if (c) map[c.name] = c;   // key: AudioClip adı
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RandomSarkiCal();
    }
    public void Durdur()
    {
        src.Stop();
    }
    public void Cal(string ad, float vol = 1f)
    {
        if (map != null && map.TryGetValue(ad, out var clip) && clip)
            src.PlayOneShot(clip, vol);
    }

   public void RandomSarkiCal()
    {
        string[] bgNames = { "ArkaPlan1", "ArkaPlan2", "ArkaPlan3" };
        if (bgNames == null || bgNames.Length == 0 || src == null) return;

        int r = Random.Range(0, 3);

        if (!map.TryGetValue(bgNames[r], out var clip) || clip == null) return;

        src.Stop();
        src.loop = true;        // loop açık
        src.clip = clip;        // klibi kaynağa ata
        src.volume = 0.1f;      // başlangıç ses seviyesi
        src.spatialBlend = 0f;  // 2D müzik (opsiyonel ama iyi)
        src.Play();             // PlayOneShot DEĞİL
    }

    public void SesOraniGuncelle(float x)
    {
        x *= 0.7f;
        src.volume =  x;  
    }

}
