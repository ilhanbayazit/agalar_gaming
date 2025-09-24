using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int AltinSayisi = 0;
    [SerializeField] int BaslangicAltini;
    private int CanSayisi = 20;

    [SerializeField] TextMeshProUGUI AltinText;         // Canvas �zerindeki TMP objesi
    [SerializeField] TextMeshProUGUI CanSayisiText;     // Canvas �zerindeki TMP objesi

    private void Start()
    {
        AltinSayisi = BaslangicAltini;
        TextGuncelle();
    }
    public void AltinEkle(int Miktar)
    {
        AltinSayisi += Miktar;
        TextGuncelle();
    }
    public void AltinSil(int Miktar)
    {
        AltinSayisi -= Miktar;
        TextGuncelle();
    }
    public void CanSayisiAzal(int Miktar)
    {
        CanSayisi -= Miktar;
        TextGuncelle();
    }

    public int GetAltinSayisi()
    {
        return AltinSayisi;
    }

    public void TextGuncelle()
    {
        AltinText.text = AltinSayisi.ToString();
        CanSayisiText.text = CanSayisi.ToString();

    }

}
