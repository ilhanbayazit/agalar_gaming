using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class KraliceKarinca : MonoBehaviour
{
    Animator Anim;
    [SerializeField] GameObject yumurta;
    void Start()
    {
        Anim = GetComponent<Animator>();
        YuruAc();
        CanvasYokEt();
        InvokeRepeating(nameof(Yumurtla), 8f, 5f);
    }

    private void Update()
    {
        if (gameObject.GetComponent<DusmanSc>().can < gameObject.GetComponent<DusmanSc>().maxCan * 0.2)
        {
            CancelInvoke(nameof(Yumurtla));
            if (gameObject.GetComponent<DusmanSc>().IsFly) return;
            Havalan();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Yumurtla();
        }
    }
    public void Havalan()
    {
        Anim.SetTrigger("Havalan");
        GetComponent<DusmanSc>().IsFly = true;
        YuruKapa();
        StartCoroutine(BekleUc(2.4f));

    }

    public void Uc()
    {
        Anim.SetBool("Ucma", true);
    }

    IEnumerator BekleUc(float x)
    {
        gameObject.tag = "Finish";
        gameObject.GetComponent<DusmanSc>().speed = 0.1f;
        yield return new WaitForSeconds(x);
        gameObject.GetComponent<BoxCollider>().center = new Vector3(1.3f, 11f, 0);
        transform.GetChild(3).localPosition = new Vector3(4, 12, 0);
        gameObject.GetComponent<DusmanSc>().speed = 1;
        gameObject.tag = "FlyingEnemy";
        Uc();
    }
    public void YuruAc()
    {
        Anim.SetBool("Yurume", true);
    }
    public void YuruKapa()
    {
        Anim.SetBool("Yurume", false);
    }
    public void Yumurtla()
    {
        Anim.SetTrigger("Yumurtla");
        StartCoroutine(BekleYumurtla(1f));
    }

    IEnumerator BekleYumurtla(float x)
    {
        gameObject.GetComponent<DusmanSc>().speed = 0;
        yield return new WaitForSeconds(x / 2);
        Instantiate(yumurta, gameObject.transform.position, yumurta.transform.localRotation);
        yield return new WaitForSeconds(x / 2);
        gameObject.GetComponent<DusmanSc>().speed = 1;
    }

    public void CanvasYokEt()
    {
        PlayerStats.Instance.gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        PlayerStats.Instance.gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

}
