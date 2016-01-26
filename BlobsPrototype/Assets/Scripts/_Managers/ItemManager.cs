using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class ItemManager : MonoBehaviour {
	private static ItemManager _itemManager;
	public static ItemManager itemManager { get {if(_itemManager == null) _itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>(); return _itemManager; } }


	public List<BaseItem> items = new List<BaseItem>(); // All items
	public List<Item> storedItems = new List<Item>();
	public List<int> seenItems = new List<int>();
	public UIAtlas iconAtlas;

	public void AddItemToStorage(BaseItem b) { AddItemToStorage(b, 1); }
	public void RemoveItemFromStorage(int itemId, int ct) { RemoveItemFromStorage(GetBaseItemByID(itemId), ct); }
	public void RemoveItemFromStorage(BaseItem b) { RemoveItemFromStorage(b, 1); }
	public bool DoesNameExistInList(string nameParam){return (GetBaseItemWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseItemByID(idParam) != null); }
	public bool DoesItemExistInStorage(int itemId) { return (GetItemCountById(itemId) > 0); }
	

	public BaseItem GetBaseItemWithName(string nameParam) {
		foreach(BaseItem i in items)
			if (i.itemName == nameParam)
				return i;
		return null;
	}
	
	public BaseItem GetBaseItemByID(int idParam) {
		foreach(BaseItem i in items)
			if (i.id == idParam)
				return i;
		return null;
	}

	
	public int GetNextAvailableID() {
		int lowestIdVal = 0;
		List<BaseItem> sortedByID = items.OrderBy(x => x.id).ToList();
		foreach(BaseItem i in sortedByID)
			if (i.id == lowestIdVal)
				lowestIdVal++;
		return lowestIdVal;
	}


	public void FirstTimeSetup() {
		//foreach(BaseItem b in items)
		//	AddItemToStorage(b, 3);

		AddItemToStorage(GetBaseItemByID(21), 20);
		AddItemToStorage(GetBaseItemByID(26), 20);
	}


	public void AddItemToStorage(BaseItem b, int count) {
		Item i = GetItemFromStorageByName(b.itemName);
		if(i == null || (i.count + count > i.maxStack)) {
			i = new Item(b);
			storedItems.Add(i);
		}
		
		i.count+=count;
		if(seenItems.Contains(b.id) == false) {
			seenItems.Add(b.id);
			List<BaseGene> genes = GeneManager.geneManager.GetBaseGenesWithKeyItem(b.id);
			if(genes != null && genes.Count > 0) {
				if(genes.Count == 1)
					HudManager.hudManager.ShowNotice("A new Gene is now available in the Store");
				else
					HudManager.hudManager.ShowNotice(genes.Count.ToString() + " Genes are now available in the Store");
			}
		}
	}

	public void RemoveItemFromStorage(BaseItem b, int count) {
		Item i = GetItemFromStorageByName(b.itemName);
		if(i == null)
			return;
		i.count-=count;
		if(i.count <=0)
			storedItems.Remove(i);
	}
	

	public Item GetItemFromStorageByName(string nameParam) {
		foreach(Item i in storedItems)
			if(nameParam == i.itemName)
				return i;
		return null;
	}

	
	public int GetItemCountById(int itemId) {
		int ct = 0;
		foreach(Item i in storedItems)
			if(itemId == i.id)
				ct += i.count;
		return ct;
	}

	public bool HaveRequiredMaterialsForGene(BaseGene gene) {
		foreach(GeneActivationRequirement req in gene.activationRequirements)
			if(GetItemCountById(req.itemId) < req.amountNeeded)
				return false;
			return true;
	}

}
