﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chara : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject.Find(animator.transform.gameObject.name).GetComponent<CharacterMovement>().isAttacking = false;
        if (GameObject.Find(animator.transform.gameObject.name).GetComponent<CharacterMovement>().takingHit)
        {
            GameObject.Find(animator.transform.gameObject.name).GetComponent<CharacterMovement>().takingHit = false;
            if (GameObject.Find(animator.transform.gameObject.name).GetComponent<CharacterMovement>().isGuarding)
            {
                GameObject.Find(animator.transform.gameObject.name).GetComponent<CharacterMovement>().ChangeAnimation("Guard");
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
