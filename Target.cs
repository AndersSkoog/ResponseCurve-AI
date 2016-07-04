using UnityEngine;
using System.Linq;
using G = Globals;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Target : Entity
{
	public Need asociatedNeed;
	[HideInInspector]
	public float needGain;
	[HideInInspector]
	public float area;
	public float interactionPeriod;
	public float exitTime { get; set;}
	public int needIndex {get{return G.needs.ToList ().IndexOf (asociatedNeed);}}
	public AnimationClip enterMotion;
	public AnimationClip mainMotion;
	public AnimationClip exitMotion;
	public bool motionsAssigned {get{return (new[]{ enterMotion, mainMotion, exitMotion }.Any (m => m == null)) ? false : true;}}
	#if UNITY_EDITOR
	public override void ON_INSPECTOR_GUI()
	{
		needGain = EditorGUILayout.Slider ("gain", needGain, 10f, 100f);
		area = EditorGUILayout.Slider ("area", area, 1f, 10f);
		if (!motionsAssigned && asociatedNeed != null)
		{
			EditorGUILayout.HelpBox ("All motions must be asigned", MessageType.Error);
		}
		if (EditorApplication.isPlayingOrWillChangePlaymode && !motionsAssigned){
			EditorApplication.isPlaying = false;
		}
	}
	#endif
	void Awake()
	{
		gameObject.name = GetInstanceID ().ToString();
		if (asociatedNeed.name != "Social")
		{
			startAdvertisment(gameObject.name);
		}
	}
}
