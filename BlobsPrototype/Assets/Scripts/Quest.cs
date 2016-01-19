using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Quest : BaseQuest {
	public static int maxblobsRequired = 5;
	public List<int> blobIds;
	public QuestState state;
	public DateTime actionReadyTime;
	public List<Element> elementRequirements = new List<Element>();
	public List<Sigil> sigilRequirements = new List<Sigil>();
	public List<QuestBonus> combatBonuses = new List<QuestBonus>();
	public List<QuestBonus> gatheringBonuses = new List<QuestBonus>();
	public List<QuestBonus> scoutingBonuses = new List<QuestBonus>();
	public bool alreadySeen = false;
	public int zoneId = -1;


	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	QuestManager questManager  { get { return QuestManager.questManager; } }
	public DateTime GetActionReadyTime() { return System.DateTime.Now + GetActionReadyDuration(); }
	public void MakeAvailable() { state = QuestState.Available; }

	public Quest(BaseQuest b) {
		id = b.id;
		itemName = b.itemName;
		description = b.description;
		iconName = b.iconName;
		iconAtlas = b.iconAtlas;
		quality = b.quality;
		type = b.type;
		blobsRequired = b.blobsRequired;
		mins = b.mins;
		hrs = b.hrs;
		days = b.days;
		LootTableA = b.LootTableA.ToList();
		LootTableB = b.LootTableB.ToList();
		usesElements = b.usesElements;
		usesSigils = b.usesSigils;
		mixedElements = b.mixedElements;
		mixedSigils = b.mixedSigils;
		monsters = b.monsters.ToList();
		prerequisiteQuestIds = b.prerequisiteQuestIds.ToList();
		RollRequirements();
		RollQuestBonuses();
		blobIds = new List<int>();
		for(int i = 0; i < blobsRequired; i++)
			blobIds.Add(-1);
		state = QuestState.Available;
	}


	public void RollRequirements() {
		GenericRange requirementsRange = new GenericRange();
		requirementsRange.max = blobsRequired + 1;
		requirementsRange.min = 0;
		int reqCt = requirementsRange.GetRandom();

		Element RandomElement = (Element)UnityEngine.Random.Range(0, (int)Element.ElementCt);
		Sigil RandomSigil = (Sigil)UnityEngine.Random.Range(0, (int)Sigil.SigilCt);

		for(int i = 0; i < blobsRequired; i++) {
			if(usesElements && i < reqCt)
				elementRequirements.Add(RandomElement);
			else
				elementRequirements.Add(Element.None);
			
			if(usesSigils && i < reqCt)
				sigilRequirements.Add(RandomSigil);
			else
				sigilRequirements.Add(Sigil.None);
		}
	}


	public int GetRequirementCount() {
		int ct = 0;
		for(int i=0; i < blobsRequired; i++) {
			bool inc = false;
			if(i < sigilRequirements.Count && sigilRequirements[i] != Sigil.None)
				inc = true;
			if(i < elementRequirements.Count && elementRequirements[i] != Element.None)
				inc = true;
			if(inc)
				ct++;
		}
		return ct;
	}


	public void RollQuestBonuses() {
		if(GetRequirementCount() == 0)
			return;
		int bonusCt = 0;
		int roll = UnityEngine.Random.Range(0, 10);
		if(roll >= 10)
			bonusCt = 3;
		else if(roll >= 6)
			bonusCt = 2;
		else
			bonusCt = 1;

		switch(type) {
		case QuestType.Combat:
			combatBonuses = new List<QuestBonus>();
			for(int i = 0; i < bonusCt; i++)
				combatBonuses.Add(questManager.RollQuestBonus(this));
			combatBonuses = combatBonuses.Distinct().ToList();
			break;
		case QuestType.Gathering: 
			gatheringBonuses = new List<QuestBonus>();
			for(int i = 0; i < bonusCt; i++)
				gatheringBonuses.Add(questManager.RollQuestBonus(this));
			gatheringBonuses = gatheringBonuses.Distinct().ToList();
			break;
		case QuestType.Scouting: 
			scoutingBonuses = new List<QuestBonus>();
			for(int i = 0; i < bonusCt; i++)
				scoutingBonuses.Add(questManager.RollQuestBonus(this));
			scoutingBonuses = scoutingBonuses.Distinct().ToList();
			break;
		}
	}


	public TimeSpan GetActionReadyDuration() { 
		TimeSpan actionDuration = new TimeSpan(days, hrs, mins, 0); 
		if(actionDuration.TotalSeconds == 0)
			actionDuration = new TimeSpan(0, 0, 0, 5);
		return actionDuration;
	}
	

	public void Complete() {
		state = QuestState.Completed;
		// announce
		HudManager.hudManager.Broadcast("QuestComplete", this);
		HudManager.hudManager.ShowNotice("Quest Complete");

		for(int i=0; i < blobIds.Count; i++) {
			roomManager.GetBlobByID(blobIds[i]).actionReadyTime = System.DateTime.Now;
		}
	}


	public void Clear() {
		RemoveAllBlobs();
		state = QuestState.NotAvailable;
	}


	public void RemoveAllBlobs() {
		for(int i = 0; i < blobsRequired; i++)
			blobIds[i] = -1;
	}


	public void AddBlob (int id, int index) {
		RemoveBlob(id);
		blobIds[index] = id;//  order matters
	}


	public void RemoveBlob (int id) {
		for(int i = 0; i < blobIds.Count; i++)
			if(id == blobIds[i])
				blobIds[i] = -1;
	}


	public int GetIncreasedRewardsBonusLevel() {
		List<QuestBonus> bonusList = GetAppropriateBonusList();
		foreach(QuestBonus qb in bonusList) 
			if(qb.type == QuestBonusType.IncreasedRewards || qb.type == QuestBonusType.IncreasedRewardsScouting)
				return (int)qb.value + 1;
		return 0;
	}


	public List<QuestBonus> GetAppropriateBonusList() {
		switch(type) {
		case QuestType.Combat: return combatBonuses;
		case QuestType.Gathering: return gatheringBonuses;
		case QuestType.Scouting: return scoutingBonuses;
		}
		return null;
	}


	public int GetNumMatchesRequiredForBonus(QuestBonus qb) {
		List<QuestBonus> qbList = GetAppropriateBonusList();
		int matchTotalCt = GetRequirementCount();
		int index = qbList.IndexOf(qb);
		if(index == -1)
			return index;
		int retVal = Mathf.CeilToInt(matchTotalCt * (index + 1) / qbList.Count);
		return retVal;
	}










}
