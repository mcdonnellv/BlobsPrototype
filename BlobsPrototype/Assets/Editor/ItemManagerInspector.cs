using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ItemManager))]
public class ItemManagerInspector : Editor {
	ItemManager itemManager;
	bool showGenes = false;
	string newName = "";
	bool mConfirmDelete = false;
	static int mIndex = 0;
	static GeneReq.Identifier geneReqIdTemp;
	static Stat.Identifier statIdTemp;
	
	
	public override void OnInspectorGUI(){
		NGUIEditorTools.SetLabelWidth(80f);
		itemManager = (ItemManager)target;
		Item item = null;

		if (itemManager.items == null || itemManager.items.Count == 0)
			mIndex = 0;
		else {
			mIndex = Mathf.Clamp(mIndex, 0, itemManager.items.Count - 1);
			item = itemManager.items[mIndex];
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
				if (GUILayout.Button("New Item") && itemManager.DoesNameExistInList(newName) == false){
					Item i = new Item();
					i.itemName = newName;
					if(item != null) {
						if(newName == "")
							i.itemName = item.itemName + " copy";
						i.description = item.description;
					}
					itemManager.items.Add(i);
					mIndex = itemManager.items.Count - 1;
					newName = "";
					item = i;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			if (item != null) {
				NGUIEditorTools.DrawSeparator();
				
				// Navigation section
				GUILayout.BeginHorizontal();{
					if (mIndex == 0) GUI.color = Color.grey;
					if (GUILayout.Button("<<")) { mConfirmDelete = false; --mIndex; }
					GUI.color = Color.white;
					mIndex = EditorGUILayout.IntField(mIndex + 1, GUILayout.Width(40f)) - 1;
					GUILayout.Label("/ " + itemManager.items.Count, GUILayout.Width(40f));
					if (mIndex + 1 == itemManager.items.Count) GUI.color = Color.grey;
					if (GUILayout.Button(">>")) { mConfirmDelete = false; ++mIndex; }
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
				NGUIEditorTools.DrawSeparator();

				// Item name and delete item button
				GUILayout.BeginHorizontal();{
					string itemName = EditorGUILayout.TextField("Item Name", item.itemName);
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("Delete", GUILayout.Width(55f)))
						mConfirmDelete = true;
					GUI.backgroundColor = Color.white;
					if (!itemName.Equals(item.itemName) && itemManager.DoesNameExistInList(itemName) == false)
						item.itemName = itemName;
				}
				GUILayout.EndHorizontal();
				item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));
			}
		}
	}
}
