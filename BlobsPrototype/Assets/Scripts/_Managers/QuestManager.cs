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
	public Dictionary<int, int> completedQuestIds = new Dictionary<int, int>();
	public Dictionary<int, int> completedQuestInTier = new Dictionary<int, int>();
	public int currentQuestTier = 1;
	public UIAtlas iconAtlas;
	public static int maxAvailableQuests = 10;

	public bool DoesNameExistInList(string nameParam){return (GetBaseQuestWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseQuestByID(idParam) != null); }
	public void AddQuestToList(BaseQuest bq) { availableQuests.Add(new Quest(bq)); }


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


	public void CollectRewardsForQuest(Quest quest) {
		hudManager.lootMenu.Show(quest);
		foreach(int blobId in quest.blobIds) {
			if(blobId == -1)
				continue;
			Blob blob = roomManager.GetBlobByID(blobId);
			blob.CompleteQuest();
		}
		availableQuests.Remove(quest);
		if(completedQuestIds.ContainsKey(quest.id))
			completedQuestIds[quest.id]++;
		else
			completedQuestIds.Add(quest.id, 1);

		if(completedQuestInTier.ContainsKey(quest.tier))
			completedQuestInTier[quest.tier]++;
		else
			completedQuestInTier.Add(quest.tier, 1);

		if(completedQuestInTier[currentQuestTier] >= 10)  // do at least 10 quests of the current tier
			currentQuestTier++;

		if(availableQuests.Count < maxAvailableQuests)
			AddRandomQuestToList();
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


	int GetTotalQuestCountForTier(int tier) {
		int ct = 0;
		foreach(BaseQuest baseQuest in quests)
			if(tier == baseQuest.tier)
				ct++;
		return ct;
	}


	void AddRandomQuestToList() {
		// rules: you can get a quest of any tier below the tier you currenly are
		// its weightet so quests of your current tier are favored

		int[] tiers = new int[currentQuestTier];
		for(int i = 0; i < currentQuestTier; i++) {
			if(i == 0)
				tiers[0] = 100;
			else
				tiers[i] = (int) (tiers[i-1] + (100 * (1 + (i * 0.2f))));
		}

		int roll = UnityEngine.Random.Range(0, tiers[currentQuestTier-1]);
		int chosenTier = 1;
		for(int i = 0; i < currentQuestTier; i++) {
			if(roll < tiers[i])
				chosenTier = i + 1;
			roll -= tiers[i];
		}

		BaseQuest randomQuest = GetRandomQuestFromTier(chosenTier);
		if(randomQuest == null)
			return;
		AddQuestToList(randomQuest);
	}


	BaseQuest GetRandomQuestFromTier(int tier) {
		List<BaseQuest> questsOfTier = new List<BaseQuest>();
		foreach(BaseQuest baseQuest in quests)
			if(baseQuest.tier == tier)
				questsOfTier.Add(baseQuest);
		if(questsOfTier.Count <= 0)
			return null;
		int index = UnityEngine.Random.Range(0, questsOfTier.Count);
		return questsOfTier[index];
	}


	void Update() {
		foreach(Quest quest in availableQuests) {
			if(quest.state == QuestState.Embarked && quest.actionReadyTime <= System.DateTime.Now)
				quest.Complete();
		}
	}
}
