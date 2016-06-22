using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class RC_NEED : RC_ASSET {
	[SerializeField]
	public float slopePointTime = 1f;
	[SerializeField]
	public float slopePointValue = 1f;
	public float slopePointTimeSlider {get{return slopePointTime;}set{slopePointTime = value;}}
	public float slopePointValueSlider  {get{return slopePointValue;}set{slopePointValue = value;}}
	public AnimationCurve guicurve
	{
		get 
		{
			var curvepoints = new Keyframe[]
			{
				new Keyframe (0f, 100f),
				new Keyframe (slopePointTimeSlider,slopePointValueSlider),
				new Keyframe (100f,0f)

			};
			return new AnimationCurve(curvepoints);
		}
		set
		{
			
		}
	}
	public AnimationCurve needCurve {get{return new AnimationCurve (new Keyframe[] {
			new Keyframe (0f, 100f),
			new Keyframe (slopePointTime, slopePointValue),
			new Keyframe (100f, 0f)
		});}}

	/// <summary>
	/// Shared
	/// </summary>
	public static RC_NEED[] needs()
	{
		var paths = AssetDatabase.FindAssets ("t:RC_NEED").ExtractProps (guid => AssetDatabase.GUIDToAssetPath (guid));
		//Debug.Log (paths.Count());
		return paths.ExtractProps (p => (RC_NEED)AssetDatabase.LoadAssetAtPath (p, typeof(RC_NEED))).ToArray();
	}
	public static float maxneedsum()
	{
		return 100f * (needs().Length + 1);
	}
	public static float minneedsum()
	{
		return 0f;
	}
	public static float evaluate(int needindex,float frame)
	{
		return needs ()[needindex].needCurve.Evaluate (Mathf.Clamp (frame, 0f, 100f));
	}
	//public static int needcount {get{}}

	public override void ON_INSPECTOR_GUI ()
	{
		EditorGUI.BeginChangeCheck ();
		slopePointTimeSlider = EditorGUILayout.Slider("slope point time",slopePointTimeSlider,1f, 99f);
		slopePointValueSlider = EditorGUILayout.Slider("slope point value",slopePointValueSlider,1f, 99f);
		guicurve = EditorGUILayout.CurveField (guicurve,GUILayout.Height(100f));
		if(EditorGUI.EndChangeCheck())
		{
			slopePointTime = slopePointTimeSlider;
			slopePointValue = slopePointValueSlider;
		}
	}
	private static string folder = "Assets/Standard Assets/RC/Serialized/Needs/";
	[MenuItem("Assets/Create/Need")]
	static void create()
	{
		var p = EditorUtility.SaveFilePanelInProject ("", "", "asset", "", folder);
		RC_NEED inst = ScriptableObject.CreateInstance<RC_NEED>();
		AssetDatabase.CreateAsset (inst, p);
		AssetDatabase.SaveAssets();
	}
}
