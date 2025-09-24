using UnityEngine;
using System.Collections;

public class BlendTreeTrigger : MonoBehaviour
{
    Animator anim;
 
    void Awake() { anim = GetComponent<Animator>(); }



   void tetikle()
    {
        anim.SetTrigger("w1");
        anim.SetTrigger("w2");
        anim.SetTrigger("w3");
        anim.SetTrigger("w4");
        anim.SetTrigger("w5");
        anim.SetTrigger("w6");
        anim.SetTrigger("w7");


    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            tetikle();
        }
    }

}
