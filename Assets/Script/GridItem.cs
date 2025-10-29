using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GridItem : MonoBehaviour
{
    [SerializeField] Image Icon;  // SlotPrefab içinde atanır

    int _index;
    Action<int> _tiklama;

    public void Ayarla(int index, Action<int> tiklama)
    {
        _index = index;
        _tiklama = tiklama;

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => _tiklama?.Invoke(_index));
    }

    public void GorselAyarla(Sprite s)
    {
        if (!Icon) Icon = GetComponentInChildren<Image>(true);
        Icon.sprite = s;
        Icon.enabled = s != null;
    }

    public void EtkilesimAyarla(bool acik)
    {
        GetComponent<Button>().interactable = acik;
        if (Icon) Icon.color = acik ? Color.white : new Color(1, 1, 1, 0.35f);
    }
}
