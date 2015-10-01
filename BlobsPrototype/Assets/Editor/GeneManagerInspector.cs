using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GeneManager))]
public class GeneManagerInspector : Editor {
	GeneManager mm;
	ItemManager im;
	string newName = "";
	static int mIndex = 0;
	bool mConfirmDelete = false;
	static GeneReq.Identifier geneReqIdTemp;
	static Stat.Identifier statIdTemp;


	public override void OnInspectorGUI(){
		NGUIEditorTools.SetLabelWidth(80f);
		mm = (GeneManager)target;

		if(im == null)
			im = GameObject.Find("ItemManager").GetComponent<ItemManager>();

		Gene item = null;

		if (mm.genes == null || mm.genes.Count == 0)
			mIndex = 0;
		else{
			mIndex = Mathf.Clamp(mIndex, 0, mm.genes.Count - 1);
			item = mm.genes[mIndex];
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
					mm.genes.RemoveAt(mIndex);
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
				if (GUILayout.Button("New Gene") && mm.DoesNameExistInList(newName) == false){
					Gene g = new Gene();
					g.itemName = newName;
					if(item != null) {
						if(newName == "")
							g.itemName = item.itemName + " copy";
						g.description = item.description;
						g.type = item.type;
						g.quality = item.quality;
						g.stats = item.stats.ToList();
					}
					mm.genes.Add(g);
					mIndex = mm.genes.Count - 1;
					newName = "";
					item = g;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			if(GUILayout.Button ("Sort"))
				mm.genes = mm.genes.OrderBy(x => x.type).ThenByDescending(x => x.quality).ThenBy(x => x.itemName).ToList();

			if (item != null) {
				NGUIEditorTools.DrawSeparator();
				
				// Navigation section
				GUILayout.BeginHorizontal();{
					if (mIndex == 0) GUI.color = Color.grey;
					if (GUILayout.Button("<<")) { mConfirmDelete = false; --mIndex; }
					GUI.color = Color.white;
					mIndex = EditorGUILayout.IntField(mIndex + 1, GUILayout.Width(40f)) - 1;
					GUILayout.Label("/ " + mm.genes.Count, GUILayout.Width(40f));
					if (mIndex + 1 == mm.genes.Count) GUI.color = Color.grey;
					if (GUILayout.Button(">>")) { mConfirmDelete = false; ++mIndex; }
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
				NGUIEditorTools.DrawSeparator();
			}


			// Item name and delete item button
			GUILayout.BeginHorizontal();{
				string itemName = EditorGUILayout.TextField("Gene Name", item.itemName);
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					mConfirmDelete = true;
				GUI.backgroundColor = Color.white;
				if (!itemName.Equals(item.itemName) && mm.DoesNameExistInList(itemName) == false)
					item.itemName = itemName;
			}
			GUILayout.EndHorizontal();
			item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));
			item.type = (Gene.GeneType)EditorGUILayout.EnumPopup("Gene Type: ", item.type);
			item.active = (item.type != Gene.GeneType.MonsterGene);
			item.quality = (Quality)EditorGUILayout.EnumPopup("Quality: ", item.quality);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Stats");
			EditorGUI.indentLevel++;
			if(item.stats == null)
				item.stats = new List<Stat>();

			for(int i=0; i < (int)Stat.Identifier.StatIdCount; i++) {
				Stat.Identifier statID = (Stat.Identifier)i;
				Stat stat = null;
				foreach (Stat s in item.stats) {
					if(s.id == statID)
						stat = s;
				}

				GUILayout.BeginHorizontal();{
					if(stat != null) {
						stat.amount = EditorGUILayout.IntField(Stat.GetStatIdByIndex(i).ToString(), stat.amount, GUILayout.Width(140f));
						stat.modifier = (Stat.Modifier)EditorGUILayout.EnumPopup(stat.modifier);
						if(stat.amount == 0)
							item.stats.Remove(stat);
					}
					else {
						int val = EditorGUILayout.IntField(Stat.GetStatIdByIndex(i).ToString(), 0, GUILayout.Width(140f));
						if (val != 0) {
							Stat newStat = new Stat();
							newStat.id = statID;
							newStat.amount = val;
							item.stats.Add(newStat);
						}
					}
				}
				GUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel--;



			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Activation Requirements");
			EditorGUI.indentLevel++;
			
			//item.activationReq.Clear();
			GUILayout.BeginHorizontal(); {
				geneReqIdTemp = (GeneReq.Identifier)EditorGUILayout.EnumPopup(geneReqIdTemp);
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Add")) {
					bool addIt = true;
					foreach(GeneReq gr in item.activationReq)
						if (gr.id == GeneReq.Identifier.LvlReq && geneReqIdTemp == GeneReq.Identifier.LvlReq)
							addIt = false;
					
					
					if (geneReqIdTemp == GeneReq.Identifier.LvlReq) {
						foreach(GeneReq gr in item.activationReq)
							if (gr.id == geneReqIdTemp)
								addIt = false;
					}
					if (addIt) {
						GeneReq newGeneReq = new GeneReq();
						newGeneReq.id = geneReqIdTemp;
						item.activationReq.Add(newGeneReq);
					}
					
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();

			List <GeneReq> toDelete = new List<GeneReq>();
			foreach(GeneReq gr in item.activationReq) {
				switch(gr.id) {
				case GeneReq.Identifier.LvlReq:
					GUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Blob Level      >=", GUILayout.Width(125f));
						gr.amount = EditorGUILayout.IntField(gr.amount, GUILayout.Width(50f));
					break;

				case GeneReq.Identifier.StatReq: 
					GUILayout.BeginHorizontal();
						gr.statId = (Stat.Identifier)EditorGUILayout.EnumPopup(gr.statId, GUILayout.Width(80f));
						EditorGUILayout.LabelField(">=", GUILayout.Width(40f));
						gr.amount = EditorGUILayout.IntField(gr.amount, GUILayout.Width(50f));
					break;


				case GeneReq.Identifier.ConsumeReq:
					GUILayout.BeginHorizontal(); 
					string[] allItems =  new string[im.items.Count];
					foreach(Item i in im.items)
						allItems[im.items.IndexOf(i)] = i.itemName;
					int index = EditorGUILayout.Popup("Item", 0, allItems, GUILayout.Width(180f));
					break;
				}


				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Del", GUILayout.Width(35f)))
					toDelete.Add(gr);
				GUI.backgroundColor = Color.white;
				GUILayout.EndHorizontal();
			}
			
			foreach(GeneReq gr in toDelete)
				item.activationReq.Remove(gr);


			switch(item.type) {
			case Gene.GeneType.MonsterGene: {



//				for(int i=0; i < (int)Stat.Identifier.StatIdCount; i++) {
//					Stat.Identifier statID = (Stat.Identifier)i;
//					Stat stat = null;
//					foreach (GeneReq s in item.activationReq) {
//						if(s.id == statID)
//							stat = s;
//					}
//					
//					GUILayout.BeginHorizontal(); {
//						if(stat != null) {
//							stat.amount = EditorGUILayout.IntField(Stat.GetStatIdByIndex(i).ToString() + " >=", stat.amount, GUILayout.Width(160f));
//							if(stat.amount == 0)
//								item.activationReq.Remove(stat);
//						}
//						else {
//							int val = EditorGUILayout.IntField(Stat.GetStatIdByIndex(i).ToString() + " >=", 0, GUILayout.Width(160f));
//							if (val != 0) {
//								Stat newStat = new Stat();
//								newStat.id = statID;
//								newStat.amount = val;
//								item.activationReq.Add(newStat);
//							}
//						}
//					}
//					GUILayout.EndHorizontal();
//				}
				EditorGUI.indentLevel--;
			}
				break;

			}

			NGUIEditorTools.DrawSeparator();
		}
	}
}
	