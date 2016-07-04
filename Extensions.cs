using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor.Animations;
using System;
using UnityEditor;

public static class AnimationClipExtensions
{
	public static void ModifyClipCurve(this AnimationClip clip,EditorCurveBinding binding, float time, float value)
	{
		var curve = AnimationUtility.GetEditorCurve (clip, binding);
		if (curve != null) 
		{
			var keys = curve.keys.ToList();
			if (keys.ExtractProps (k => k.time).Contains (time)) 
			{
				keys [keys.IndexOf (keys.Single (k => k.time == time))] = new Keyframe (time, value);
			} 
			else 
			{
				keys.Add (new Keyframe (time, value));
			}
			AnimationUtility.SetEditorCurve(clip,binding,new AnimationCurve(keys.ToArray()));
		}
	}
	public static void SetLength(this AnimationClip clip,float length)
	{
		var bindings = AnimationUtility.GetCurveBindings(clip);
		AnimationUtility.GetAllCurves(clip,true).ToList ().ForEach (cd => 
		{
				if(cd.curve.length < length)
				{
					var lastkey = cd.curve.keys.Single (k => k.time == cd.curve.keys.ExtractProps (_k => _k.time).Max ());
					var newkeys = cd.curve.keys.ToList ();
					newkeys [newkeys.IndexOf (lastkey)] = new Keyframe (length, lastkey.value);
					AnimationUtility.SetEditorCurve(clip,bindings.Single(b => b.propertyName == cd.propertyName),new AnimationCurve(newkeys.ToArray()));
				}
				if (cd.curve.length > length){
					var newkeys = cd.curve.keys.Where (k => k.time <= length).ToList ();
					var lastkey = newkeys.Single (k => k.time == newkeys.ExtractProps (_k => _k.time).Max ());
					newkeys [newkeys.IndexOf (lastkey)] = new Keyframe (length,lastkey.value);
					AnimationUtility.SetEditorCurve(clip,bindings.Single(b => b.propertyName == cd.propertyName),new AnimationCurve(newkeys.ToArray()));
				} 
		});
	}
	public static AnimationClipPair ClipPair(this AnimationClip overideClip, AnimationClip originalClip)
	{
		var ret = new AnimationClipPair ();
		ret.originalClip = originalClip;
		ret.overrideClip = overideClip;
		return ret;
	}
}

public static class VectorExtenstions
{
	public static Bounds bounds(this Vector3 pos, float size){return new Bounds (pos, new Vector3(size,size,size));}
	public static List<float> ToList(this Vector3 v){return new List<float>{v.x, v.y, v.z };}
}

public static class GameObjectExtensions
{
	public static GameObject AddChild(this GameObject go,GameObject org,string name, Vector3 pos, Quaternion rot)
	{
		if (go.transform.FindChild (name) != null)
		{	
			GameObject.DestroyImmediate (go.transform.FindChild(name).gameObject);
		}
		var ret = (GameObject)GameObject.Instantiate(org, pos, rot);
		ret.name = name;
		ret.transform.parent = go.transform;
		return ret;
	}
		
}

public static class CollectionExtensions
{
	public static IEnumerable<T2> ExtractProps<T1,T2>(this IEnumerable<T1> coll,Func<T1,T2> extractor)
	{
		var ret = new List<T2>();
		coll.ToList ().ForEach (el => ret.Add (extractor (el)));
		return ret;
	}
	
}

public static class StringExtensions
{
	public static bool hassubstring(this string str, params string[] substrings)
	{
		bool ret = false;
		foreach (string substr in substrings) {
			if (str.Contains (substr)) {
				ret = true;
				break;
			}
		}
		return ret;
	}
}

public static class NumberExtensions
{
	public static int[] toArray(this int i){return Enumerable.Range (0, i).ToArray ();}
	public static int[] toArray(this int i,int start, int end){return Enumerable.Range (start, end).ToArray ();}
	public static int[] toArray(this int i,Func<int,int> transducer){return Enumerable.Repeat (transducer (i), i).ToArray ();}
	public static int toEven(this int v){return v + (v % 2);}
	public static int toOdd (this int v){return (v % 2 == 0) ? v + 1 : v;}
	public static int NegHalf(this int v, int div){return (v >= 0) ? v - (div.toEven () / 2) : v + (- div.toEven () / 2);}
	public static int PosHalf(this int v, int div){return v + div.toEven() / 2;}
	public static float percentage(this float v, float min,float max){return (Mathf.Clamp(v,min,max) - min) / (max - min);}
}



//AnimationUtility.GetAllCurves (editclip,true).ToList ().ForEach(cd =>
//	{
//		var b = musclebindings.Single (_b => _b.propertyName == cd.propertyName);
//		if (cd.curve.length < editClipLengthSlider) {
//			var lastkey = cd.curve.keys.Single (k => k.time == cd.curve.keys.ExtractProps (_k => _k.time).Max ());
//			var newkeys = cd.curve.keys.ToList ();
//			newkeys [newkeys.IndexOf (lastkey)] = new Keyframe (editClipLengthSlider, lastkey.value);
//			AnimationUtility.SetEditorCurve (editclip, b, new AnimationCurve (newkeys.ToArray ()));
//		}
//		if (cd.curve.length > editClipLengthSlider) {
//			var newkeys = cd.curve.keys.Where (k => k.time <= editClipLengthSlider).ToList ();
//			var lastkey = newkeys.Single (k => k.time == newkeys.ExtractProps (_k => _k.time).Max ());
//			newkeys [newkeys.IndexOf (lastkey)] = new Keyframe (editClipLengthSlider, lastkey.value);
//			AnimationUtility.SetEditorCurve (editclip, b, new AnimationCurve (newkeys.ToArray ()));
//		} 
//	});
//editclip.SampleAnimation (transform.FindChild ("npcpreview").gameObject, editTimeSlider);
//muscleSliders = getMuscleValues (editClipSelector, bodyPartSelector, editTimeSlider);