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
	List <string>surnames = null;
	string newName;
	string giantstr;


	public override void OnInspectorGUI()
	{
		mm = (GeneManager)target;
		genes = mm.genes.OrderBy(x => x.type).ToList();
		EditorGUILayout.LabelField("Total Genes: " + genes.Count);

		showGenes = EditorGUILayout.Foldout(showGenes,"Genes (" + genes.Count + ")");
		if (showGenes)
		{
			for (int i=0;i<genes.Count; i++)
			{
				Gene gene = genes[i];

				EditorGUI.indentLevel = 1;
				EditorGUILayout.BeginHorizontal();
				gene.geneName = EditorGUILayout.TextField("Name: ", gene.geneName, GUILayout.Width(200f));
				GUI.backgroundColor = Color.red;
				if(GUILayout.Button("Delete"))
				{
					mm.genes.Remove(gene);
					genes.Remove(gene);
					i--;
				}
				GUI.backgroundColor = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel += 1;
				gene.description = EditorGUILayout.TextField("Description: ", gene.description);
				gene.preRequisite = EditorGUILayout.TextField("Prereq: ", gene.preRequisite);
				if (mm.DoesNameExistInList(gene.preRequisite) == false)
				    gene.preRequisite = "";
				gene.rarity = (Gene.Rarity)EditorGUILayout.EnumPopup("Rarity: ", gene.rarity);
				gene.geneStrength = (Gene.GeneStrength)EditorGUILayout.EnumPopup("Strength: ", gene.geneStrength);
				gene.negativeEffect = EditorGUILayout.Toggle("Negative Effect", gene.negativeEffect);
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
					m.negativeEffect = true;
					mm.genes.Add(m);
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
	