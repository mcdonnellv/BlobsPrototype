using UnityEngine;
using System;
using System.Collections;

public class CheatMenu : GenericGameMenu {

	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	CombatManager combatManager { get { return CombatManager.combatManager; } }

	public void Pressed() {	base.Show(); }

	public void AddGold() { hudManager.UpdateGold(gameManager.gameVars.AddGold(1000)); }

	public void IncrementMissionCount() { 
		foreach(Blob blob in roomManager.currentRoom.blobs)
			if(blob.hasHatched)
				blob.gameObject.ReturnFromQuest();
	}

	public void FinishQuests() { 
		foreach(Quest quest in QuestManager.questManager.availableQuests)
			quest.actionReadyTime = System.DateTime.Now + new TimeSpan(0,0,2);
	}

	public void StartMockFight() {
		gameManager.roomCamera.gameObject.SetActive(false);
		combatManager.AddActor(RoomManager.roomManager.currentRoom.blobs[0]);
		combatManager.AddActor(RoomManager.roomManager.currentRoom.blobs[1]);
		combatManager.AddActor(RoomManager.roomManager.currentRoom.blobs[2]);
		
		BaseMonster bm = MonsterManager.monsterManager.GetBaseMonsterByID(0);
		Monster m = new Monster(bm);
		combatManager.AddActor(m);
		combatManager.StartFight();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
