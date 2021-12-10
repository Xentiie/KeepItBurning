using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticIdleAnimationBehaviour : StateMachineBehaviour {

    readonly public float changeIdleMinTime = 5;
    readonly public float changeIdleMaxTime = 7;

    float blinkTimer = 7;

    string[] idleTriggers = { "Idle_1" };

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (blinkTimer <= 0) {

            TriggerRandomIdle(animator);
            blinkTimer = Random.Range(changeIdleMinTime, changeIdleMaxTime);

        }
        else {

            blinkTimer -= Time.deltaTime;

        }
    }

    void TriggerRandomIdle(Animator animator) {

        System.Random rnd = new System.Random();
        int idleIndex = rnd.Next(idleTriggers.Length);
        string idleTrigger = idleTriggers[idleIndex];
        animator.SetTrigger(idleTrigger);
    }

}
