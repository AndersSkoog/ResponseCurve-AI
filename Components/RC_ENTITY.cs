using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;



public class RC_ENTITY : RC_COMPONENT 
{
	public static Mediator mediator = mediators.affectiveMediator;
	
	public void makeBooking(string targetID,string npcID)
	{
		mediator.makeBooking (targetID, npcID);
	}
	public void startInteraction(string targetID,string npcID)
	{
		mediator.startInteraction (targetID, npcID);
	}
	public void endInteraction(string targetID,string npcID)
	{
		mediator.endInteraction (targetID, npcID);
	}
	public void advertise(string targetID,int needIndex,float needGain)
	{
		mediator.advertise (targetID, needIndex, needGain);
	}
	public void evalAds(string npcID){
		mediator.evalAds (npcID);
	}
}

