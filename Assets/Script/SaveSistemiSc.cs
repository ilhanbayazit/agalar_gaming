using UnityEngine;

public class SaveSistemiSc : MonoBehaviour
{
    public static SaveSistemiSc Instance;
    private void Awake()
    {
        Instance=this;
    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.Save();
        }

    }
    public int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("Level", 0);
    }
    public void LevelAtla()
    {

        int Level = PlayerPrefs.GetInt("Level", 0);
        ++Level;
        PlayerPrefs.SetInt("Level", Level);
        PlayerPrefs.Save();
  
    }
}
