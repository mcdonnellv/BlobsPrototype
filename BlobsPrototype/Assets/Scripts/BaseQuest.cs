using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


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
	public List<int> prerequisiteQuestIds = new List<int>();
}