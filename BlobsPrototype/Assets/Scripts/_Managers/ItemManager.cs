using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ItemManager : MonoBehaviour {
	public List<BaseItem> items = new List<BaseItem>(); // All items
	public List<Item> storedItems = new List<Item>();
	public UIAtlas iconAtlas;

	public void FirstTimeSetup() {
		foreach(BaseItem b in items) {
			AddItemToStorage(b, 3);
		}
	}

	public void AddItemToStorage(BaseItem b) { AddItemToStorage(b, 1); }
	public void RemoveItemFromStorage(BaseItem b) { RemoveItemFromStorage(b, 1); }

	public void AddItemToStorage(BaseItem b, int count) {
		Item i = GetItemFromStorageByName(b.itemName);
		if(i == null || (i.count + count > i.maxStack)) {
			i = new Item(b);
			storedItems.Add(i);
		}
		
		i.count+=count;
	}

	public void RemoveItemFromStorage(BaseItem b, int count) {
		Item i = GetItemFromStorageByName(b.itemName);
		if(i == null)
			return;
		i.count-=count;
		if(i.count <=0)
			storedItems.Remove(i);
	}

	public bool DoesNameExistInList(string nameParam){
		foreach(BaseItem i in storedItems)
			if (i.itemName == nameParam)
				return true;
		return false;
	}

	public Item GetItemFromStorageByName(string nameParam) {
		foreach(Item i in storedItems)
			if(nameParam == i.itemName)
				return i;
		return null;
	}
}
