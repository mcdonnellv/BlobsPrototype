using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[CustomEditor(typeof(ZoneManager))]
public class ZoneManagerInspector : GenericManagerInspector {

	public ZoneManager zoneManager { get { return ZoneManager.zoneManager; } }

	public override void OnInspectorGUI() {
		NGUIEditorTools.SetLabelWidth(defaultLabelWidth);
		Zone zone = null;
		
		if (zoneManager.zones == null || zoneManager.zones.Count == 0)
			mIndex = 0;
		else {
			mIndex = Mathf.Clamp(mIndex, 0, zoneManager.zones.Count - 1);
			zone = (Zone)zoneManager.zones[mIndex];
		}
		
		if (mConfirmDelete) {
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + zone.itemName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete")){
					zoneManager.zones.RemoveAt(mIndex);
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else {
			
			// "New" button
			EditorGUILayout.BeginHorizontal();{
				newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("New Zone") && !zoneManager.DoesNameExistInList(newName)){
					Zone z = new Zone();
					z.itemName = newName;
					z.id = zoneManager.GetNextAvailableID();
					if(zone != null) {
						if(newName == "")
							z.itemName = zone.itemName + " copy";
						z.questIds = zone.questIds.ToList();
					}
					zoneManager.zones.Add(z);
					mIndex = zoneManager.zones.Count - 1;
					newName = "";
					zone = z;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button ("Sort by ID"))
				zoneManager.zones = zoneManager.zones.OrderBy(x => x.id).ToList();
			if(GUILayout.Button ("Sort by Name"))
				zoneManager.zones = zoneManager.zones.OrderBy(x => x.itemName).ToList();
			GUILayout.EndHorizontal();
			
			if (zone == null)
				return;
			
			NavigationSection(zoneManager.zones.Count);
			// Item name and delete item button
			GUILayout.BeginHorizontal();
			{
				NGUIEditorTools.SetLabelWidth(20f);
				int newId = EditorGUILayout.IntField("ID", zone.id, GUILayout.Width(60f));
				if(newId != zone.id)
					zone.id = (zoneManager.DoesIdExistInList(newId)) ? zoneManager.GetNextAvailableID() : zone.id = newId;
				NGUIEditorTools.SetLabelWidth(40);
				string itemName = EditorGUILayout.TextField("Name", zone.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(zone.itemName) && zoneManager.DoesNameExistInList(itemName) == false)
					zone.itemName = itemName;
			}
			GUILayout.EndHorizontal();

			NGUIEditorTools.DrawSeparator();
			EditorGUILayout.LabelField("Zone Quests");
			EditorGUI.indentLevel++;
			NGUIEditorTools.SetLabelWidth(60f);
			List <int> toDelete = new List<int>();
			int i = 0;
			foreach(int questID in zone.questIds) {
				GUILayout.BeginHorizontal();
				BaseQuest bq = questManager.GetBaseQuestByID(questID);
				int indexOfQuest = questManager.quests.IndexOf(bq);
				int newIndexOfQuest = EditorGUILayout.Popup("Quest", indexOfQuest, allQuests, GUILayout.Width(250f));

				if(indexOfQuest != newIndexOfQuest) 
					zone.questIds[i] = newIndexOfQuest;
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Del", GUILayout.Width(35f)))
					toDelete.Add(i);
				GUI.backgroundColor = Color.white;
				GUILayout.EndHorizontal();
				i++;
			}

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Add", GUILayout.Width(35f))) 
				zone.questIds.Add(0);
			GUI.backgroundColor = Color.white;
			EditorGUI.indentLevel--;
			NGUIEditorTools.DrawSeparator();
			EditorGUILayout.LabelField("Zone Monsters");

			foreach(int d in toDelete)
				zone.questIds.RemoveAt(d);
		}
	}
}
