using UnityEngine;
using System.Collections;
using System.Linq;
using G = Globals;
public class Advertisement
{
	public Target target {get; private set;}
	public float getMotivationScore(Npc npc)
	{
		var framedict = npc.needCurveFrames.Keys.ToDictionary (k => k, v => (target.asociatedNeed == G.needs[v]) ? npc.needCurveFrames [v] - target.needGain : npc.needCurveFrames [v]);
		var sum = framedict.ExtractProps (kv => G.needs [kv.Key].evaluate (kv.Value)).Sum ();
		var total = (100 * sum) / G.maxNeedSum;
		return Mathf.Abs (total - npc.needTotal);
	}
	public Advertisement(string targetName)
	{
		target = GameObject.Find(targetName).GetComponent<Target> ();
	}
}

