using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class LootEntry {
	public int quantity = 1;
	public int itemId = -1;
	public int probability;
}


[Serializable]
public class QuestMonster {
	public int id;
	public int level;
	public QuestMonster(int idParam, int levelParam) {id = idParam; level = levelParam; }
}


[Serializable]
public class BaseQuest : BaseThing {
	public QuestType type;
	public int mins = 0;
	public int hrs = 0;
	public int days = 0;
	public List<LootEntry> LootTableA = new List<LootEntry>(); // moster specific loot
	public List<LootEntry> LootTableB = new List<LootEntry>(); // more generic loot common across all quests
	public int blobsRequired = 1;
	public bool usesElements = false;
	public bool usesSigils = false;
	public bool mixedElements = false;
	public bool mixedSigils = false;
	public List<QuestMonster> monsters = new List<QuestMonster>();
}


[Serializable]
public class Quest : BaseQuest {
	public static int maxblobsRequired = 5;
	public List<int> blobIds;
	public QuestState state;
	public DateTime actionReadyTime;
	public List<Element> elementRequirements = new List<Element>();
	public List<Sigil> sigilRequirements = new List<Sigil>();
	public bool alreadySeen = false;
	public int zoneId = -1;


	RoomManager roomManager  { get { return RoomManager.roomManager; } }


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

		Element element = (Element)UnityEngine.Random.Range(0, (int)Element.ElementCt);
		Sigil sigil = (Sigil)UnityEngine.Random.Range(0, (int)Sigil.SigilCt);
		for(int i = 0; i < blobsRequired; i++) {
			if(usesElements) {
				elementRequirements.Add(element);
				if(mixedElements) 
					element = (Element)UnityEngine.Random.Range(0, (int)Element.ElementCt);
			}
			else
				elementRequirements.Add(Element.None);

			if(usesSigils) {
				sigilRequirements.Add(sigil);
				if(mixedSigils) 
					sigil = (Sigil)UnityEngine.Random.Range(0, (int)Sigil.SigilCt);
			}
			else
				sigilRequirements.Add(Sigil.None);
		}

		blobIds = new List<int>();
		for(int i = 0; i < blobsRequired; i++)
			blobIds.Add(-1);
		state = QuestState.Available;
	}

	public void Start(List<int> blobListParam) {
		blobIds = blobListParam.ToList();
		foreach(int blobID in blobIds) {
			Blob blob = roomManager.GetBlobByID(blobID);
			blob.gameObject.DepartForQuest(this);
		}
		actionReadyTime = GetActionReadyTime();
		state = QuestState.Embarked;
	}


	public TimeSpan GetActionReadyDuration() { 
		TimeSpan actionDuration = new TimeSpan(days, hrs, mins, 0); 
		if(actionDuration.TotalSeconds == 0)
			actionDuration = new TimeSpan(0, 0, 0, 5);
		return actionDuration;
	}

	public DateTime GetActionReadyTime() { return System.DateTime.Now + GetActionReadyDuration(); }


	public void MakeAvailable() {
		state = QuestState.Available;
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
}
