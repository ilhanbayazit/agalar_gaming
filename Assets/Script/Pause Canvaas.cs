using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvaas : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject KazanmaPanel;
    [SerializeField] GameObject KaybetmePanel;
    public bool OyunDurduMu = false;
    public static PauseCanvaas Instance;

    private void Awake()
    {
        Instance = this;
    }


    public void KaybetmePanelAc()
    {
        KaybetmePanel.SetActive(true);
        Time.timeScale = 0f;
        OyunDurduMu = true;
        if (BuildManagerSc.aktif != null)
        {
            BuildManagerSc.aktif.PanelKapat();
        }
    }

    public void KazanmaPanelAc()
    {
        KazanmaPanel.SetActive(true);
        Time.timeScale = 0f;
        OyunDurduMu = true;
        if (BuildManagerSc.aktif != null)
        {
            BuildManagerSc.aktif.PanelKapat();
        }

    }
    public void btnPause()
    {
        if (OyunDurduMu) return;
        PausePanel.SetActive(true);
        OyunDurduMu = true;
        Time.timeScale = 0f;
        if (BuildManagerSc.aktif != null)
        {
            BuildManagerSc.aktif.PanelKapat();
        }
    }
    public void btnDevamEt()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
        OyunDurduMu = false;

    }
    public void btnResetAt()
    {
        Time.timeScale = 1f;
        if (LevelManager.instance == null) return;
        SceneManager.LoadScene(LevelManager.instance.Level);
    }
    public void btnAnamenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        SesManagerSc.Instance.Durdur();
    }
}
