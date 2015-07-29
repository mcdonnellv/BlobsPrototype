using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(MutationManager))]
public class MutationManagerInspector : Editor 
{
	MutationManager mm;
	bool showMutations = false;
	List <Mutation>mutations = null;
	Mutation.Type selectedType = 0;
	string newName;


	public override void OnInspectorGUI()
	{
		mm = (MutationManager)target;
		mutations = mm.mutations;//.ToList();
		EditorGUILayout.LabelField("Total Mutations: " + mutations.Count);


		showMutations = EditorGUILayout.Foldout(showMutations,"Mutations (" + mutations.Count + ")");
		if (showMutations)
		{
			foreach(Mutation mutation in mutations)
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.BeginHorizontal();
				mutation.mutationName = EditorGUILayout.TextField("Name: ", mutation.mutationName, GUILayout.Width(200f));
				GUI.backgroundColor = Color.red;
				if(GUILayout.Button("Delete"))
					mutations.Remove(mutation);
				GUI.backgroundColor = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel += 1;
				mutation.preRequisite = EditorGUILayout.TextField("Prereq: ", mutation.preRequisite);
				if (mm.DoesNameExistInList(mutation.preRequisite) == false)
				    mutation.preRequisite = "";
				mutation.rarity = (Mutation.Rarity)EditorGUILayout.EnumPopup("Rarity: ", mutation.rarity);
				mutation.type = (Mutation.Type)EditorGUILayout.EnumPopup(mutation.type);
				switch(mutation.type)
				{
				case Mutation.Type.BodyColor:
					mutation.bodyColor = EditorGUILayout.ColorField("Body Color", mutation.bodyColor, GUILayout.Width(250f));
					break;
				}
				NGUIEditorTools.DrawSeparator();
				//EditorGUILayout.Space();
			}

			EditorGUILayout.BeginHorizontal();
			newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
			GUI.backgroundColor = Color.green;
			if(GUILayout.Button("Add New Mutation"))
			{
				if(mm.DoesNameExistInList(newName) == false)
				{
					Mutation m = new Mutation();
					m.mutationName = newName;
					m.type = 0;
					mutations.Add(new Mutation());
				}
				newName = "";
			}
			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel = 0;
		}

//		if(GUILayout.Button("Delete All Mutations"))
//		{
//			mutations.Clear();
//			mutations.TrimExcess();
//		}
	}
}
