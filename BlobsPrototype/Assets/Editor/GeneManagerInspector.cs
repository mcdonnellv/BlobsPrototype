using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GeneManager))]
public class GeneManagerInspector : GenericManagerInspector {
	GeneManager geneManager;


	public override void OnInspectorGUI(){
		NGUIEditorTools.SetLabelWidth(80f);
		geneManager = (GeneManager)target;

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
			// Database icon atlas
			UIAtlas atlas = EditorGUILayout.ObjectField("Icon Atlas", geneManager.iconAtlas, typeof(UIAtlas), false) as UIAtlas;
			
			if (atlas != geneManager.iconAtlas) {
				geneManager.iconAtlas = atlas;
				foreach (BaseGene i in geneManager.genes) i.iconAtlas = atlas;
			}

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
						g.sellValue = item.sellValue;
						g.iconAtlas = item.iconAtlas;
						g.iconName = item.iconName;
						g.iconTintIndex = item.iconTintIndex;
						g.quality = item.quality;
						g.value = item.value;
						g.modifier = item.modifier;
						g.traitType = item.traitType;
						g.traitCondition = item.traitCondition;
						g.showInStore = item.showInStore;
						g.id = geneManager.GetNextAvailableID();
						foreach(GeneActivationRequirement req in item.activationRequirements)
							g.activationRequirements.Add(new GeneActivationRequirement(req));
					}
					geneManager.genes.Add(g);
					mIndex = geneManager.genes.Count - 1;
					newName = "";
					item = g;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			GUILayout.BeginHorizontal();
			if(GUILayout.Button ("Sort by ID"))
				geneManager.genes = geneManager.genes.OrderBy(x => x.id).ToList();
			if(GUILayout.Button ("Sort by Name"))
				geneManager.genes = geneManager.genes.OrderBy(x => x.itemName).ToList();
			GUILayout.EndHorizontal();

			if(item == null)
				return;

			NavigationSection(geneManager.genes.Count);

			// Item name and delete item button
			GUILayout.BeginHorizontal();
			{
				NGUIEditorTools.SetLabelWidth(20f);
				int newId = EditorGUILayout.IntField("ID", item.id, GUILayout.Width(60f));
				if(newId != item.id)
					item.id = (geneManager.DoesIdExistInList(newId)) ? geneManager.GetNextAvailableID() : item.id = newId;
				NGUIEditorTools.SetLabelWidth(50f);
				string itemName = EditorGUILayout.TextField("Name", item.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(item.itemName) && geneManager.DoesNameExistInList(itemName) == false)
					item.itemName = itemName;
			}
			GUILayout.EndHorizontal();
			item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(50f));
			item.quality = (Quality)EditorGUILayout.EnumPopup("Quality: ", item.quality);
			GUILayout.BeginHorizontal();{
				NGUIEditorTools.SetLabelWidth(40f);
				item.traitType = (TraitType)EditorGUILayout.EnumPopup("Trait: ", item.traitType, GUILayout.Width(200f));
				item.value = EditorGUILayout.IntField(item.value, GUILayout.Width(40f));
				item.modifier = (AbilityModifier)EditorGUILayout.EnumPopup(item.modifier);
			}
			GUILayout.EndHorizontal();
			NGUIEditorTools.SetLabelWidth(70f);
			item.traitCondition = (TraitCondition)EditorGUILayout.EnumPopup("Condition: ", item.traitCondition, GUILayout.Width(200f));
			item.sellValue = EditorGUILayout.IntField("Sell Value", item.sellValue, GUILayout.Width(160f));
			NGUIEditorTools.SetLabelWidth(100f);
			item.showInStore = EditorGUILayout.Toggle("Show In Store", item.showInStore, GUILayout.Width(160f));
			NGUIEditorTools.SetLabelWidth(70f);
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Activation Requirements");
			if(AddButtonPressed())
				item.activationRequirements.Add(new GeneActivationRequirement());
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel++;
			NGUIEditorTools.SetLabelWidth(70f);
			
			//item.activationReq.Clear();
			List <GeneActivationRequirement> toDelete = new List<GeneActivationRequirement>();

			foreach(GeneActivationRequirement req in item.activationRequirements) {
				GUILayout.BeginHorizontal(); 
				int index = 0;
				if(req.itemId >= 0) {
					BaseItem itm = itemManager.GetBaseItemByID(req.itemId);
					index = itemManager.items.IndexOf(itm);
				}
					
				NGUIEditorTools.SetLabelWidth(70f);
				int newIndex = EditorGUILayout.Popup("Item", index, allItems, GUILayout.Width(180f));
				if(newIndex != index || req.itemId < 0) 
					req.itemId = itemManager.items[newIndex].id;
				NGUIEditorTools.SetLabelWidth(30f);
				req.amountNeeded = EditorGUILayout.IntField("x", req.amountNeeded, GUILayout.Width(60f));
				req.amountNeeded = Mathf.Max(1, req.amountNeeded);

				if (DeleteButtonPressed())
					toDelete.Add(req);
				GUILayout.EndHorizontal();
			}

			NGUIEditorTools.SetLabelWidth(70f);
			foreach(GeneActivationRequirement gr in toDelete)
				item.activationRequirements.Remove(gr);

			NGUIEditorTools.DrawSeparator();

			if(atlas != null && item.iconAtlas == null) item.iconAtlas = atlas;
			SpriteSelection(item);
		}
	}
}






	