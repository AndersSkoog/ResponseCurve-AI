using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using G = Globals;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Npc : Entity 
{
	public float nexttick;
	public NavMeshAgent agent {get{return GetComponent<NavMeshAgent>();}}
	public Animator animator {get{return GetComponent<Animator> ();}}
	public Dictionary<int,float> needCurveFrames;
	public Dictionary<int,float> needCurveValues
	{
		get
		{
			return needCurveFrames.ToDictionary ((k) => k.Key, (v) => G.needs[v.Key].evaluate (v.Value));	
		}
	}
	public float needTotal
	{
		get 
		{
			return (100 * needCurveValues.ExtractProps(kv => kv.Value).Sum()) / G.maxNeedSum;	
		}
	}
	public string highestNeed
	{
		get 
		{
			var ind = needCurveValues.Single (kv => kv.Value == needCurveValues.Values.ToArray ().Min ()).Key;
			return G.needs [ind].name;
		}
	}
	public Target socialTarget{get{return transform.GetComponentInChildren<Target>();}}
	public static int needDecreaseRate = 1;

	void Awake()
	{
		needCurveFrames = G.needs.Length.toArray().ToDictionary ((k) => k, (v) => UnityEngine.Random.Range (0f, 100f));
		gameObject.name = GetInstanceID ().ToString ();
	}
	void Start()
	{
		evaluateAdvertisements (gameObject.name);
	}
	void Update()
	{
		if (Time.time >= nexttick)
		{
			nexttick += Time.time + needDecreaseRate;
		}
	}
	#if UNITY_EDITOR
	public override void ON_INSPECTOR_GUI ()
	{
		if (EditorApplication.isPlaying) {
			EditorGUILayout.LabelField ("highest need:" + highestNeed);
			needCurveValues.ToList ().ForEach ((kv) => {
				var need = G.needs [kv.Key];
				EditorGUILayout.LabelField (need.name + " level: " + kv.Value.ToString ());
			});
			EditorGUILayout.LabelField ("total:" + needTotal.ToString ());
		}
	}
	#endif
	public void incNeed(int index)
	{
		if(needCurveFrames[index] <= 95f){ needCurveFrames [index] += 5.0f;}
	}
	public static AnimatorOverrideController NpcOverideController(AnimationClip enter,AnimationClip main, AnimationClip exit)
	{
		var ret = new AnimatorOverrideController ();
		ret.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load ("Controllers/DefaultController");
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
			(AnimationClip)Resources.Load("Animations/Overide/enter",typeof(AnimationClip)),
			(AnimationClip)Resources.Load("Animations/Overide/main",typeof(AnimationClip)),
			(AnimationClip)Resources.Load("Animations/Overide/exit",typeof(AnimationClip)),
		};
	}
}
