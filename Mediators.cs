using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;


public struct Ad
{
	public int needIndex  {get; private set;}
	public float needGain {get; private set;}
	public Vector3 targetPos {get; private set;}
	public Ad(int ni, float ng, Vector3 tp)
	{
		needIndex = ni;
		needGain = ng;
		targetPos = tp;
	}
}




public interface Mediator
{
	void makeBooking (string targetID,string npcID);
	void startInteraction(string targetID,string npcID);
	void endInteraction (string targetID,string npcID);
	void advertise (string targetID,int needIndex,float needGain);
	void evalAds (string npcID);
}


public class AffectiveMediator : Mediator 
{
	public Dictionary<string,RC_TARGET> targets 
	{
		get 
		{
			return RC_TARGET.FindObjectsOfType<RC_TARGET> ()
				.ToDictionary ((k) => k.GetInstanceID ().ToString (), (v) => v);
		}
	}
	public Dictionary<string,RC_NPC> npcs
	{
		get
		{
			return RC_NPC.FindObjectsOfType<RC_NPC>()
				.ToDictionary ((k) => k.GetInstanceID().ToString(),(v) => v);
		}
	}
	public Dictionary<string,Ad> advertisements;
	public void makeBooking(string targetID,string npcID)
	{
		if (advertisements.ContainsKey(targetID)) 
		{
			advertisements.Remove (targetID);
			Debug.Log ("madebooking" + targetID);
			var npc = npcs [npcID];
			var target = targets [targetID];

			npc.agent.SetDestination (target.transform.position);
			npc.animator.SetTrigger ("walk");
			npc.StartCoroutine (npc.move (target.transform.position, target.area, new Action (() => {
				//npc.agent.Stop();
				npc.animator.runtimeAnimatorController = RC_NPC.NpcOverideController(target.smartobj.enterclip,target.smartobj.mainclip,target.smartobj.exitclip);
				npc.needCurveFrames[target.needIndex] = Mathf.Clamp (npc.needCurveFrames[target.needIndex] - target.needGain, 0f, 100f);
				npc.transform.rotation = target.transform.rotation;
				npc.transform.position = target.transform.position;
				npc.animator.SetInteger("targetID",int.Parse(targetID));
				npc.animator.SetTrigger("interact");
				//startInteraction(targetID,npcID);
			})));
//			npcs [npcID].GetComponent<NavMeshAgent> ().SetDestination (targets [targetID].transform.position);
//			npcs[npcID].GetComponent<Animator> ().SetTrigger ("walk");
//			npcs [npcID].StartCoroutine (npcs [npcID]
//				.move
//				(
//					targets [targetID].transform.position,
//					targets [targetID].area,
//					new Action (() => {
//						npcs[npcID].GetComponent<NavMeshAgent>().Stop();
//						npcs[npcID].GetComponent<Anim>
//						npcs [npcID].GetComponent<Animator> ().SetTrigger ("interact");	
//						startInteraction(targetID,npcID);
//					})
//				));
		}	
	}
	public void startInteraction(string targetID,string npcID)
	{
//		var npc = npcs [npcID];
//		var target = targets [targetID];
//		npc.needCurveFrames[target.needIndex] = Mathf.Clamp (npc.needCurveFrames[target.needGain] - target.needGain, 0f, 100f);
//		npc.animator.SetTrigger("interact");
	}
	public void endInteraction(string targetID,string npcID)
	{
//		Debug.Log("interactonEnded");
//		advertise (targetID, targets [targetID].needIndex, targets [targetID].needGain);
	}
	public void advertise(string targetID,int needIndex,float needGain)
	{
		Debug.Log ("newAD");
		advertisements [targetID] = new Ad (needIndex, needGain, targets [targetID].transform.position);
	}

	public void evalAds(string npcID)
	{
		var npc = npcs [npcID];
		var s = npc.needCurveFrames.ExtractProps(kv => RC_NEED.evaluate(kv.Key,kv.Value)).Sum();
		var lvl = (100 * s) / RC_NEED.maxneedsum ();
		float bestscore = -1;
		string scoreholder = "";
		foreach (string k in advertisements.Keys.ToArray()) {
			var f = npcs [npcID].needCurveFrames.Values.ToArray();
			var v = f [advertisements [k].needIndex] - advertisements [k].needGain; 
			//Debug.Log ("new frame" + v);
			f [advertisements[k].needIndex] = v;
			var newsum = f.ExtractProps (val => RC_NEED.evaluate (advertisements [k].needIndex, val)).Sum ();
			//Debug.Log ("new sum" + newsum);
			var newlvl = (100 * newsum) / RC_NEED.maxneedsum();
			//Debug.Log ("new lvl" + newlvl);
			var score = Math.Abs (newlvl - lvl);
			//Debug.Log ("the score" + score);
			if (score > bestscore) {
				bestscore = score;
				scoreholder = k;
				Debug.Log ("bestscore: " + scoreholder);
			}
		} 
		//Debug.Log (scoreholder);
		makeBooking (scoreholder, npcID);
	}
	public AffectiveMediator()
	{
		advertisements = new Dictionary<string, Ad> ();
	}
}
public static class mediators 
{
	public static Mediator affectiveMediator = new AffectiveMediator();
}