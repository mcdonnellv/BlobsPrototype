using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {
	public List<Item> items = new List<Item>(); // All items
	public List<Item> storedItems = new List<Item>();

	public bool DoesNameExistInList(string nameParam){
		foreach(Item i in items)
			if (i.itemName == nameParam)
				return true;
		return false;
	}
}
