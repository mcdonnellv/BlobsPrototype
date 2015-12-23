using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(QuestManager))]
public class QuestManagerInspector : GenericManagerInspector {

	public override void OnInspectorGUI() {
		NGUIEditorTools.SetLabelWidth(defaultLabelWidth);
		BaseQuest quest = null;

		if (questManager.quests == null || questManager.quests.Count == 0)
			mIndex = 0;
		else {
			mIndex = Mathf.Clamp(mIndex, 0, questManager.quests.Count - 1);
			quest = (BaseQuest)questManager.quests[mIndex];
		}
	
		if (mConfirmDelete) {
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + quest.itemName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete")){
					questManager.quests.RemoveAt(mIndex);
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else {

			// Database icon atlas
			UIAtlas atlas = EditorGUILayout.ObjectField("Icon Atlas", questManager.iconAtlas, typeof(UIAtlas), false) as UIAtlas;
			
			if (atlas != questManager.iconAtlas) {
				questManager.iconAtlas = atlas;
				foreach (BaseQuest i in questManager.quests) i.iconAtlas = atlas;
			}

			// "New" button
			EditorGUILayout.BeginHorizontal();{
				newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("New Quest") && !questManager.DoesNameExistInList(newName)){
					BaseQuest i = new BaseQuest();
					i.itemName = newName;
					i.id = questManager.GetNextAvailableID();
					if(quest != null) {
						if(newName == "")
							i.itemName = quest.itemName + " copy";
						i.description = quest.description;
						i.quality = quest.quality;
						i.iconAtlas = quest.iconAtlas;
						i.iconName = quest.iconName;
						i.iconTintIndex = quest.iconTintIndex;
						i.type = quest.type;
						i.mins = quest.mins;
						i.hrs = quest.hrs;
						i.days = quest.days;
						i.LootTableA = quest.LootTableA.ToList();
						i.LootTableB = quest.LootTableB.ToList();
						i.blobsRequired = quest.blobsRequired;
						i.usesElements = quest.usesElements;
						i.usesSigils = quest.usesSigils;
						i.mixedElements = quest.mixedElements;
						i.mixedSigils = quest.mixedSigils;
						i.monsters = quest.monsters;
					}
					questManager.quests.Add(i);
					mIndex = questManager.quests.Count - 1;
					newName = "";
					quest = i;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			GUILayout.BeginHorizontal();
			if(GUILayout.Button ("Sort by ID"))
				questManager.quests = questManager.quests.OrderBy(x => x.id).ToList();
			if(GUILayout.Button ("Sort by Name"))
				questManager.quests = questManager.quests.OrderBy(x => x.itemName).ToList();
			GUILayout.EndHorizontal();

			if (quest == null)
				return;

			NavigationSection(questManager.quests.Count);
			// Item name and delete item button
			GUILayout.BeginHorizontal();
			{
				NGUIEditorTools.SetLabelWidth(20f);
				int newId = EditorGUILayout.IntField("ID", quest.id, GUILayout.Width(60f));
				if(newId != quest.id)
					quest.id = (questManager.DoesIdExistInList(newId)) ? questManager.GetNextAvailableID() : quest.id = newId;
				NGUIEditorTools.SetLabelWidth(40);
				string itemName = EditorGUILayout.TextField("Name", quest.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(quest.itemName) && questManager.DoesNameExistInList(itemName) == false)
					quest.itemName = itemName;
			}
			GUILayout.EndHorizontal();
			quest.description = GUILayout.TextArea(quest.description, 200, GUILayout.Height(100f));
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Duration", GUILayout.Width(60f));
			NGUIEditorTools.SetLabelWidth(35f);
			quest.days = EditorGUILayout.IntField("Days", quest.days, GUILayout.Width(70f));
			quest.hrs = EditorGUILayout.IntField("Hours", quest.hrs, GUILayout.Width(70f));
			quest.mins = EditorGUILayout.IntField("Mins", quest.mins, GUILayout.Width(70f));
			GUILayout.EndHorizontal();

			NGUIEditorTools.SetLabelWidth(100);
			GUILayout.BeginHorizontal();
			quest.blobsRequired = Mathf.Max(1, Mathf.Min(Quest.maxblobsRequired, EditorGUILayout.IntField("Blobs Allowed", quest.blobsRequired, GUILayout.Width(130f))));
			GUILayout.EndHorizontal();
			quest.type = (QuestType)EditorGUILayout.EnumPopup("Type",quest.type);
			quest.quality = (Quality)EditorGUILayout.EnumPopup("Quality",quest.quality);


			List<string> zones = new List<string>();
			foreach(Zone zone in ZoneManager.zoneManager.zones)
				foreach(int questId in zone.questIds) 
					if(questId == quest.id)
						zones.Add(zone.itemName);
				
			string zoneStr = "Zones: ";
			foreach(string s in zones)
				zoneStr += s + ", ";
			EditorGUILayout.LabelField(zoneStr);

			quest.usesElements = EditorGUILayout.Toggle("Uses Elements", quest.usesElements);
			if(quest.usesElements)
				quest.mixedElements = EditorGUILayout.Toggle("Mixed Elements", quest.mixedElements);
			quest.usesSigils = EditorGUILayout.Toggle("Uses Sigils", quest.usesSigils);
			if(quest.usesSigils)
				quest.mixedSigils = EditorGUILayout.Toggle("Mixed Sigils", quest.mixedSigils);

			// MONSTER
			if(quest.type == QuestType.Combat) {
				NGUIEditorTools.DrawSeparator();
				EditorGUILayout.LabelField("Monsters");
				EditorGUI.indentLevel++;
				NGUIEditorTools.SetLabelWidth(30f);
				List<BaseMonster> monsterlist = new List<BaseMonster>();
				string[] monsterlistStrings =  new string[monsterManager.monsters.Count];
				for(int i=0; i < monsterlistStrings.Length; i++)
					monsterlistStrings[i] = monsterManager.monsters[i].id.ToString() + " : " + monsterManager.monsters[i].itemName;

				List<QuestMonster> monstersToAdd = new List<QuestMonster>();
				List<QuestMonster> monstersToDelete = new List<QuestMonster>();
				foreach(QuestMonster questMonster in quest.monsters) {
					GUILayout.BeginHorizontal();
					BaseMonster bm = monsterManager.GetBaseMonsterByID(questMonster.id);
					int index = monsterManager.monsters.IndexOf(bm);
					int newMonsterIndex = EditorGUILayout.Popup(index, monsterlistStrings, GUILayout.Width(150f));
					questMonster.level = EditorGUILayout.IntField("lv", questMonster.level, GUILayout.Width(70f));

					if(index != newMonsterIndex) {
						int newMonsterId = GetIdFromString(monsterlistStrings[newMonsterIndex]);
						monstersToDelete.Add(questMonster);
						monstersToAdd.Add(new QuestMonster(newMonsterId, 1));
					}

					if(DeleteButtonPressed())
						monstersToDelete.Add(questMonster);
					GUILayout.EndHorizontal();
				}

				foreach(QuestMonster bm in monstersToDelete)
					quest.monsters.Remove(bm);

				foreach(QuestMonster bm in monstersToAdd)
					quest.monsters.Add(bm);
				
				if(AddButtonPressed())
					quest.monsters.Add(new QuestMonster(GetIdFromString(monsterlistStrings[0]), 1));
				EditorGUI.indentLevel--;
			}



			// LOOT TABLES
			NGUIEditorTools.DrawSeparator();
			LootTable(quest, quest.LootTableA, "Loot Table A");
			NGUIEditorTools.DrawSeparator();
			LootTable(quest, quest.LootTableB, "Loot Table B");

			// SPRITE
			NGUIEditorTools.DrawSeparator();
			NGUIEditorTools.SetLabelWidth(defaultLabelWidth);
			if(atlas != null && quest.iconAtlas == null) quest.iconAtlas = atlas;
			SpriteSelection(quest);
		}
	}


	public void LootTable(BaseQuest quest, List<LootEntry> lootTable, string name) {
		NGUIEditorTools.SetLabelWidth(defaultLabelWidth);
		EditorGUILayout.LabelField(name);
		EditorGUI.indentLevel++;
		List <LootEntry> toDelete = new List<LootEntry>();
		int pointsleft = 100;
		foreach(LootEntry lootEntry in lootTable) {
			GUILayout.BeginHorizontal();
			int index = 0;
			if(lootEntry.itemId >= 0) {
				BaseItem itm = itemManager.GetBaseItemByID(lootEntry.itemId);
				index = itemManager.items.IndexOf(itm);
			}
			
			NGUIEditorTools.SetLabelWidth(30f);
			lootEntry.probability = EditorGUILayout.IntField("%", lootEntry.probability, GUILayout.Width(60f));
			lootEntry.probability = Mathf.Max(1, Mathf.Min(100, lootEntry.probability));
			pointsleft -= lootEntry.probability;
			NGUIEditorTools.SetLabelWidth(35f);
			int newIndex = EditorGUILayout.Popup("ID", index, allItems, GUILayout.Width(100f));
			if(newIndex != index || lootEntry.itemId < 0) 
				lootEntry.itemId = itemManager.items[newIndex].id;
			
			NGUIEditorTools.SetLabelWidth(30f);
			lootEntry.quantity = EditorGUILayout.IntField("x", lootEntry.quantity, GUILayout.Width(50f));
			lootEntry.quantity = Mathf.Max(1, Mathf.Min(99, lootEntry.quantity));
			
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Del", GUILayout.Width(35f)))
				toDelete.Add(lootEntry);
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
		}

		if(pointsleft != 0)
			EditorGUILayout.LabelField(pointsleft.ToString() + " points left to distribute");
		GUILayout.BeginHorizontal();
		if(AddButtonPressed())
			lootTable.Add(new LootEntry());
		GUILayout.EndHorizontal();
		foreach(LootEntry lootEntry in toDelete)
			lootTable.Remove(lootEntry);
		EditorGUI.indentLevel--;
	}
}
