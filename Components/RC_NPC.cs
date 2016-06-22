using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEditor.Animations;
using UnityEditor;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class RC_NPC : RC_ENTITY 
{
	public float nexttick;
	public Dictionary<int,float> needCurveFrames;
	public NavMeshAgent agent {get{return GetComponent<NavMeshAgent>();}}
	public Animator animator {get{return GetComponent<Animator> ();}}




	public static AnimatorController NpcDefaultController()
	{	
		return (AnimatorController)AssetDatabase.LoadAssetAtPath ("Assets/Standard Assets/RC/Serialized/Controllers/DefaultController.controller",typeof(AnimatorController));
	}
	public static AnimatorOverrideController NpcOverideController(AnimationClip enter,AnimationClip main, AnimationClip exit)
	{
		var ret = new AnimatorOverrideController ();
		ret.runtimeAnimatorController = (RuntimeAnimatorController)NpcDefaultController();
		ret.clips = new AnimationClipPair[]
		{
			enter.ClipPair(OverideClips()[0]),
			main.ClipPair(OverideClips()[1]),
			exit.ClipPair(OverideClips()[2])
		};
		return ret;
	}
	public static AnimationClip[] OverideClips()
	{
		return new AnimationClip[] {
			(AnimationClip)NpcDefaultController().layers [0].stateMachine.states.Single (cs => cs.state.name == "enter").state.motion,
			(AnimationClip)NpcDefaultController().layers [0].stateMachine.states.Single (cs => cs.state.name == "main").state.motion,
			(AnimationClip)NpcDefaultController().layers [0].stateMachine.states.Single (cs => cs.state.name == "exit").state.motion,
		};
	}




	public IEnumerator move(Vector3 pos,float stopRange ,Action atpos)
	{
		bool isrunning = true;
		while (isrunning)
		{
			if (pos.bounds(stopRange).Contains(transform.position))
			{
				//this.StopCoroutine ("move");
				isrunning = false;
				atpos ();
			}
			yield return null;
		}
	}
	public IEnumerator routine(Func<bool> exitpred,Action loopaction,Action onexit)
	{
		bool isrunning = true;
		while (isrunning) 
		{
			if (exitpred ()) {
				isrunning = false;
				onexit ();
				//this.StopCoroutine ("routine");
			} 
			else 
			{
				loopaction();
			}
			yield return null;
		}
	}



	 
	void Awake()
	{
		needCurveFrames = new Dictionary<int, float>{{0,UnityEngine.Random.Range(0f,100f)},{1,UnityEngine.Random.Range(0f,100f)},{2,UnityEngine.Random.Range(0f,100f)},{3,UnityEngine.Random.Range(0f,100f)},{4,UnityEngine.Random.Range(0f,100f)}};
		gameObject.name = GetInstanceID ().ToString ();
	}
	void Start(){}
	void Update()
	{
		if (Time.time >= nexttick)
		{
			nexttick = Time.time + 2f;
			foreach (int k in needCurveFrames.Keys) {
				needCurveFrames [k] = Mathf.Clamp (needCurveFrames [k] + 1f, 0f, 100f);
			}
		}
	}
}
