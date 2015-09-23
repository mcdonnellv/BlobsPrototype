using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GeneManager))]
public class GeneManagerInspector : Editor 
{
	GeneManager mm;
	bool showGenes = false;
	List <string>surnames = null;
	string newName = "";
	string giantstr;

	static int mIndex = 0;
	int preReqToAddIndex = 0;
	int preReqToDeleteIndex = 0;
	int exclusiveToAddIndex = 0;
	int exclusiveToDeleteIndex = 0;
	Gene.GeneActivationRequirements activationReqToAdd = 0;
	int activationReqToDeleteIndex = 0;
	bool mConfirmDelete = false;



	public override void OnInspectorGUI()
	{
		NGUIEditorTools.SetLabelWidth(80f);
		mm = (GeneManager)target;
		Gene item = null;

		if (mm.genes == null || mm.genes.Count == 0)
		{
			mIndex = 0;
		}
		else
		{
			mIndex = Mathf.Clamp(mIndex, 0, mm.genes.Count - 1);
			item = mm.genes[mIndex];
		}

		if (mConfirmDelete)
		{
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + item.geneName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();
			{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete"))
				{
					mm.genes.RemoveAt(mIndex);
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			// "New" button
			
			EditorGUILayout.BeginHorizontal();
			newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("New Gene") && mm.DoesNameExistInList(newName) == false && newName != "")
			{

				Gene g = new Gene();
				g.geneName = newName;
				mm.genes.Add(g);
				mIndex = mm.genes.Count - 1;
				newName = "";
				item = g;
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
			if(GUILayout.Button ("Sort"))
			{
				mm.genes = mm.genes.OrderBy(x => x.type).ThenByDescending(x => x.quality).ThenBy(x => x.geneName).ToList();
			}

			if (item != null)
			{
				NGUIEditorTools.DrawSeparator();
				
				// Navigation section
				GUILayout.BeginHorizontal();
				{
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
			GUILayout.BeginHorizontal();
			{
				string itemName = EditorGUILayout.TextField("Gene Name", item.geneName);
				
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete", GUILayout.Width(55f)))
				{
					mConfirmDelete = true;
				}
				GUI.backgroundColor = Color.white;
				
				if (!itemName.Equals(item.geneName) && mm.DoesNameExistInList(newName) == false)
				{
					item.geneName = itemName;
				}
			}

			GUILayout.EndHorizontal();
			item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));
			item.type = (Gene.Type)EditorGUILayout.EnumPopup("Gene Type: ", item.type);
			EditorGUILayout.Space();



			// Prerequisites
			List<string> geneNames = new List<string>();
			foreach(Gene g in mm.genes)
			{
				bool found = false;
				
				if(g.geneName == item.geneName)
					found = true;
				
				if(item.preRequisites.Count > 0)
					foreach(string s in item.preRequisites)
						if(g.geneName == s)
							found = true;
				
				if(!found)
					geneNames.Add(g.geneName);
			}

			EditorGUILayout.LabelField("Prerequisite Genes");
			EditorGUI.indentLevel++;
			NGUIEditorTools.SetLabelWidth(70f);
			GUILayout.BeginHorizontal();
			preReqToDeleteIndex = EditorGUILayout.Popup("Current:", preReqToDeleteIndex, 
			                                            item.preRequisites.ToArray(), GUILayout.Width(150f));
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("-", GUILayout.Width(20f)))
				item.preRequisites.RemoveAt(preReqToDeleteIndex);
			GUI.backgroundColor = Color.white;
			preReqToAddIndex = EditorGUILayout.Popup(preReqToAddIndex, geneNames.ToArray());
			if (GUILayout.Button("+", GUILayout.Width(20f)))
				item.preRequisites.Add(geneNames[preReqToAddIndex]);
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();


			// Activation Requirements
			List<string> activationRequirements = new List<string>();
			foreach(Gene.GeneActivationRequirements ar in item.activationRequirements)
				activationRequirements.Add(ar.ToString());
			EditorGUILayout.LabelField("Activation Requirements");
			EditorGUI.indentLevel++;
			GUILayout.BeginHorizontal();
			activationReqToDeleteIndex = EditorGUILayout.Popup("Current:", activationReqToDeleteIndex, 
			                                                   activationRequirements.ToArray(), GUILayout.Width(150f));
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("-", GUILayout.Width(20f)))
				item.activationRequirements.RemoveAt(activationReqToDeleteIndex);
			GUI.backgroundColor = Color.white;
			activationReqToAdd = (Gene.GeneActivationRequirements)EditorGUILayout.EnumPopup(activationReqToAdd);
			if (GUILayout.Button("+", GUILayout.Width(20f)))
				item.activationRequirements.Add(activationReqToAdd);
			NGUIEditorTools.SetLabelWidth(80f);
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();


			//Gene info
			item.quality = (Quality)EditorGUILayout.EnumPopup("Rarity: ", item.quality);
			item.geneStrength = (Gene.GeneStrength)EditorGUILayout.EnumPopup("Strength: ", item.geneStrength);
			item.negativeEffect = EditorGUILayout.Toggle("Sickness", item.negativeEffect);


			NGUIEditorTools.DrawSeparator();
			switch(item.type)
			{
			case Gene.Type.BodyColor:
				item.bodyColor = EditorGUILayout.ColorField("Body Color", item.bodyColor, GUILayout.Width(250f));
				break;
			}
		}
	}
}
	