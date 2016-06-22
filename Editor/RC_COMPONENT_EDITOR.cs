using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RC_COMPONENT),true)]
[CanEditMultipleObjects]
public class RC_COMPONENT_EDITOR : Editor {
	public int a;
	public RC_COMPONENT Target {get {return (RC_COMPONENT)target;}}
	public override void OnInspectorGUI(){Target.ON_INSPECTOR_GUI();DrawDefaultInspector();}
}
