using UnityEngine;
using UnityEngine.UI;

public class Ayarlarcanvas : MonoBehaviour
{
    [SerializeField] GameObject[] AcilcakPanel;
    [SerializeField] Scrollbar muzikDuzeyi;
    [SerializeField] GameObject SesIcon;
    [SerializeField] Sprite sesliSprite;
    [SerializeField] Sprite sessizSprite;

    [SerializeField] float varsayilanSes = 0.5f;
    float sonSes = 0.5f;

    void Start()
    {
        if (muzikDuzeyi) MuzikSesAyarla(sonSes);
    }
    public void MuzikSesAyarla(float Value)
    {
        SesManagerSc.Instance.SesOraniGuncelle(Value);
        ImageKontrol(Value);
    }

    void ImageKontrol(float v)
    {
        SesIcon.GetComponent<Image>().sprite = v <= 0.0001f ? sessizSprite : sesliSprite;
    }

    public void SesIconTikla()
    {
        float v = muzikDuzeyi ? muzikDuzeyi.value : 0f;
        bool kapat = v > 0.0001f;

        if (kapat)
        {
            if (v > 0.0001f) sonSes = v;
            if (muzikDuzeyi) muzikDuzeyi.value = 0f;
            MuzikSesAyarla(0f);
        }
        else
        {
            float hedef = (sonSes > 0.0001f ? sonSes : Mathf.Clamp01(varsayilanSes));
            if (muzikDuzeyi) muzikDuzeyi.value = hedef;
            MuzikSesAyarla(hedef);
        }
    }

    public void AyarlarKapat()
    {
        gameObject.SetActive(false);
        PauseCanvaas.Instance.SonPanel.SetActive(true);
    }

}
