using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using System;

public class RC_SMARTOBJECT : RC_COMPONENT 
{
	
	public AnimationClip enterclip;
	public AnimationClip mainclip;
	public AnimationClip exitclip;





	void Awake()
	{
		Destroy(transform.FindChild ("npcpreview").gameObject);
	}


	public readonly static npcprefabpath = "";
	public readonly static GameObject npcprefab = (GameObject)AssetDatabase.LoadAssetAtPath(npcprefabpath,typeof(GameObject));
	public readonly static EditorCurveBinding[] musclebindings = AnimationUtility
		.GetAnimatableBindings (npcprefab, npcprefab)
		.Where (b => new string[] {"RootT.x","RootT.y","RootT.z","RootQ.x","RootQ.y","RootQ.z"}.Contains (b.propertyName) || HumanTrait.MuscleName.Contains (b.propertyName)).ToArray ();
	public readonly static Dictionary<string,List<EditorCurveBinding>> bodypartmap = new Dictionary<string, List<EditorCurveBinding>>
	{
				{"root",musclebindings.Where (b => new List<string>{ "RootT.x", "RootT.y", "RootT.z", "RootQ.x", "RootQ.y", "RootQ.z" }.Contains (b.propertyName)).ToList ()},
				{"body",musclebindings.Where (b => b.propertyName.hassubstring ("Chest", "Spine")).ToList () },
				{"head",musclebindings.Where (b => b.propertyName.hassubstring ("Head", "Neck", "Eye", "Jaw")).ToList () },
				{"leftarm",musclebindings.Where (b => b.propertyName.hassubstring ("Left Arm", "Left Shoulder", "Left Forearm", "Left Hand")).ToList ()},
				{"rightarm",musclebindings.Where (b => b.propertyName.hassubstring ("Right Arm", "Right Shoulder", "Right Forearm", "Right Hand")).ToList ()},
				{"leftleg",musclebindings.Where (b => b.propertyName.hassubstring ("Left Upper Leg", "Left Lower Leg", "Left Foot", "Left Toes")).ToList ()},
				{"rightleg",musclebindings.Where (b => b.propertyName.hassubstring ("Right Upper Leg", "Right Lower Leg", "Right Foot", "Right Toes")).ToList ()}
	};
	public readonly static string[] bodypartmapkeys = new string[]{"root","body","head","leftarm","rightarm","leftleg","rightleg"};
	/// <summary>
	/// GUI HELPERS;
	/// </summary>
	private float[] getMuscleValues(int clipindex,int bodypartindex,float time)
	{
		var bindings = bodypartmap [bodypartmapkeys[bodyPartSelector]].ExtractProps(b => b.propertyName);
		var clip = new []{ enterclip, mainclip, exitclip }[clipindex];
		var curves = AnimationUtility.GetAllCurves (clip, true).Where (b => bindings.Contains (b.propertyName)).ExtractProps (cd => cd.curve);
		return curves.ExtractProps (c => c.Evaluate(time)).ToArray();
	}
	private void changeclipLength()
	{
		AnimationUtility.GetAllCurves (editclip,true).ToList ().ForEach(cd =>
		{
				var b = musclebindings.Single (_b => _b.propertyName == cd.propertyName);
			if (cd.curve.length < editClipLengthSlider) {
				var lastkey = cd.curve.keys.Single (k => k.time == cd.curve.keys.ExtractProps (_k => _k.time).Max ());
				var newkeys = cd.curve.keys.ToList ();
				newkeys [newkeys.IndexOf (lastkey)] = new Keyframe (editClipLengthSlider, lastkey.value);
				AnimationUtility.SetEditorCurve (editclip, b, new AnimationCurve (newkeys.ToArray ()));
			}
			if (cd.curve.length > editClipLengthSlider) {
				var newkeys = cd.curve.keys.Where (k => k.time <= editClipLengthSlider).ToList ();
				var lastkey = newkeys.Single (k => k.time == newkeys.ExtractProps (_k => _k.time).Max ());
				newkeys [newkeys.IndexOf (lastkey)] = new Keyframe (editClipLengthSlider, lastkey.value);
				AnimationUtility.SetEditorCurve (editclip, b, new AnimationCurve (newkeys.ToArray ()));
			} 
		});
		editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject, editTimeSlider);
		muscleSliders = getMuscleValues (editClipSelector, bodyPartSelector, editTimeSlider);
	}
	private void copymirrorside(List<EditorCurveBinding> mirrorbindings)
	{
		foreach (int i in Enumerable.Range(0,muscleSliders.Length)) {
			editclip.ModifyClipCurve(mirrorbindings[i],editTimeSlider,muscleSliders[i]);
		}
		editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject,editTimeSlider);
	}

	/// <summary>
	/// GUI FIELDS;
	/// </summary>
	private bool editToggle;
	private int bodyPartSelector;
	private int editClipSelector;
	private int copyClipSelector;
	private float editTimeSlider;
	private float copyTimeSlider;
	private float editClipLengthSlider;
	private float[] muscleSliders;
	private bool hasclips {get{return mainclip != null && enterclip != null && exitclip != null;}}
	private AnimationClip editclip {get{return new[]{ enterclip, mainclip, exitclip } [editClipSelector];}}
	/// <summary>
	/// GUI;
	/// </summary>
	public override void ON_INSPECTOR_GUI ()
	{
		if (!EditorApplication.isPlayingOrWillChangePlaymode && hasclips)
		{
			if (transform.FindChild ("npcpreview") == null){
				gameObject.AddChild (npcprefab, "npcpreview", transform.position, Quaternion.identity);
			}
			EditorGUILayout.LabelField ("EDITCLIP");
			editClipSelector = GUILayout.SelectionGrid (editClipSelector, new string[]{ "enter", "main", "exit" }, 3);
			EditorGUI.BeginChangeCheck ();
			editTimeSlider = EditorGUILayout.Slider ("EditTime", editTimeSlider, 0, editclip.length);
			if (EditorGUI.EndChangeCheck()) {
				editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject,editTimeSlider);
				muscleSliders = getMuscleValues(editClipSelector,bodyPartSelector,editTimeSlider);
			}
			EditorGUI.BeginChangeCheck ();
			editClipLengthSlider = EditorGUILayout.Slider ("EditClipLength", editClipLengthSlider, 1f, 100f);
			if (EditorGUI.EndChangeCheck ())
			{
				changeclipLength();
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("COPYCLIP");
			copyClipSelector = GUILayout.SelectionGrid (copyClipSelector, new string[]{ "enter", "main", "exit" }, 3);
			copyTimeSlider = EditorGUILayout.Slider ("CopyTime", copyTimeSlider, 0, editclip.length);
			if (GUILayout.Button ("paste from copy time",GUILayout.Width(200f),GUILayout.Height(20f)))
			{
				var copyclip = new[]{ enterclip, mainclip, exitclip } [copyClipSelector];
				foreach (EditorCurveBinding b in musclebindings)
				{
					editclip.ModifyClipCurve(b, editTimeSlider, AnimationUtility.GetEditorCurve (copyclip, b).Evaluate (copyTimeSlider));
					editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject,editTimeSlider);
					muscleSliders = getMuscleValues(editClipSelector,bodyPartSelector,editTimeSlider);
				}
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUI.BeginChangeCheck ();
			bodyPartSelector = GUILayout.SelectionGrid (bodyPartSelector, bodypartmapkeys, 7);
			if (EditorGUI.EndChangeCheck ()) {
				muscleSliders = getMuscleValues (editClipSelector,bodyPartSelector, editTimeSlider);
			}
			foreach(int i in Enumerable.Range(0,muscleSliders.Length)){
				EditorGUI.BeginChangeCheck ();
				muscleSliders[i] = EditorGUILayout.Slider(bodypartmap[bodypartmapkeys[bodyPartSelector]].ElementAt(i).propertyName,muscleSliders [i], -2f, 2f);
				if (EditorGUI.EndChangeCheck ()) {
					editclip.ModifyClipCurve(bodypartmap [bodypartmapkeys [bodyPartSelector]].ElementAt (i), editTimeSlider, muscleSliders [i]);		
					editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject, editTimeSlider);
					muscleSliders = getMuscleValues (editClipSelector, bodyPartSelector, editTimeSlider);
				}
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			if (new[]{ 3, 4, 5, 6 }.Contains(bodyPartSelector)) {
				if (GUILayout.Button ("copy mirror side",GUILayout.Width(100f),GUILayout.Height(20f))) {
					switch (bodyPartSelector) {
					case 3:
						copymirrorside (bodypartmap ["rightarm"]);
						break;
					case 4:
						copymirrorside (bodypartmap ["leftarm"]);
						break;
					case 5:
						copymirrorside (bodypartmap ["rightleg"]);
						break;
					case 6:
						copymirrorside (bodypartmap ["leftleg"]);
						break;
					default:
						break;
					}
				}
			}
			if (GUILayout.Button ("clear",GUILayout.Width(60f),GUILayout.Height(20f))) {
				foreach (EditorCurveBinding b in musclebindings) {
					AnimationUtility.SetEditorCurve (editclip, b, new AnimationCurve (new Keyframe[] {
						new Keyframe (0f, 0f),
						new Keyframe (editClipLengthSlider,0f)
					}));
				}
				editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject, 0f);
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}
	}



}