using UnityEngine;

public class GameManagerSc : MonoBehaviour
{

    [SerializeField] GameObject Dusman;

    [SerializeField] Transform SpawnPoint;

    [SerializeField] GameObject Dusmanlar;

    [SerializeField] GameObject Player;

    [SerializeField] Transform SpawnPointPlayer;

    private void Start()
    {
        PlayerSpawn();
        InvokeRepeating(nameof(DusmanSpawn), 0f, 2f);

    }
    private void Update()
    {

    }

    void DusmanSpawn()
    {
        Instantiate(Dusman, SpawnPoint.position, Quaternion.identity, Dusmanlar.transform);
        Debug.Log("Dusman Olusturuldu.");
    }

    void PlayerSpawn()
    {
        Instantiate(Player, SpawnPointPlayer.position, Quaternion.identity);

    }

}
