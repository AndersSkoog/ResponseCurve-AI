using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public class Walk : StateMachineBehaviour {
	NavMeshAgent agent;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		agent = animator.GetComponent<NavMeshAgent> ();			
	}
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		var lookrot = Quaternion.LookRotation (agent.desiredVelocity);
		agent.velocity = animator.deltaPosition / Time.deltaTime;
		animator.transform.rotation = lookrot;
		if (!agent.hasPath) {
			animator.SetTrigger ("halt");
		}
	}
}

