using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject gamemngr;
    [SerializeField] int Level;
    GameManagerSc gameManager;
    private void Start()
    {
        gameManager = gamemngr.GetComponent<GameManagerSc>();
       StartCoroutine(BaslatDalgalar());
    }


    IEnumerator BaslatDalgalar()
    {
        // (spawner, S_count, S_interval, A_count, A_interval, laneOffset)
        var waves = new ((int sp, int S, float Si, int A, float Ai, float off) up,
                          (int sp, int S, float Si, int A, float Ai, float off) left)[]
        {
    // W1 – hafif
    (up:(0, 3, 2.4f, 1, 3.4f, 1.2f),
     left:(1, 1, 2.4f, 3, 3.4f, 1.2f)),

    // W2 – hafif artış
    (up:(0, 4, 2.1f, 2, 3.1f, 1.0f),
     left:(1, 3, 2.1f, 3, 3.1f, 1.0f)),

    // W3 – orta
    (up:(0, 6, 1.9f, 3, 2.8f, 0.8f),
     left:(1, 4, 1.9f, 4, 2.8f, 0.8f)),

    // W4 – belirgin zorlaşma
    (up:(0, 8, 1.6f, 6, 2.4f, 0.6f),
     left:(1, 6, 1.6f, 6, 2.4f, 0.6f)),

    // W5 – güçlendirildi (7 taretle sıkı)
    (up:(0,16, 1.15f, 16, 1.80f, 0.35f),
     left:(1,16, 1.15f, 16, 1.80f, 0.35f)),

    // W6 – final daha sert
    (up:(0,20, 1.05f, 20, 1.70f, 0.25f),
     left:(1,20, 1.05f, 20, 1.70f, 0.25f)),
        };



        for (int i = 0; i < waves.Length; i++)
        {
            // Yukarı hattı
            StartCoroutine(HatSpawnRutini(
                waves[i].up.sp,
                waves[i].up.S, waves[i].up.Si,
                waves[i].up.A, waves[i].up.Ai,
                waves[i].up.off));

            // Sol hattı
            StartCoroutine(HatSpawnRutini(
                waves[i].left.sp,
                waves[i].left.S, waves[i].left.Si,
                waves[i].left.A, waves[i].left.Ai,
                waves[i].left.off));

            // Dalga tahmini süre + ara
            float tahminiSure =
                Mathf.Max(
                    waves[i].up.off + Mathf.Max((waves[i].up.S - 1) * waves[i].up.Si, (waves[i].up.A - 1) * waves[i].up.Ai),
                    waves[i].left.off + Mathf.Max((waves[i].left.S - 1) * waves[i].left.Si, (waves[i].left.A - 1) * waves[i].left.Ai)
                ) + 5f;

            yield return new WaitForSeconds(tahminiSure);
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator HatSpawnRutini(int yolIndex, int sivriAdet, float sivriAralik, int karincaAdet, float karincaAralik, float ofset)
    {
        if (ofset > 0f) yield return new WaitForSeconds(ofset);

        if (sivriAdet > 0)
            yield return StartCoroutine(SpawnSerisi(() => gameManager.SivriSpawn(yolIndex), sivriAdet, sivriAralik));

        if (karincaAdet > 0)
            yield return StartCoroutine(SpawnSerisi(() => gameManager.KarincaSpawn(yolIndex), karincaAdet, karincaAralik));
    }

    IEnumerator SpawnSerisi(System.Action spawnCall, int adet, float aralik)
    {
        for (int i = 0; i < adet; i++)
        {
            spawnCall();
            if (i < adet - 1 && aralik > 0f)
                yield return new WaitForSeconds(aralik);
        }
    }
}
