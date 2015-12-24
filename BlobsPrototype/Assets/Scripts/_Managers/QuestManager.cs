using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RewardRange { public int max, min; }

public class QuestManager : MonoBehaviour {
	private static QuestManager _questManager;
	public static QuestManager questManager { get {if(_questManager == null) _questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>(); return _questManager; } }

	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	public List<BaseQuest> quests = new List<BaseQuest>(); // All quests
	public List<Quest> availableQuests = new List<Quest>(); //current quests
	public Dictionary<int, int> completedQuestIds = new Dictionary<int, int>();
	public Dictionary<int, int> completedQuestInZone = new Dictionary<int, int>();
	public UIAtlas iconAtlas;
	public static int maxAvailableQuests = 10;

	public bool DoesNameExistInList(string nameParam){return (GetBaseQuestWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseQuestByID(idParam) != null); }

	public Quest AddQuestToList(BaseQuest bq) {
		if(bq == null) return null;
		Quest q = new Quest(bq); 
		availableQuests.Add(q);
		if(q.type == QuestType.Scouting) {
			Zone zone = ZoneManager.zoneManager.GetZoneWithQuestID(q.id);
			if(zone != null)
				q.zoneId = zone.id;
		}
		return q; 
	}


	public BaseQuest GetBaseQuestWithName(string nameParam) {
		foreach(BaseQuest i in quests)
			if (i.itemName == nameParam)
				return i;
		return null;
	}

	
	public BaseQuest GetBaseQuestByID(int idParam) {
		if (idParam == -1) return null;
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


	public List<BaseQuest> GetQuestsOfType(QuestType qType) {
		List<BaseQuest> ret = new List<BaseQuest>();
		foreach(BaseQuest bq in quests)
			if(bq.type == qType)
				ret.Add(bq);
		return ret;
	}


	public void FirstTimeSetup() {
		//AddQuestToList(GetBaseQuestByID(100));
	}


	public void CollectRewardsForQuest(Quest quest) {

		foreach(Zone zone in ZoneManager.zoneManager.zones)
			if(zone.unlockingQuestId == quest.id && !zone.IsUnlocked())
				hudManager.popup.Show("New Zone", "Congratulations! The " + zone.itemName + " zone is now available");
		
		CompleteQuestsForBlobs(quest);
		hudManager.questListMenu.QuestRemoved(availableQuests.IndexOf(quest));
		availableQuests.Remove(quest);
		if(quest.type == QuestType.Scouting) {
			CollectRewardsForScout(quest);
			return;
		}
		if(quest.LootTableA.Count > 0 || quest.LootTableB.Count > 0)
			hudManager.lootMenu.Show(quest);
		else
			hudManager.questListMenu.RewardsCollected();


		if(completedQuestIds.ContainsKey(quest.id))
			completedQuestIds[quest.id]++;
		else
			completedQuestIds.Add(quest.id, 1);

		if(completedQuestInZone.ContainsKey(quest.zoneId))
			completedQuestInZone[quest.zoneId]++;
		else
			completedQuestInZone.Add(quest.zoneId, 1);
	}


	void CollectRewardsForScout(Quest quest) {
		Zone zone = ZoneManager.zoneManager.GetZoneByID(quest.zoneId);
		List<Quest> questsToAdd = new List<Quest>();
		RewardRange range = GetRewardRange(quest);
		int questCt = UnityEngine.Random.Range(range.min, range.max + 1);
		for(int i = 0; i < questCt; i++) {
			Quest q = new Quest(GetRandomQuestFromZone(zone));
			q.zoneId = zone.id;
			questsToAdd.Add(q);
			availableQuests.Add(q);
		}

		hudManager.ShowNotice(questCt.ToString() + " new quest" + (questCt > 1 ? "s" : "") + " scouted");
		hudManager.Broadcast("QuestsAdded", questsToAdd);
	}


	void CompleteQuestsForBlobs(Quest quest) {
		foreach(int blobId in quest.blobIds) {
			if(blobId == -1)
				continue;
			Blob blob = roomManager.GetBlobByID(blobId);
			blob.gameObject.ReturnFromQuest();
		}
	}


	public RewardRange GetRewardRange(Quest quest) {
		if(IsPartyFull(quest) == false)
			return null;
		
		int matchCt = 0;
		for(int i=0; i<quest.blobsRequired; i++) {
			Blob blob = roomManager.GetBlobByID(quest.blobIds[i]);
			if(DoesBlobMatchSlot(quest, blob, i))
				matchCt++;
		}

		RewardRange rewardRange = new RewardRange();
		if(quest.type == QuestType.Scouting) {
			rewardRange.min = 1;
			rewardRange.max = rewardRange.min + matchCt;
		}
		else {
			float ct = 2f + (1f * matchCt / quest.blobsRequired) * 3f;
			rewardRange.max = Mathf.FloorToInt(ct);
			rewardRange.min = Mathf.Min(quest.blobsRequired, rewardRange.max);
		}
		return rewardRange;
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


	BaseQuest GetRandomQuestFromZone(Zone zone) {
		List<BaseQuest> zoneBaseQuests = zone.QuestsForZone();
		List<BaseQuest> zoneBaseQuestsToDelete = new List<BaseQuest>();
		foreach(BaseQuest bq in zoneBaseQuests)
			if(IsPreReqCompleteForQuest(bq) == false)
				zoneBaseQuestsToDelete.Add(bq);
		foreach(BaseQuest bq in zoneBaseQuestsToDelete)
			zoneBaseQuests.Remove(bq);

		if(zoneBaseQuests.Count <= 0)
			return null;
		return zoneBaseQuests[UnityEngine.Random.Range(0, zoneBaseQuests.Count)];
	}


	public bool IsPreReqCompleteForQuest(BaseQuest bq) {
		foreach(int reqId in bq.prerequisiteQuestIds)
			if(completedQuestIds.ContainsKey(reqId) == false)
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
