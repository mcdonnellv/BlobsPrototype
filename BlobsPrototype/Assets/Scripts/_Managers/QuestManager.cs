using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenericRange { 
	public int max, min; 
	public int GetRandom() { return UnityEngine.Random.Range(min, max); }
}

public class QuestManager : MonoBehaviour {
	private static QuestManager _questManager;
	public static QuestManager questManager { get {if(_questManager == null) _questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>(); return _questManager; } }

	HudManager hudManager { get { return HudManager.hudManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	CombatManager combatManager { get { return CombatManager.combatManager; } }
	MonsterManager monsterManager { get { return MonsterManager.monsterManager; } }
	public List<BaseQuest> quests = new List<BaseQuest>(); // All quests
	public List<Quest> availableQuests = new List<Quest>(); //current quests
	public List<QuestBonus> combatBonuses = new List<QuestBonus>();
	public List<QuestBonus> gatheringBonuses = new List<QuestBonus>();
	public List<QuestBonus> scoutingBonuses = new List<QuestBonus>();
	public Dictionary<int, int> completedQuestIds = new Dictionary<int, int>();
	public Dictionary<int, int> completedQuestInZone = new Dictionary<int, int>();
	public UIAtlas iconAtlas;
	public static int maxAvailableQuests = 6;

	public bool DoesNameExistInList(string nameParam){return (GetBaseQuestWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseQuestByID(idParam) != null); }

	public Quest AddQuestToList(BaseQuest bq) {
		if(availableQuests.Count >= maxAvailableQuests)
			return null;
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
		BuildQuestBonuses();
		AddQuestToList(this.GetBaseQuestByID(0));
		AddQuestToList(this.GetBaseQuestByID(100));
	}


	public void CollectRewardsForQuest(Quest quest) {
		QuestFinishedCleanup(quest);

		foreach(Zone zone in ZoneManager.zoneManager.zones)
			if(zone.unlockingQuestId == quest.id && !zone.IsUnlocked())
				hudManager.popup.Show("New Zone", "Congratulations! The " + zone.itemName + " zone is now available");


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


	public void QuestFinishedCleanup(Quest quest) {
		CompleteQuestsForBlobs(quest);
		hudManager.questListMenu.QuestRemoved(availableQuests.IndexOf(quest));
		availableQuests.Remove(quest);
	}


	void CollectRewardsForScout(Quest quest) {
		Zone zone = ZoneManager.zoneManager.GetZoneByID(quest.zoneId);
		List<Quest> questsToAdd = new List<Quest>();
		GenericRange range = GetRewardRange(quest);
		int questSlotsLeft = maxAvailableQuests - availableQuests.Count;
		range.min = Mathf.Min(range.min, questSlotsLeft);
		range.max = Mathf.Min(range.max, questSlotsLeft);
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


	public GenericRange GetRewardRange(Quest quest) {
		GenericRange rewardRange = new GenericRange();

		if(quest.type == QuestType.Scouting) {
			rewardRange.min = 1 + quest.GetIncreasedRewardsBonusLevel();
			rewardRange.max = rewardRange.min;
		}
		else {
			rewardRange.min = 2 + quest.GetIncreasedRewardsBonusLevel();
			rewardRange.max = rewardRange.min + 5;
		}
		return rewardRange;
	}


	public bool DoesBlobMatchSlot(Quest quest, Blob blob, int index) {
		if(blob == null) 
			return false;
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


	public void StartQuest(Quest quest, List<int> blobListParam) {
		quest.blobIds = blobListParam.ToList();
		foreach(int blobId in quest.blobIds) {
			Blob blob = roomManager.GetBlobByID(blobId);
			blob.gameObject.DepartForQuest(quest);
		}
		quest.actionReadyTime = quest.GetActionReadyTime();
		quest.state = QuestState.Embarked;
		
		if(quest.type == QuestType.Combat) {
			foreach(int blobId in quest.blobIds) {
				Blob blob = roomManager.GetBlobByID(blobId);
				combatManager.AddCombatant(blob);
			}

			foreach(QuestMonster monster in quest.monsters) {
				BaseMonster bm = monsterManager.GetBaseMonsterByID(monster.id);
				Monster m = new Monster(bm);
				m.level = monster.level;
				combatManager.AddCombatant(m);
			}
			combatManager.quest = quest;
			hudManager.combatMenu.Show();
		}
	}


	public List<BaseQuest> GetQuestsWithPreReqComplete() {
		List<BaseQuest> retList = new List<BaseQuest>();
		foreach(BaseQuest b in quests)
			if(IsPreReqCompleteForQuest(b))
				retList.Add(b);
		return retList;
	}


	public bool IsItemIdPartOfQuestLoot(int itemId, BaseQuest bq) {
		foreach(LootEntry l in bq.LootTableA) 
			if(l.itemId == itemId)
				return true;
		foreach(LootEntry l in bq.LootTableB) 
			if(l.itemId == itemId)
				return true;
		return false;
	}
	

	public void BuildQuestBonuses() {
		BuildCombatBonuses();
		BuildGatheringBonuses();
		BuildScoutingBonuses();
	}


	public void BuildCombatBonuses() {
		combatBonuses = new List<QuestBonus>();
		combatBonuses.Add(new QuestBonus(.10f, QuestBonusType.Attack, 60));
		combatBonuses.Add(new QuestBonus(.15f, QuestBonusType.Attack, 30));
		combatBonuses.Add(new QuestBonus(.20f, QuestBonusType.Attack, 10));
		combatBonuses.Add(new QuestBonus(.10f, QuestBonusType.Health, 60));
		combatBonuses.Add(new QuestBonus(.15f, QuestBonusType.Health, 30));
		combatBonuses.Add(new QuestBonus(.20f, QuestBonusType.Health, 10));
		combatBonuses.Add(new QuestBonus(.20f, QuestBonusType.IncreasedRewards, 100));
		combatBonuses = combatBonuses.OrderBy(x => x.weight).ToList();
	}


	public void BuildGatheringBonuses() {
		gatheringBonuses = new List<QuestBonus>();
		gatheringBonuses.Add(new QuestBonus(0, QuestBonusType.IncreasedRewards, 60));
		gatheringBonuses.Add(new QuestBonus(1, QuestBonusType.IncreasedRewards, 25));
		gatheringBonuses.Add(new QuestBonus(2, QuestBonusType.IncreasedRewards, 10));
		gatheringBonuses.Add(new QuestBonus(3, QuestBonusType.IncreasedRewards, 5));
		gatheringBonuses = gatheringBonuses.OrderBy(x => x.weight).ToList();
	}


	public void BuildScoutingBonuses() {
		scoutingBonuses = new List<QuestBonus>();
		scoutingBonuses.Add(new QuestBonus(0, QuestBonusType.IncreasedRewardsScouting, 60));
		scoutingBonuses.Add(new QuestBonus(1, QuestBonusType.IncreasedRewardsScouting, 25));
		scoutingBonuses.Add(new QuestBonus(2, QuestBonusType.IncreasedRewardsScouting, 10));
		scoutingBonuses.Add(new QuestBonus(3, QuestBonusType.IncreasedRewardsScouting, 5));
		scoutingBonuses = scoutingBonuses.OrderBy(x => x.weight).ToList();
	}


	public QuestBonus RollQuestBonus(Quest quest) {
		List<QuestBonus> possibleBonuses = GetAppropriateBonusList(quest);
		int totalWeight = 0;
		int cummulativeWeight = 0;

		foreach(QuestBonus qb in possibleBonuses) totalWeight += qb.weight;
		int roll = UnityEngine.Random.Range(0, totalWeight);
		foreach(QuestBonus qb in possibleBonuses) {
			cummulativeWeight += qb.weight;
			if(roll < cummulativeWeight)
				return qb;
		}
		return null;
	}


	public List<QuestBonus> GetAppropriateBonusList(Quest quest) {
		switch(quest.type) {
		case QuestType.Combat: return combatBonuses;
		case QuestType.Gathering: return gatheringBonuses;
		case QuestType.Scouting: return scoutingBonuses;
		}
		return null;
	}

	void Update() {
		foreach(Quest quest in availableQuests) {
			if(quest.state == QuestState.Embarked && quest.actionReadyTime <= System.DateTime.Now)
				quest.Complete();
		}
	}
}
