using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using G = Globals;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Need : AbstractAsset 
{
	[HideInInspector]
	public float slopeTime = 1f;
	[HideInInspector]
	public float slopeValue = 99f;
	public AnimationCurve needCurve
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{new Keyframe(0f,100f),new Keyframe(slopeTime,slopeValue),new Keyframe(100f,0f)});
		}
	}
	public int needIndex
	{
		get{return G.needs.ToList ().IndexOf(this);}
	}
	public float evaluate(float time)
	{
		var t = Mathf.Clamp (time, 0f, 100f);
		return needCurve.Evaluate (t);
	}
	[HideInInspector]
	public AnimationCurve guicurve;
	#if UNITY_EDITOR
	public override void ON_INSPECTOR_GUI ()
	{
		guicurve = EditorGUILayout.CurveField (guicurve, GUILayout.Height (100f));
		EditorGUI.BeginChangeCheck ();
		slopeTime = EditorGUILayout.Slider("slope point time",slopeTime,1f, 99f);
		slopeValue = EditorGUILayout.Slider("slope point value",slopeValue,1f, 99f);
		if(EditorGUI.EndChangeCheck())
		{
			guicurve = needCurve;
		}		
	}
	[MenuItem("Assets/Create/Need")]
	static void create()
	{
		var folder = "Assets/Resources/Needs/";
		var p = EditorUtility.SaveFilePanelInProject ("", "", "asset", "", folder);
		Need inst = ScriptableObject.CreateInstance<Need>();
		AssetDatabase.CreateAsset (inst, p);
		AssetDatabase.SaveAssets();
	}
	#endif
}
