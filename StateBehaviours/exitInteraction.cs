using UnityEngine;
using System.Collections;

public class exitInteraction : StateMachineBehaviour {

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		var target = GameObject.Find (animator.GetInteger ("targetID").ToString ()).GetComponent<RC_TARGET> ();
		var npc = animator.GetComponent<RC_NPC>();
		npc.evalAds (npc.gameObject.name);
		target.advertise (target.name, target.needIndex, target.needGain);

	}
}
