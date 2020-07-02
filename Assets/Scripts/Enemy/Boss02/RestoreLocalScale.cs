using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreLocalScale : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Heal bugs are child of boss, make sure local scale of this boss is positive so bugs are not flipped wrong way
        GameObject obj = animator.gameObject;
        obj.transform.localScale = obj.transform.localScale.x < 0 ?
            new Vector3(-obj.transform.localScale.x, obj.transform.localScale.y) :
            obj.transform.localScale;
    }
}
