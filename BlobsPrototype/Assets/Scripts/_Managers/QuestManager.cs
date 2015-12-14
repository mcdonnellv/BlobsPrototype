using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestManager : MonoBehaviour {
	private static QuestManager _questManager;
	public static QuestManager questManager { get {if(_questManager == null) _questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>(); return _questManager; } }

	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
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


	public void CollectRewardsForQuest(Quest quest) {
		hudManager.lootMenu.Show(quest);
		foreach(int blobId in quest.blobIds) {
			if(blobId == -1)
				continue;
			Blob blob = roomManager.GetBlobByID(blobId);
			blob.ActionDone();
		}
		availableQuests.Remove(quest);
	}


	public int GetRewardCount(Quest quest) {
		if(IsPartyFull(quest) == false)
			return 0;
		
		int matchCt = 0;
		for(int i=0; i<quest.blobsRequired; i++) {
			Blob blob = roomManager.GetBlobByID(quest.blobIds[i]);
			if(DoesBlobMatchSlot(quest, blob, i))
				matchCt++;
		}
		
		float ct = 2f + (1f * matchCt / quest.blobsRequired) * 3f;
		return Mathf.FloorToInt(ct);
	}


	public bool DoesBlobMatchSlot(Quest quest, Blob blob, int index) {
		bool match = true;
		if(quest.usesSigils && blob.sigil != quest.sigilRequirements[index])
			match = false;
		if(quest.usesElements && blob.element != quest.elementRequirements[index])
			match = false;
		return match;
	}

	public bool IsPartyFull(Quest quest) {
		foreach(int blobId in quest.blobIds) 
			if(blobId == -1) 
				return false;
		return true;
	}


	void Update() {
		foreach(Quest quest in availableQuests) {
			if(quest.state == QuestState.Embarked && quest.actionReadyTime <= System.DateTime.Now)
				quest.Complete();
		}
	}
}
