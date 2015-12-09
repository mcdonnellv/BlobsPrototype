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
public class BaseQuest : BaseThing {
	public QuestType type;
	public int mins = 0;
	public int hrs = 0;
	public int days = 0;
	public List<LootEntry> LootTableA = new List<LootEntry>(); // moster specific loot
	public List<LootEntry> LootTableB = new List<LootEntry>(); // more generic loot common across all quests
	public int blobsRequired = 1;

}


[Serializable]
public class Quest : BaseQuest {
	public static int maxblobsRequired = 5;
	public List<int> blobIds = new List<int>();
	public QuestState state;
	public DateTime actionReadyTime;
	public List<Element> elementRequirements = new List<Element>();

	RoomManager _roomManager;
	RoomManager roomManager { get {if(_roomManager == null) _roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>(); return _roomManager; } }


	public Quest(BaseQuest b) {
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

		for(int i = 0; i < blobsRequired; i++) {
			Element element = (Element)UnityEngine.Random.Range((int)Element.None, (int)Element.ElementCt);
			elementRequirements.Add(element);
		}
		
		state = QuestState.Available;
	}

	public void Start(List<int> blobListParam) {
		blobIds = blobListParam.ToList();
		foreach(int blobID in blobIds) {
			Blob blob = roomManager.GetBlobByID(blobID);
			blob.DepartForQuest(this);
		}

		TimeSpan actionDuration = new TimeSpan(days, hrs, mins, 0);
		if(actionDuration.TotalSeconds == 0)
			actionDuration = new TimeSpan(0, 0, 0, 5);
		actionReadyTime = System.DateTime.Now + actionDuration;
		state = QuestState.Embarked;
	}

	public void MakeAvailable() {
		state = QuestState.Available;
	}

	public void Complete() {
		state = QuestState.Completed;
	}

	public void Clear() {
		blobIds.Clear();
		state = QuestState.NotAvailable;
	}

	public void AddBlob (int id, int index) {
		foreach(int blobID in blobIds)
			if(id == blobID)
				blobIds.Remove(id); //remove dupes

		blobIds[index] = id;//  order matters
	}

	public void RemoveBlob (int id) {
		blobIds.Remove(id);
	}

	public bool IsHighYield() {
		if(blobIds == null || blobsRequired == 0)
			return false;

		int totalStam = 0;
		int index = 0;
		foreach(int blobID in blobIds) {
			Blob blob = roomManager.GetBlobByID(blobID);
			if(blob.combatStats.element != elementRequirements[index]) //  order matters
				return false;
			index++;
		}

		return true;
	}
}
