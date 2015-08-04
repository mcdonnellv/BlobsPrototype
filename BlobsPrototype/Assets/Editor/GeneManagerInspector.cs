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
	List <Gene>genes = null;
	string newName;


	public override void OnInspectorGUI()
	{
		mm = (GeneManager)target;
		genes = mm.genes;//.ToList();
		EditorGUILayout.LabelField("Total Genes: " + genes.Count);


		showGenes = EditorGUILayout.Foldout(showGenes,"Genes (" + genes.Count + ")");
		if (showGenes)
		{
			foreach(Gene gene in genes)
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.BeginHorizontal();
				gene.geneName = EditorGUILayout.TextField("Name: ", gene.geneName, GUILayout.Width(200f));
				GUI.backgroundColor = Color.red;
				if(GUILayout.Button("Delete"))
					genes.Remove(gene);
				GUI.backgroundColor = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel += 1;
				gene.preRequisite = EditorGUILayout.TextField("Prereq: ", gene.preRequisite);
				if (mm.DoesNameExistInList(gene.preRequisite) == false)
				    gene.preRequisite = "";
				gene.rarity = (Gene.Rarity)EditorGUILayout.EnumPopup("Rarity: ", gene.rarity);
				gene.geneStrength = (Gene.GeneStrength)EditorGUILayout.EnumPopup("Strength: ", gene.geneStrength);
				gene.type = (Gene.Type)EditorGUILayout.EnumPopup(gene.type);
				switch(gene.type)
				{
				case Gene.Type.BodyColor:
					gene.bodyColor = EditorGUILayout.ColorField("Body Color", gene.bodyColor, GUILayout.Width(250f));
					break;
				}
				NGUIEditorTools.DrawSeparator();
			}

			EditorGUILayout.BeginHorizontal();
			newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
			GUI.backgroundColor = Color.green;
			if(GUILayout.Button("Add New Gene"))
			{
				if(mm.DoesNameExistInList(newName) == false)
				{
					Gene m = new Gene();
					m.geneName = newName;
					m.type = Gene.Type.BodyColor;
					m.geneStrength = Gene.GeneStrength.Normal;
					genes.Add(m);
				}
				newName = "";
			}
			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel = 0;
		}

//		if(GUILayout.Button("Delete All Genes"))
//		{
//			genes.Clear();
//			genes.TrimExcess();
//		}
	}
}
