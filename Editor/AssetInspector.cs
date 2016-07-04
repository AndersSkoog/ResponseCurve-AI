using UnityEditor;

[CustomEditor(typeof(AbstractAsset),true)]
[CanEditMultipleObjects]
public class AssetInspector : Editor 
{
	public AbstractAsset Target {get {return (AbstractAsset)target;}}
	public override void OnInspectorGUI(){Target.ON_INSPECTOR_GUI();DrawDefaultInspector ();}
}
