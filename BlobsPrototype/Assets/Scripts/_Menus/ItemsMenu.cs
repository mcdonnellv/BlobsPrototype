using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsMenu : BaseThingMenu {

	Item selectedItem = null;
	private ItemManager _im;
	ItemManager itemManager { get{ if(_im == null) _im = GameObject.Find("ItemManager").GetComponent<ItemManager>(); return _im; } }
	public override void SetSelectedThing(int index) { selectedItem = itemManager.storedItems[index]; }
	public override BaseThing GetSelectedThing() {return (BaseThing)selectedItem; }
	public override void ShowInfo() { ShowInfoForItemGameObject(GetItemPointerFromItem(selectedItem)); }
	public override GameObject CreateGameObject(BaseThing g) { return ((Item)g).CreateItemGameObject(); }
	public Item GetSelectedItem() { return GetItemFromIndex(selectedIndex); }

	public void Show() {
		foreach(Item item in itemManager.storedItems) {
			if(item == null) continue;
			SetupThingInSocket(item ,itemManager.storedItems.IndexOf(item));
		}
		SelectedThingSetup(itemManager.storedItems.Count);
	}


	public override void UpdateItemCounts() {
		foreach(Transform inventorySocket in grid.transform) {
			UILabel countLabel = inventorySocket.gameObject.GetComponentInChildren<UILabel>();
			ItemPointer ip = inventorySocket.GetComponentInChildren<ItemPointer>();
			Item item = (ip == null) ? null : ip.item;
			countLabel.text = (item != null && item.count > 1) ? item.count.ToString() : "";
		}
	}
	

	public ItemPointer GetItemPointerFromItem(Item item) {
		foreach(Transform socket in grid.transform) {
			ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
			if(ip != null && ip.item == item)
				return ip;
		}
		return null;
	}


	public void ShowInfoForItemGameObject(ItemPointer itemPointer) {
		if(itemPointer == null) return;
		base.DisplayInfoPopup();
		base.CreateSlotHighlight(itemPointer.transform.parent);
		itemInfoPopup.PopulateInfoFromItem(itemPointer.item);
	}


	public Item GetItemFromIndex(int index) {
		if(index < 0 && index >= grid.transform.childCount)
			return null;
		Transform socket = grid.transform.GetChild(index);
		ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
		if(ip != null)
			return ip.item;
		return null;
	}


	public override void DeleteSelectedThing() {
		Transform socket = grid.transform.GetChild(selectedIndex);
		ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
		itemManager.RemoveItemFromStorage(ip.item);
		CleanUpAfterDelete(ip.item.count, ip.gameObject);
		UpdateItemCounts();
	}
}
