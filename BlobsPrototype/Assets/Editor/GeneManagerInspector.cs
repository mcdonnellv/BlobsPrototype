using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GeneManager))]
public class GeneManagerInspector : Editor {
	GeneManager geneManager;
	ItemManager itemManager;
	string newName = "";
	static int mIndex = 0;
	bool mConfirmDelete = false;


	public override void OnInspectorGUI(){
		NGUIEditorTools.SetLabelWidth(80f);
		geneManager = (GeneManager)target;

		if(itemManager == null)
			itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();

		BaseGene item = null;

		if (geneManager.genes == null || geneManager.genes.Count == 0)
			mIndex = 0;
		else{
			mIndex = Mathf.Clamp(mIndex, 0, geneManager.genes.Count - 1);
			item = geneManager.genes[mIndex];
		}

		if (mConfirmDelete){
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + item.itemName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete")){
					geneManager.genes.RemoveAt(mIndex);
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
				if (GUILayout.Button("New Gene") && geneManager.DoesNameExistInList(newName) == false){
					BaseGene g = new BaseGene();
					g.itemName = newName;
					if(item != null) {
						if(newName == "")
							g.itemName = item.itemName + " copy";
						g.description = item.description;
						g.quality = item.quality;
						g.value = item.value;
						g.modifier = item.modifier;
						g.traitType = item.traitType;
					}
					geneManager.genes.Add(g);
					mIndex = geneManager.genes.Count - 1;
					newName = "";
					item = g;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			if(GUILayout.Button ("Sort"))
				geneManager.genes = geneManager.genes.OrderBy(x => x.traitType).ThenByDescending(x => x.quality).ThenBy(x => x.itemName).ToList();

			if (item != null) {
				NGUIEditorTools.DrawSeparator();
				
				// Navigation section
				GUILayout.BeginHorizontal();{
					if (mIndex == 0) GUI.color = Color.grey;
					if (GUILayout.Button("<<")) { mConfirmDelete = false; --mIndex; }
					GUI.color = Color.white;
					mIndex = EditorGUILayout.IntField(mIndex + 1, GUILayout.Width(40f)) - 1;
					GUILayout.Label("/ " + geneManager.genes.Count, GUILayout.Width(40f));
					if (mIndex + 1 == geneManager.genes.Count) GUI.color = Color.grey;
					if (GUILayout.Button(">>")) { mConfirmDelete = false; ++mIndex; }
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
				NGUIEditorTools.DrawSeparator();
			}


			if (item == null)
				return;

			// Item name and delete item button
			GUILayout.BeginHorizontal();{
				string itemName = EditorGUILayout.TextField("Gene Name", item.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(item.itemName) && geneManager.DoesNameExistInList(itemName) == false)
					item.itemName = itemName;
			}
			GUILayout.EndHorizontal();
			item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));
			item.quality = (Quality)EditorGUILayout.EnumPopup("Quality: ", item.quality);
			GUILayout.BeginHorizontal();{
				item.traitType = (TraitType)EditorGUILayout.EnumPopup("Trait: ", item.traitType, GUILayout.Width(200f));
				item.value = EditorGUILayout.IntField(item.value, GUILayout.Width(40f));
				item.modifier = (AbilityModifier)EditorGUILayout.EnumPopup(item.modifier);
			}
			GUILayout.EndHorizontal();

//			EditorGUILayout.Space();
//			EditorGUILayout.LabelField("Stats");
//			EditorGUI.indentLevel++;
//			if(item.stats == null)
//				item.stats = new List<Stat>();
//
//			for(int i=0; i < (int)TraitType.TraitTypeCt; i++) {
//				TraitType traitTypeId = (TraitType)i;
//				Stat stat = null;
//				foreach (Stat s in item.stats) {
//					if(s.id == traitTypeId)
//						stat = s;
//				}
//
//				GUILayout.BeginHorizontal();{
//					if(stat != null) {
//						stat.amount = EditorGUILayout.IntField(Stat.GetStatIdByIndex(i).ToString(), stat.amount, GUILayout.Width(140f));
//						stat.modifier = (Stat.Modifier)EditorGUILayout.EnumPopup(stat.modifier);
//						if(stat.amount == 0)
//							item.stats.Remove(stat);
//					}
//					else {
//						int val = EditorGUILayout.IntField(Stat.GetStatIdByIndex(i).ToString(), 0, GUILayout.Width(140f));
//						if (val != 0) {
//							Stat newStat = new Stat();
//							newStat.id = statID;
//							newStat.amount = val;
//							item.stats.Add(newStat);
//						}
//					}
//				}
//				GUILayout.EndHorizontal();
//			}
//			EditorGUI.indentLevel--;



			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Activation Requirements");
			EditorGUI.indentLevel++;
			
			//item.activationReq.Clear();
			GUILayout.BeginHorizontal(); {
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Add")) {
					item.activationRequirements.Add(new GeneActivationRequirement());
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();

			List <GeneActivationRequirement> toDelete = new List<GeneActivationRequirement>();
			string[] allItems =  new string[itemManager.items.Count];
			foreach(BaseItem i in itemManager.items)
				allItems[itemManager.items.IndexOf(i)] = i.itemName;

			foreach(GeneActivationRequirement gr in item.activationRequirements) {
				GUILayout.BeginHorizontal(); 
				int index = 0;
				if(gr.item != null) {
					BaseItem itm = itemManager.GetBaseItemWithName(gr.item.itemName);
					index = itemManager.items.IndexOf(itm);
				}
					
				int oldIndexValue = index;
				index = EditorGUILayout.Popup("Item", index, allItems, GUILayout.Width(180f));
				if(oldIndexValue != index || gr.item == null) {
					gr.item = itemManager.items[index];
				}
				gr.amountNeeded = EditorGUILayout.IntField("Amount", gr.amountNeeded, GUILayout.Width(140f));

				if(gr.amountNeeded == 0)
					gr.amountNeeded = 1;


				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Del", GUILayout.Width(35f)))
					toDelete.Add(gr);
				GUI.backgroundColor = Color.white;
				GUILayout.EndHorizontal();
			}
			
			foreach(GeneActivationRequirement gr in toDelete)
				item.activationRequirements.Remove(gr);


			NGUIEditorTools.DrawSeparator();
		}
	}
}






	