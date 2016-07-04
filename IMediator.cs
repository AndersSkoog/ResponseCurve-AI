using System.Collections;
using System;

public interface IMediator 
{
	void attendTarget(string targetName,string npcName);
	void startAdvertisement(string targetName);
	void evaluateAdvertisements(string npcName);
	void startInteraction (string targetName, string npcName);
	IEnumerator routine (Func<bool> exitpred, Action loopaction, Action onexit);
}
