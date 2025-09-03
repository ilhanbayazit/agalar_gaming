using UnityEngine;

public class GameManagerSc : MonoBehaviour
{

    [SerializeField] GameObject Dusman;

    [SerializeField] Transform SpawnPoint;

    [SerializeField] GameObject Dusmanlar;

    [SerializeField] GameObject Player;

    [SerializeField] Transform SpawnPointPlayer;



    [Header("Spawn Ayarları")]
    [SerializeField] float spawnDelay = 2f;       // Başlangıç spawn süresi
    [SerializeField] float minSpawnDelay ;  // Minimum spawn süresi

    float zaman = 0f;            // Geçen süre

    private void Start()
    {
        PlayerSpawn();
    }
    private void Update()
    {
        DusmanSpawnHizlandirma();
    }

    void DusmanSpawn()
    {
        Instantiate(Dusman, SpawnPoint.position, Quaternion.identity, Dusmanlar.transform);
      //  Debug.Log("Dusman Olusturuldu.");
    }

    void PlayerSpawn()
    {
        Instantiate(Player, SpawnPointPlayer.position, Quaternion.identity);
        Debug.Log("oyuncu olusturuldu");
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
}
