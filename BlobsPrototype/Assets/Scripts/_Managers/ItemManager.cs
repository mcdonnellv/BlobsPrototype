using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ItemManager : MonoBehaviour {
	public List<Item> items = new List<Item>(); // All items
	public List<Item> storedItems = new List<Item>();
	public UIAtlas iconAtlas;

	public void FirstTimeSetup() {
		foreach(Item i in items)
			storedItems.Add(i);
	}

	public bool DoesNameExistInList(string nameParam){
		foreach(Item i in items)
			if (i.itemName == nameParam)
				return true;
		return false;
	}
}
