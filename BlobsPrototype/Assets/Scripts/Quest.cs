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
	public int blobsAllowed = 1;
}


[Serializable]
public class Quest : BaseQuest {
	public static int maxBlobsAllowed = 5;
	public List<Blob> blobs;
	public QuestState state;

	public Quest(BaseQuest b) {
		itemName = b.itemName;
		description = b.description;
		iconName = b.iconName;
		iconAtlas = b.iconAtlas;
		quality = b.quality;
		type = b.type;
		blobsAllowed = b.blobsAllowed;
		mins = b.mins;
		hrs = b.hrs;
		days = b.days;
		LootTableA = b.LootTableA.ToList();
		LootTableB = b.LootTableB.ToList();
		
		state = QuestState.Available;
	}
}
