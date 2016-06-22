using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;





[RequireComponent(typeof(RC_SMARTOBJECT))]
public class RC_TARGET : RC_ENTITY 
{
	[HideInInspector]	
	public int needIndex;
	[HideInInspector]
	public float needGain;
	[HideInInspector]
	public float area;
	public RC_SMARTOBJECT smartobj { get { return GetComponent<RC_SMARTOBJECT> (); } }
	void Awake()
	{
		gameObject.name = GetInstanceID ().ToString();
		advertise (gameObject.name, this.needIndex, this.needGain);
	}
	public override void ON_INSPECTOR_GUI ()
	{
		needIndex = EditorGUILayout.Popup ("need",needIndex,RC_NEED.needs().ExtractProps(n => n.name).ToArray());
		needGain = EditorGUILayout.Slider ("gain", needGain, 10f, 100f);
		area = EditorGUILayout.Slider ("area", area, 1f, 10f);
	}
	public AnimationClip[] interactionclips(){
		var sobj = GetComponent<RC_SMARTOBJECT> ();
		return new AnimationClip[]{ sobj.enterclip, sobj.mainclip, sobj.exitclip };
	}

	public IEnumerator routine(Func<bool> exitpred,Action loopaction,Action onexit)
	{
		bool isrunning = true;
		while (isrunning) 
		{
			if (exitpred ()) {
				isrunning = false;
				onexit ();
			} 
			else 
			{
				loopaction();
			}
			yield return null;
		}
	}
}