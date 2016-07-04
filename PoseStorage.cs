using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public class PoseStorage : ScriptableObject
{
	public Dictionary<string,Dictionary<EditorCurveBinding,float>> poses = new Dictionary<string,Dictionary<EditorCurveBinding,float>>{{"default",musclebindings.ToDictionary(k => k,v => 0f)}};
	[MenuItem("Assets/Create/PoseStorage")]
	static void create()
	{
		Debug.Log (Application.dataPath + "/Resources/PoseStorage.asset");
		if (!System.IO.File.Exists (Application.dataPath + "/Resources/PoseStorage.asset"))
		{
			var inst = PoseStorage.CreateInstance<PoseStorage> ();
			//inst.poses = new Dictionary<string,Dictionary<EditorCurveBinding,float>>{{"default",musclebindings.ToDictionary(k => k,v => 0f)}};
			AssetDatabase.CreateAsset (inst, path);
			AssetDatabase.SaveAssets ();
		} 
		else 
		{
			Debug.Log ("posestorage object aldready exists");
		}
	}
	//public static PoseStorage instance(){return (PoseStorage)AssetDatabase.LoadAssetAtPath(path,typeof(PoseStorage));}
	private static GameObject npcprefab = (GameObject)Resources.Load("Prefabs/Npc");
	private static string path = "Assets/Resources/PoseStorage.asset";
	private static EditorCurveBinding[] musclebindings = AnimationUtility
		.GetAnimatableBindings (npcprefab, npcprefab)
		.Where (b => new string[] {"RootT.x","RootT.y","RootT.z","RootQ.x","RootQ.y","RootQ.z"}.Contains (b.propertyName) || HumanTrait.MuscleName.Contains (b.propertyName)).ToArray ();
}
