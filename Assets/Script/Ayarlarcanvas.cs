using UnityEngine;
using UnityEngine.UI;

public class Ayarlarcanvas : MonoBehaviour
{
    [SerializeField] Scrollbar muzikDuzeyi;
    void Start()
    {
        if (muzikDuzeyi) MuzikSesAyarla(muzikDuzeyi.value);
    }
    public void MuzikSesAyarla(float Value)
    {
        SesManagerSc.Instance.SesOraniGuncelle(Value);
    }





}
