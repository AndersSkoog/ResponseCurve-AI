using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class HumanoidAnimationEditor : EditorWindow 
{
	public int bodypartSelect;
	public float clipLengthField;
	public float editTimeSlider;
	public float copyTimeSlider;
	public float[] muscleSliders;
	public bool validTargetSelected
	{
		get
		{
			var go = Selection.activeGameObject;
			var anim = go.GetComponent<Animator> ();
			var avatar = anim.avatar;
			var nullcheck = new object[]{ go, anim, avatar }.Any (obj => obj == null);
			return (nullcheck == false) ? avatar.isHuman : false;
		}
	}
	private AnimationClip clip;
	private GameObject humanoidTarget;
	private string clipCreationNameField;
	private float clipCreationLengthField;
	private bool posefoldout;
	private string posesavename;
	private int poseloadpopup;

	[MenuItem ("Window/HumanoidAnimationEditor")]
	static void Init ()
	{
		HumanoidAnimationEditor window = (HumanoidAnimationEditor)EditorWindow.GetWindow (typeof(HumanoidAnimationEditor));
		window.Show();
	}
	void OnGUI()
	{
		if (validTargetSelected) {
			EditorGUI.BeginChangeCheck ();
			clip = (AnimationClip)EditorGUILayout.ObjectField ("Clip", clip, (typeof(AnimationClip)), false);
			if (EditorGUI.EndChangeCheck ()) {
				if (clip != null) {
					applyChange ();
				}
			}
			if (clip != null) {
				if (muscleSliders != null) {
					EditorGUI.BeginChangeCheck ();
					clipLengthField = EditorGUILayout.FloatField ("ClipLength", clipLengthField);
					if (EditorGUI.EndChangeCheck ()) {
						clip.SetLength (clipLengthField);
						applyChange ();
					}
					EditorGUI.BeginChangeCheck ();
					editTimeSlider = EditorGUILayout.Slider ("EditTime", editTimeSlider, 0, clip.length);
					if (EditorGUI.EndChangeCheck ()) {
						applyChange ();
					}
					copyTimeSlider = EditorGUILayout.Slider ("CopyTime", copyTimeSlider, 0, clip.length);
					if (GUILayout.Button ("Paste", GUILayout.Width (100f), GUILayout.Height (20f))) {
						foreach (EditorCurveBinding b in musclebindings) {
							clip.ModifyClipCurve (b, editTimeSlider, AnimationUtility.GetEditorCurve (clip, b).Evaluate (copyTimeSlider));
						}
						applyChange ();
					}
					EditorGUI.BeginChangeCheck ();
					bodypartSelect = GUILayout.SelectionGrid (bodypartSelect, bodypartmapkeys, 7);
					if (EditorGUI.EndChangeCheck ()) {
						muscleSliders = getMuscleValues (clip, bodypartSelect, editTimeSlider);
					}
					foreach (int i in Enumerable.Range(0,muscleSliders.Length)) {
						EditorGUI.BeginChangeCheck ();
						muscleSliders [i] = EditorGUILayout.Slider (bodypartmap [bodypartmapkeys [bodypartSelect]].ElementAt (i).propertyName, muscleSliders [i], -2f, 2f);
						if (EditorGUI.EndChangeCheck ()) {
							clip.ModifyClipCurve (bodypartmap [bodypartmapkeys [bodypartSelect]].ElementAt (i), editTimeSlider, muscleSliders [i]);
							applyChange ();
						}
					}
					posefoldout = EditorGUILayout.Foldout (posefoldout, "poses");
					if (posefoldout) {
						poseloadpopup = EditorGUILayout.Popup (poseloadpopup, posestorage.poses.Keys.ToArray ());	
						posesavename = EditorGUILayout.TextField (posesavename);
						if (GUILayout.Button ("loadpose")) {
							posestorage.poses [posestorage.poses.Keys.ToArray () [poseloadpopup]].ToList ().ForEach (kv => {
								clip.ModifyClipCurve (kv.Key, editTimeSlider, kv.Value);
							});
							applyChange ();
						}
						if (GUILayout.Button ("savepose")) {
							posestorage.poses.Add (posesavename, musclebindings.ToDictionary (k => k, v => AnimationUtility.GetEditorCurve (clip, v).Evaluate (editTimeSlider)));
						}
					}
					if (new[]{ 3, 4, 5, 6 }.Contains (bodypartSelect)) {
						if (GUILayout.Button ("copy mirror side", GUILayout.Width (100f), GUILayout.Height (20f))) {
							switch (bodypartSelect) {
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
					if (GUILayout.Button ("clear", GUILayout.Width (60f), GUILayout.Height (20f))) {
						foreach (EditorCurveBinding b in musclebindings) {
							AnimationUtility.SetEditorCurve (clip, b, new AnimationCurve (new Keyframe[] {
								new Keyframe (0f, 0f),
								new Keyframe (clip.length, 0f)
							}));
						}
						applyChange ();
					}
					if (GUILayout.Button ("Save As"))
					{
						var path = EditorUtility.SaveFilePanelInProject ("", "new clip", "anim", "", "Assets/Resources/Animations");
						var curvedata = AnimationUtility.GetAllCurves (clip, true);
						var newclip = new AnimationClip ();
						foreach (AnimationClipCurveData cd in curvedata) {
							AnimationUtility.SetEditorCurve (newclip, musclebindings.Single (b => b.propertyName == cd.propertyName), cd.curve);
						}
						AssetDatabase.CreateAsset(newclip,path);
						AssetDatabase.SaveAssets();
					}
				} 
				else
				{
					muscleSliders = getMuscleValues (clip, bodypartSelect, editTimeSlider);
				}
			} 
			else 
			{
				EditorGUILayout.LabelField ("Create New Animation");
				clipCreationNameField = EditorGUILayout.TextField ("clipname", clipCreationNameField);
				clipCreationLengthField = EditorGUILayout.FloatField ("cliplength", clipCreationLengthField);
				if (GUILayout.Button ("create")) {
					clip = createHumanMotionClip (clipCreationNameField, clipCreationLengthField);
					applyChange ();
				}
			}
		} 
		else 
		{
			EditorGUILayout.HelpBox ("select a humanoid gameobject in the scene", MessageType.Info);
		}
	}
	private AnimationClip createHumanMotionClip(string name,float length)
	{
		var ret = new AnimationClip();
		ret.name = name;
		ret.legacy = false;
		foreach (EditorCurveBinding b in musclebindings)
		{
			AnimationUtility.SetEditorCurve(ret,b,new AnimationCurve(new Keyframe[]{new Keyframe(0f,0f),new Keyframe(length,0f)}));
		}
		AssetDatabase.CreateAsset (ret,"Assets/Resources/Animations/" + name + ".anim");
		AssetDatabase.SaveAssets();
		return ret;
	}
	private void copymirrorside(List<EditorCurveBinding> mirrorbindings)
	{
		foreach (int i in Enumerable.Range(0,muscleSliders.Length)) {
			clip.ModifyClipCurve(mirrorbindings[i],editTimeSlider,muscleSliders[i]);
		}
		applyChange();
	}
	private void applyChange()
	{
		clip.SampleAnimation(Selection.activeGameObject,editTimeSlider);
		muscleSliders = getMuscleValues (clip, bodypartSelect, editTimeSlider);
	}
	public float[] getMuscleValues(AnimationClip clip,int bodypartindex,float sampleTime)
	{
		var bindings = bodypartmap [bodypartmapkeys [bodypartindex]].ExtractProps (b => b.propertyName);
		var curves = AnimationUtility.GetAllCurves (clip, true).Where (b => bindings.Contains (b.propertyName)).ExtractProps (cd => cd.curve);
		return curves.ExtractProps (c => c.Evaluate(Mathf.Clamp(sampleTime,0f,c.length))).ToArray();
	}
	private static GameObject npcprefab = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/Resources/Prefabs/Npc.prefab", typeof(GameObject));
	private static EditorCurveBinding[] musclebindings = AnimationUtility
		.GetAnimatableBindings (npcprefab, npcprefab)
		.Where (b => new string[] {"RootT.x","RootT.y","RootT.z","RootQ.x","RootQ.y","RootQ.z"}
			.Contains (b.propertyName) || HumanTrait.MuscleName.Contains (b.propertyName)).ToArray ();
	private static Dictionary<string,List<EditorCurveBinding>> bodypartmap = new Dictionary<string, List<EditorCurveBinding>>
	{
		{"root",musclebindings.Where (b => new List<string>{ "RootT.x", "RootT.y", "RootT.z", "RootQ.x", "RootQ.y", "RootQ.z" }.Contains (b.propertyName)).ToList ()},
		{"body",musclebindings.Where (b => b.propertyName.hassubstring ("Chest", "Spine")).ToList () },
		{"head",musclebindings.Where (b => b.propertyName.hassubstring ("Head", "Neck", "Eye", "Jaw")).ToList () },
		{"leftarm",musclebindings.Where (b => b.propertyName.hassubstring ("Left Arm", "Left Shoulder", "Left Forearm", "Left Hand")).ToList ()},
		{"rightarm",musclebindings.Where (b => b.propertyName.hassubstring ("Right Arm", "Right Shoulder", "Right Forearm", "Right Hand")).ToList ()},
		{"leftleg",musclebindings.Where (b => b.propertyName.hassubstring ("Left Upper Leg", "Left Lower Leg", "Left Foot", "Left Toes")).ToList ()},
		{"rightleg",musclebindings.Where (b => b.propertyName.hassubstring ("Right Upper Leg", "Right Lower Leg", "Right Foot", "Right Toes")).ToList ()}
	};
	private static string[] bodypartmapkeys = new string[]{"root","body","head","leftarm","rightarm","leftleg","rightleg"};
	private static PoseStorage posestorage = (PoseStorage)AssetDatabase.LoadAssetAtPath ("Assets/Resources/PoseStorage.asset", typeof(PoseStorage));
}
