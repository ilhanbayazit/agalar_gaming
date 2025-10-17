using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int AltinSayisi = 0;
    [SerializeField] int BaslangicAltini;

    private int CanSayisi = 20;
    [SerializeField] TextMeshProUGUI AltinText;         // Canvas üzerindeki TMP objesi
    [SerializeField] TextMeshProUGUI CanSayisiText;     // Canvas üzerindeki TMP objesi

    #region wave yazdirma

    [Header("UI Bağlantısı")]
    [SerializeField] TextMeshProUGUI waveText; // UI Text kullanıyorsan: UnityEngine.UI.Text

    [Header("Biçim Ayarları")]
    [SerializeField] string itemFormat = "wave {0}/{1}"; // {0}=current, {1}=total
    [SerializeField] string separator = " , ";          // öğeler arası ayraç
    [System.Serializable]
    public struct WaveProgress { public int current; public int total; }

    public void SetWave(int current, int total)
    {
        if (!waveText) return;
        waveText.text = string.Format(itemFormat, current, total);
    }

    public void SetWaves(params WaveProgress[] items)
    {
        if (!waveText) return;
        var sb = new StringBuilder();
        for (int i = 0; i < items.Length; i++)
        {
            if (i > 0) sb.Append(separator);
            sb.AppendFormat(itemFormat, items[i].current, items[i].total);
        }
        waveText.text = sb.ToString();
    }
    public void SetWavesList(List<WaveProgress> items) => SetWaves(items.ToArray());

    #endregion

    private void Awake()
    {
        Instance = this;
    }

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
        if (CanSayisi<=0)
        {
            PauseCanvaas.Instance.KaybetmePanelAc();
        }
        TextGuncelle();
    }
    public void CanEkle(int Miktar)
    {
        CanSayisi = Miktar;
        TextGuncelle();
    }
    public void TextGuncelle()
    {
        AltinText.text = AltinSayisi.ToString();
        CanSayisiText.text = CanSayisi.ToString();
    }
 

}
