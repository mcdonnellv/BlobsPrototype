using UnityEngine;
using System;
using System.Collections;

public class CheatMenu : GenericGameMenu {

	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }

	public void Pressed() {	base.Show(); }

	public void AddGold() { hudManager.UpdateGold(gameManager.gameVars.AddGold(1000)); }

	public void IncrementMissionCount() { 
		foreach(Blob blob in roomManager.currentRoom.blobs)
			if(blob.hasHatched)
				blob.ReturnFromQuest();
	}

	public void FinishQuests() { 
		foreach(Quest quest in QuestManager.questManager.availableQuests)
			quest.actionReadyTime = System.DateTime.Now + new TimeSpan(0,0,2);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
