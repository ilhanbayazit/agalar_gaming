using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvaas : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject KazanmaPanel;
    [SerializeField] GameObject KaybetmePanel;
    [SerializeField] GameObject AyarlarPanel;

    public bool OyunDurduMu = false;
    public static PauseCanvaas Instance;
  public  GameObject SonPanel;

    private void Awake()
    {
        Instance = this;
    }


    public void KaybetmePanelAc()
    {
        KaybetmePanel.SetActive(true);
        SonPanel = KaybetmePanel;
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
        SonPanel= KazanmaPanel;
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
        SonPanel= PausePanel;
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

    public void btnNextLevel()
    {
        SceneManager.LoadScene(++LevelManager.instance.Level);
        Time.timeScale = 1f;
    }
    public void  btnAyarlarAcKapa()
    {
        bool ac = !AyarlarPanel.activeSelf;
        AyarlarPanel.SetActive(ac);
        SonPanel.SetActive(!ac);
    }

}
