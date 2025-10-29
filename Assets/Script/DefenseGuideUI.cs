using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class DefenseGuideUI : MonoBehaviour
{
    [SerializeField] GameObject[] Tabs;
    [SerializeField] Image[] TabButtons;

    [SerializeField] Sprite AktifTabBtn, DeAktifTabBtn;
    [SerializeField] Vector2 AktifTabBoyut, DeAktifTabBoyut;

    void Start()
    {
        TabDegis(0);
    }
    public void TabDegis(int TabId)
    {
        foreach (var tab in Tabs)
        {
            tab.SetActive(false);
        }
        Tabs[TabId].SetActive(true);

        foreach (var im in TabButtons)
        {
            im.sprite = DeAktifTabBtn;
            im.rectTransform.sizeDelta = DeAktifTabBoyut;
        }
        TabButtons[TabId].sprite = AktifTabBtn;
        TabButtons[TabId].rectTransform.sizeDelta = AktifTabBoyut;
    }

    [SerializeField] GameObject[] Turrets;
    [SerializeField] Image AnaImage;
  
    public void TurretSec(int index)
    {
        var turret =Turrets[index].GetComponent<TowerInfo>();
       
        BilgileriGuncelle(turret);
    }

    [SerializeField] TMP_Text AdTxt, HasarTxt, AtisHiziTxt, MaliyetTxt;

    void BilgileriGuncelle(TowerInfo t)
    {
        AnaImage.sprite = t.BuyukGorsel;
        AdTxt.text = t.Ad;
        HasarTxt.text = t.Hasar.ToString("0.##");
        AtisHiziTxt.text = t.SaldiriHizi.ToString("0.##");
        MaliyetTxt.text = t.buildCost.ToString();
    }
}