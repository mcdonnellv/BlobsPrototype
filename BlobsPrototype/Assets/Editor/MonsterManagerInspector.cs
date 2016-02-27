using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(MonsterManager))]
public class MonsterManagerInspector : GenericManagerInspector {

	public override void OnInspectorGUI() {
		NGUIEditorTools.SetLabelWidth(defaultLabelWidth);
		BaseMonster monster = null;
		
		if (monsterManager.monsters == null || monsterManager.monsters.Count == 0)
			mIndex = 0;
		else {
			mIndex = Mathf.Clamp(mIndex, 0, monsterManager.monsters.Count - 1);
			monster = (BaseMonster)monsterManager.monsters[mIndex];
		}
		
		if (mConfirmDelete) {
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + monster.itemName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete")){
					monsterManager.monsters.RemoveAt(mIndex);
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else {

			// Database icon atlas
			UIAtlas atlas = EditorGUILayout.ObjectField("Icon Atlas", questManager.iconAtlas, typeof(UIAtlas), false) as UIAtlas;
			
			if (atlas != monsterManager.iconAtlas) {
				monsterManager.iconAtlas = atlas;
				foreach (BaseMonster m in monsterManager.monsters) m.iconAtlas = atlas;
			}

			// "New" button
			EditorGUILayout.BeginHorizontal();{
				newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("New Monster") && !monsterManager.DoesNameExistInList(newName)){
					BaseMonster z = new BaseMonster();
					z.itemName = newName;
					z.id = monsterManager.GetNextAvailableID();
					if(monster != null) {
						if(newName == "")
							z.itemName = monster.itemName + " copy";
						z.iconAtlas = monster.iconAtlas;
					}
					monsterManager.monsters.Add(z);
					mIndex = monsterManager.monsters.Count - 1;
					newName = "";
					monster = z;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button ("Sort by ID"))
				monsterManager.monsters = monsterManager.monsters.OrderBy(x => x.id).ToList();
			if(GUILayout.Button ("Sort by Name"))
				monsterManager.monsters = monsterManager.monsters.OrderBy(x => x.itemName).ToList();
			GUILayout.EndHorizontal();
			
			if (monster == null)
				return;
			
			NavigationSection(monsterManager.monsters.Count);
			// Item name and delete item button
			GUILayout.BeginHorizontal();
			{
				NGUIEditorTools.SetLabelWidth(20f);
				int newId = EditorGUILayout.IntField("ID", monster.id, GUILayout.Width(60f));
				if(newId != monster.id)
					monster.id = (monsterManager.DoesIdExistInList(newId)) ? monsterManager.GetNextAvailableID() : monster.id = newId;
				NGUIEditorTools.SetLabelWidth(40);
				string itemName = EditorGUILayout.TextField("Name", monster.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(monster.itemName) && monsterManager.DoesNameExistInList(itemName) == false)
					monster.itemName = itemName;
			}
			GUILayout.EndHorizontal();
			
			NGUIEditorTools.DrawSeparator();
			NGUIEditorTools.SetLabelWidth(100f);
			ActorData data = monster.data;
			data.health = EditorGUILayout.FloatField("Health", data.health);
			data.criticalHealth = Mathf.PingPong(EditorGUILayout.FloatField("CriticalHealth", data.criticalHealth), .5f);
			data.attack = EditorGUILayout.FloatField("Attack", data.attack);
			data.speed = EditorGUILayout.FloatField("Speed", data.speed);
			data.stamina = EditorGUILayout.FloatField("Stamina", data.stamina);
			data.damageMitigation = Mathf.PingPong(EditorGUILayout.FloatField("DamageMitigation", data.damageMitigation), 1f);
			data.flinchLimit = EditorGUILayout.FloatField("flinchLimit", data.flinchLimit);
			data.walkSpeed = EditorGUILayout.FloatField("WalkSpeed", data.walkSpeed);
			data.runSpeed = EditorGUILayout.FloatField("RunSpeed", data.runSpeed);
			data.perception = EditorGUILayout.FloatField("Perception", data.perception);
			NGUIEditorTools.DrawSeparator();
			EditorGUILayout.LabelField("Enrage");
			EditorGUI.indentLevel++;
			data.enrageDuration = EditorGUILayout.FloatField("Duration", data.enrageDuration);
			data.enrageAttack = EditorGUILayout.FloatField("Attack", data.enrageAttack);
			data.enrageSpeed = EditorGUILayout.FloatField("Speed", data.enrageSpeed);
			data.enrageConditionTimer = EditorGUILayout.FloatField("Start Time", data.enrageConditionTimer);
			EditorGUI.indentLevel--;
			//SPRITE
			NGUIEditorTools.DrawSeparator();
			if(atlas != null && monster.iconAtlas == null) monster.iconAtlas = atlas;
			SpriteSelection(monster);
		}
	}
}
