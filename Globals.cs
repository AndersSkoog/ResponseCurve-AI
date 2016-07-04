using UnityEngine;
using System.Collections;
using System.Linq;


public static class Globals
{
	public static Need[] needs
	{
		get
		{
			var ret = GameObject.FindObjectsOfType<Target> ().ExtractProps (t => t.asociatedNeed).Distinct ();
			//ret.ToList ().ForEach (_ => Debug.Log (_.name));
			return ret.ToArray ();


		}	
	}
	public static int needCount
	{
		get{return needs.Length + 1;}
	}
	public static float maxNeedSum
	{
		get
		{
			return 100f * (needs.Length + 1);
		}
	}
	public static float minneedsum
	{
		get
		{
			return 0f;
		}
	}
	public static IMediator mediatorInstance
	{
		get
		{
			if (_mediatorInstance == null)
			{
				_mediatorInstance = new ImplMediator();
			}
			return _mediatorInstance;
		}	
	}
	private static IMediator _mediatorInstance;
}