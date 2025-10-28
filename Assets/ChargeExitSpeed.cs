using UnityEngine;

public class ChargeExitSpeed : StateMachineBehaviour
{
    public float newSpeed = 2f; // charge bitince hedef hız
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var d = animator.GetComponent<DusmanSc>();
        if (d) d.speed = newSpeed; // charge biter bitmez hızlan
    }
}