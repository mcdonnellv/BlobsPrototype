using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ItemManager))]
public class ItemManagerInspector : GenericManagerInspector {
	bool showGenes = false;
	public override void OnInspectorGUI() {
		NGUIEditorTools.SetLabelWidth(defaultLabelWidth);
		BaseItem item = null;

		if (itemManager.items == null || itemManager.items.Count == 0)
			mIndex = 0;
		else {
			mIndex = Mathf.Clamp(mIndex, 0, itemManager.items.Count - 1);
			item = (BaseItem)itemManager.items[mIndex];
		}

		if (mConfirmDelete) {
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + item.itemName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete")){
					itemManager.items.RemoveAt(mIndex);
					RebuildAllItems();
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else {

			// Database icon atlas
			UIAtlas atlas = EditorGUILayout.ObjectField("Icon Atlas", itemManager.iconAtlas, typeof(UIAtlas), false) as UIAtlas;

			if (atlas != itemManager.iconAtlas) {
				itemManager.iconAtlas = atlas;
				foreach (BaseItem i in itemManager.items) i.iconAtlas = atlas;
			}

			// "New" button
			EditorGUILayout.BeginHorizontal();{
				newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("New Item") && !itemManager.DoesNameExistInList(newName)){
					BaseItem i = new BaseItem();
					i.itemName = newName;
					i.id = itemManager.GetNextAvailableID();
					if(item != null) {
						if(newName == "")
							i.itemName = item.itemName + " copy";
						i.description = item.description;
					}
					itemManager.items.Add(i);
					RebuildAllItems();
					mIndex = itemManager.items.Count - 1;
					newName = "";
					item = i;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			GUILayout.BeginHorizontal();
			if(GUILayout.Button ("Sort by ID"))
				itemManager.items = itemManager.items.OrderBy(x => x.id).ToList();
			if(GUILayout.Button ("Sort by Name"))
				itemManager.items = itemManager.items.OrderBy(x => x.itemName).ToList();
			GUILayout.EndHorizontal();

			if (item == null)
				return;

			NavigationSection(itemManager.items.Count);
			// Item name and delete item button
			GUILayout.BeginHorizontal();
			{
				NGUIEditorTools.SetLabelWidth(20f);
				int newId = EditorGUILayout.IntField("ID", item.id, GUILayout.Width(60f));
				if(newId != item.id)
					item.id = (itemManager.DoesIdExistInList(newId)) ? itemManager.GetNextAvailableID() : item.id = newId;
				NGUIEditorTools.SetLabelWidth(40f);
				string itemName = EditorGUILayout.TextField("Name", item.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(item.itemName) && itemManager.DoesNameExistInList(itemName) == false)
					item.itemName = itemName;
			}
			GUILayout.EndHorizontal();
			item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));
			item.quality = (Quality)EditorGUILayout.EnumPopup("Quality",item.quality);
			item.sellValue = EditorGUILayout.IntField("Value", item.sellValue);
			if(atlas != null && item.iconAtlas == null) item.iconAtlas = atlas;
			SpriteSelection(item);
		}
	}
}
