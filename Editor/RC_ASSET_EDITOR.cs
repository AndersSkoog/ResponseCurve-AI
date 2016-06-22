using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RC_ASSET),true)]
[CanEditMultipleObjects]
public class RC_ASSET_EDITOR : Editor 
{

	public RC_ASSET Target {get {return (RC_ASSET)target;}}
	public override void OnInspectorGUI(){Target.ON_INSPECTOR_GUI();DrawDefaultInspector ();}


}
