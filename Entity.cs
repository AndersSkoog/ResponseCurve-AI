using G = Globals;

public class Entity : AbstractComponent
{
	public void startAdvertisment(string targetName){
		G.mediatorInstance.startAdvertisement (targetName);
	}
	public void startInteraction(string targetName, string npcName){
		G.mediatorInstance.startInteraction (targetName, npcName);
	}
	public void attendTarget(string targetName, string npcName){
		G.mediatorInstance.attendTarget (targetName, npcName);
	}
	public void evaluateAdvertisements(string npcName){
		G.mediatorInstance.evaluateAdvertisements (npcName);
	}
}