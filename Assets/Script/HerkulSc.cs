using System.Collections;
using UnityEngine;

public class HerkulSc : MonoBehaviour
{
    Animator Anim;
    bool YuruyorMu;

    void Start()
    {
        Anim = GetComponent<Animator>();
        YuruAc();
    }

    private void Update()
    {
        if (gameObject.GetComponent<DusmanSc>().can < gameObject.GetComponent<DusmanSc>().maxCan * 0.45&&YuruyorMu)
        {
            Chargelen();
        }
    }

    void Chargelen()
    {
        if (!YuruyorMu) return;
        Anim.SetTrigger("Charge");
        gameObject.GetComponent<DusmanSc>().speed = 0f; 
        YuruKapa();
    }
    void YuruAc()
    {
        YuruyorMu = true;
        Anim.SetBool("YuruyorMu", true);
    }

    void YuruKapa()
    {
        YuruyorMu = false;
        Anim.SetBool("YuruyorMu", false);
    }

}
