using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnaMenuCanvasSc : MonoBehaviour
{
    [SerializeField] GameObject AnamenuPanel;
    [SerializeField] GameObject BolumlerPanel;
    [SerializeField] GameObject AyarlarPanel;
    SaveSistemiSc save;


    private void Start()
    {
        save = GameObject.Find("SaveSistemi").GetComponent<SaveSistemiSc>();
        DontDestroyOnLoad(save);
        RefreshByPrefs();
    }

    public void btnOyna()
    {
        bool ac = !BolumlerPanel.activeSelf;
        BolumlerPanel.SetActive(ac);
        AnamenuPanel.SetActive(!ac);
    }

    public void btnAyarlar()
    {
        AyarlarPanel.SetActive(true);
        AnamenuPanel.SetActive(false);
    }

    public void btnAyarlarCikis()
    {
        AyarlarPanel.SetActive(false);
        AnamenuPanel.SetActive(true);
    }


    #region Bolumler 
    public void btnBolumlerCikis()
    {
        bool ac = !BolumlerPanel.activeSelf;
        BolumlerPanel.SetActive(ac);
        AnamenuPanel.SetActive(!ac);
    }

    public void LevelAc(int level)
    {
         SceneManager.LoadScene(level);
    }

    public void btnBolum1()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 1)
        {
            LevelAc(1);
        }
    }
    public void btnBolum2()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 2)
        {
            LevelAc(2);

        }
    }
    public void btnBolum3()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 3)
        {
            LevelAc(3);
        }
    }

    public void btnBolum4()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 4)
        {
            LevelAc(4);
        }
    }
    public void btnBolum5()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 5)
        {
            LevelAc(5);
        }
    }
    public void btnBolum6()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 6)
        {
            LevelAc(6);
        }
    }
    public void btnBolum7()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 7)
        {
            LevelAc(7);
        }
    }
    public void btnBolum8()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 8)
        {
            LevelAc(8);
        }
    }
    public void btnBolum9()
    {
        if (PlayerPrefs.GetInt("Level", 0) >= 9)
        {
            LevelAc(9);
        }
    }
    #endregion


    #region KilitliLeveller

    [SerializeField] Transform bolumlerRoot;   // "Bolumler" parent
    [SerializeField] Sprite lockedSprite;      // Kilitli görsel


    public void RefreshByPrefs()
    {
        int playerLevel = PlayerPrefs.GetInt("Level", 1);
        foreach (var btn in bolumlerRoot.GetComponentsInChildren<Button>(true))
        {
            int req = ExtractNumber(btn.name);         // "Button (5)" -> 5
            bool open = playerLevel >= req;

            var img = btn.image;
            if (img)
                img.sprite = open ? img.sprite : (lockedSprite ? lockedSprite : img.sprite);

            btn.interactable = open;

            var colors = btn.colors;                   // silikleşmeyi kapat
            colors.disabledColor = Color.white;
            btn.colors = colors;
        }
    }

    int ExtractNumber(string s)
    {
        int val = 0;
        foreach (char c in s)
            if (char.IsDigit(c)) { val = val * 10 + (c - '0'); }
        return val > 0 ? val : 1;
    }

    #endregion
}
