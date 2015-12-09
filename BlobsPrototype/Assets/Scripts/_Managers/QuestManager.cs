using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestManager : MonoBehaviour {
	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager _roomManager;
	RoomManager roomManager { get {if(_roomManager == null) _roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>(); return _roomManager; } }
	public List<BaseQuest> quests = new List<BaseQuest>(); // All quests
	public List<Quest> availableQuests = new List<Quest>(); //current quests
	public UIAtlas iconAtlas;

	public bool DoesNameExistInList(string nameParam){return (GetBaseQuestWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseQuestByID(idParam) != null); }

	public BaseQuest GetBaseQuestWithName(string nameParam) {
		foreach(BaseQuest i in quests)
			if (i.itemName == nameParam)
				return i;
		return null;
	}
	
	public BaseQuest GetBaseQuestByID(int idParam) {
		foreach(BaseQuest i in quests)
			if (i.id == idParam)
				return i;
		return null;
	}

	public int GetNextAvailableID() {
		int lowestIdVal = 0;
		List<BaseQuest> sortedByID = quests.OrderBy(x => x.id).ToList();
		foreach(BaseQuest i in sortedByID)
			if (i.id == lowestIdVal)
				lowestIdVal++;
		return lowestIdVal;
	}

	public void FirstTimeSetup() {
		foreach(BaseQuest q in quests)
			AddQuestToList(q);
	}

	public void AddQuestToList(BaseQuest bq) {
		availableQuests.Add(new Quest(bq));
	}


	public void QuestCompleted(Quest quest) {
		hudManager.lootMenu.Show(quest);
		foreach(int blobId in quest.blobIds) {
			Blob blob = roomManager.GetBlobByID(blobId);
			blob.ActionDone();
		}
		availableQuests.Remove(quest);
	}


	void Update() {
		foreach(Quest quest in availableQuests) {
			if(quest.state == QuestState.Embarked && quest.actionReadyTime <= System.DateTime.Now)
				quest.Complete();
		}
	}
}
