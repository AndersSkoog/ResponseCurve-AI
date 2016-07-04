using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using G = Globals;
using System.Linq;
using System;

public class ImplMediator : IMediator 
{
	public Dictionary<string,Npc> npcs {get{return Npc.FindObjectsOfType<Npc> ().ToDictionary ((k) => k.name, (v) => v);}}
	public Dictionary<string,Target> targets {get{return Target.FindObjectsOfType<Target>().ToDictionary ((k) => k.name, (v) => v);}}
	public Dictionary<string,Advertisement> advertisments;
	public void evaluateAdvertisements(string npcName)
	{
		Debug.Log ("evaluating");
		var npc = npcs [npcName];
		if (npc.highestNeed == "Social")
		{
			if (advertisments.Any (kv => kv.Value.target.asociatedNeed.name == "Social")) {
				var ad = advertisments.Single (kv => kv.Value.target.asociatedNeed.name == "Social");
				attendTarget (ad.Value.target.name, npcName);
				advertisments.Remove (ad.Key);
			} 
			else 
			{
				startAdvertisement (npc.socialTarget.name);
				var tiles = GameObject.FindGameObjectsWithTag ("floor");
				var waitpos = tiles [UnityEngine.Random.Range (0, tiles.Length)].transform.position;
				npc.agent.SetDestination (waitpos);
				npc.animator.SetTrigger ("walk");
			}

		}
		else
		{
			float bestscore = -1f;
			string scoreholder = "";
			advertisments.ToList ().ForEach (kv => {
				var score = kv.Value.getMotivationScore(npc);
				//Debug.Log(kv.Key + "scored: " + score);
				if(score > bestscore){
					bestscore = score;
					scoreholder = kv.Key;
					//Debug.Log("new scoreholder: " + kv.Key);
				}
			});
			attendTarget (advertisments [scoreholder].target.name, npcName);
			advertisments.Remove (scoreholder);
		}
	}
	public void attendTarget(string targetName,string npcName)
	{
		Debug.Log (npcName + " is attending target:" + targetName);
		var npc = npcs[npcName];
		var target = targets[targetName];
		npc.animator.SetTrigger ("walk");
		npc.agent.SetDestination (target.transform.position);
		//npc.StartCoroutine(npc.FollowTarget (1f, target.transform));
		npc.StartCoroutine (routine (
			() => {return Vector3.Distance(target.transform.position,npc.transform.position) <= 1f;},
			() => {},
			() => {startInteraction (targetName,npcName);}));
	}
	public void startInteraction(string targetName,string npcName)
	{
		var isSocial = targets[targetName].asociatedNeed.name == "Social";
		var interaction = new Action<Npc,Target> ((npc, target) => {
			npc.animator.runtimeAnimatorController = Npc.NpcOverideController(target.enterMotion,target.mainMotion,target.exitMotion);
			npc.animator.SetInteger ("targetID", int.Parse (targetName));
			target.exitTime = Time.time + target.interactionPeriod;
			npc.needCurveFrames[target.needIndex] = Mathf.Clamp(npc.needCurveFrames[target.needIndex] - target.needGain, 0f, 100f);
			npc.transform.position = target.transform.position;
			npc.transform.rotation = target.transform.rotation;
			npc.animator.SetTrigger ("interact");
			target.StartCoroutine (routine (
				() => {return Time.time >= target.exitTime;},
				() => {},
				() => 
				{
					npc.animator.SetTrigger("exit");
				}));
		});
		if (isSocial) {
			var peer1 = npcs [npcName];
			var peer2 = npcs.Single (kv => kv.Value.socialTarget == targets [targetName]).Value;
			var peer1target = targets [targetName];
			var peer2target = npcs [npcName].socialTarget;
			interaction (peer1, peer1target);
			interaction (peer2, peer2target);
		}
		else 
		{
			interaction (npcs [npcName], targets [targetName]);
		}
	}

	public void startAdvertisement(string targetName)
	{
		Debug.Log ("starting advertisment");
		advertisments[Guid.NewGuid().ToString()] = new Advertisement (targetName);
	}
	public IEnumerator routine(Func<bool> exitpred,Action loopaction,Action onexit)
	{
		bool isrunning = true;
		while (isrunning) 
		{
			if (exitpred ()) {
				isrunning = false;
				onexit ();
			} 
			else 
			{
				loopaction();
			}
			yield return null;
		}
	}
	public ImplMediator()
	{
		advertisments = new Dictionary<string,Advertisement>();

	}
}
