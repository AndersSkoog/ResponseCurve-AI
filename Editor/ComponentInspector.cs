using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(AbstractComponent),true)]
[CanEditMultipleObjects]
public class ComponentInspector : Editor
{
	public AbstractComponent Target {get {return (AbstractComponent)target;}}
	public override void OnInspectorGUI(){Target.ON_INSPECTOR_GUI();DrawDefaultInspector();}		
}
