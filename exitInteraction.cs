using UnityEngine;
using System.Collections;

public class exitInteraction : StateMachineBehaviour {
//	TestTarget target;
//	TestNpc npc;
//	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		target = GameObject.Find (animator.GetInteger ("targetID").ToString ()).GetComponent<TestTarget> ();
//		npc = animator.GetComponent<TestNpc>();
//	}
//
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log ("exit");
//		Debug.Log (animator.GetInteger ("targetID").ToString ());
		var target = GameObject.Find (animator.GetInteger ("targetID").ToString ()).GetComponent<Target> ();
		var npc = animator.GetComponent<Npc>();
		npc.evaluateAdvertisements(npc.name);
		if(target.asociatedNeed.name != "Social"){
			target.startAdvertisment (target.name);
		}

	

	}

}
