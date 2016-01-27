using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsMenu : BaseThingMenu {

	Item selectedItem = null;
	ItemManager itemManager { get { return ItemManager.itemManager; } }
	public override void SetSelectedThing(int index) { selectedItem = itemManager.storedItems[index]; }
	public override BaseThing GetSelectedThing() {return (BaseThing)selectedItem; }
	public override void ShowInfo() { ItemPressed(GetItemPointerFromItem(selectedItem)); }
	public override GameObject CreateGameObject(BaseThing g) { return ((Item)g).CreateItemGameObject(this); }
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


	public void ItemPressed(ItemPointer itemPointer) {
		HudManager hudManager = HudManager.hudManager;
		ItemInfoPopup itemInfoPopup = hudManager.itemInfoPopup;
		if(itemPointer == null) 
			return;
		itemInfoPopup.defaultStartPosition = PopupPosition.Right1;
		itemInfoPopup.Show(hudManager.inventoryMenu, itemPointer.item);
		itemInfoPopup.ShowDeleteButton(hudManager.inventoryMenu.mode == InventoryMenu.Mode.Inventory);
		CreateSlotHighlight(itemPointer.transform.parent);
	}
}
