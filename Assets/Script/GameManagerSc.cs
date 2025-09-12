using Unity.VisualScripting;
using UnityEngine;

public class GameManagerSc : MonoBehaviour
{

    [SerializeField] GameObject Karinca;

    [SerializeField] GameObject Kene;


    [SerializeField] Transform SpawnPoint;
    [SerializeField] GameObject Dusmanlar;

    [SerializeField] GameObject Player;
    [SerializeField] Transform SpawnPointPlayer;



    [Header("Spawn Ayarları")]
    [SerializeField] float spawnDelay = 2f;       // Başlangıç spawn süresi
    [SerializeField] float minSpawnDelay;  // Minimum spawn süresi

    float zaman = 0f;            // Geçen süre

    private void Start()
    {
        PlayerSpawn();
    }
    private void Update()
    {
        DusmanSpawnHizlandirma();
        OyunHizlandirma();
    }

    void DusmanSpawn()
    {
        int sayi = Random.Range(1, 3);
        switch (sayi)
        {
            case 1:
                Instantiate(Kene, SpawnPoint.position, Kene.transform.localRotation, Dusmanlar.transform);
                return;
            case 2:
                Instantiate(Karinca, SpawnPoint.position, Karinca.transform.localRotation, Dusmanlar.transform);
                return;
            default:
                return;
        }

    }

    void PlayerSpawn()
    {
        Instantiate(Player, SpawnPointPlayer.position, Player.transform.localRotation);
        // Debug.Log("oyuncu olusturuldu");
    }

    void DusmanSpawnHizlandirma()
    {
        zaman += Time.deltaTime;

        // Belirlenen süre geçtiyse düşman spawnla
        if (zaman >= spawnDelay)
        {
            DusmanSpawn();
            zaman = 0f;

            // Her spawn sonrası hızı biraz arttır
            if (spawnDelay > minSpawnDelay)
                spawnDelay -= 0.1f;
        }
    }

    void OyunHizlandirma()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale+=0.5f;
            ++Time.timeScale;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (Time.timeScale>=1)
            {

                Time.timeScale -= 0.5f;
                --Time.timeScale;

            }
        }
    }
}
