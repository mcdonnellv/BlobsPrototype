using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMenu : GenericGameMenu {

	public enum Mode {
		None = -1,
		Inventory,
		Feed,
	};

	public UILabel storageCapacityLabel;
	public GameObject grid;
	public GameObject storageContainer;
	public GameObject actionContainer;
	public ItemsMenu itemsMenu;
	public ItemInfoPopup itemInfoPopup;
	ItemManager itemManager { get { return ItemManager.itemManager; } }
	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	public Mode mode = Mode.None;
	public Mode reopenMode = Mode.None;

	

	public void Pressed() {	Show(); }
	public override void Show() { Show(Mode.Inventory); }	
	void DeleteSelectedItem() { itemsMenu.DeleteSelectedThing(); }
	void DeleteButtonPressed() { DeleteSelectedItemAsk(); }


	public void Show(Mode modeParam) {
		if(IsDisplayed()) {
			if (mode != Mode.None) {
				reopenMode = modeParam;
				Hide();
				return;
			}
		}

		mode = modeParam;
		base.Show();

		RebuildSlots(gameManager.gameVars.inventoryItemSlots);
		itemInfoPopup.ClearFields();
		itemsMenu.Show();
		storageCapacityLabel.text = gameManager.itemManager.storedItems.Count.ToString() + " / " + gameManager.gameVars.inventoryItemSlots.ToString();

		switch(mode) {
		case Mode.None:	
		case Mode.Inventory:
			headerLabel.text = "INVENTORY";
			actionContainer.gameObject.SetActive(false);
			storageContainer.gameObject.SetActive(true);
			break;

		case Mode.Feed:
			headerLabel.text = "FEED BLOB";
			actionContainer.gameObject.SetActive(true);
			storageContainer.gameObject.SetActive(false);
			actionContainer.GetComponentInChildren<UILabel>().text = "FEED";
			DisableNonConsumables();
			break;
		}
	}


	void DisableNonConsumables() {

		int selIndex = -1;
		foreach(Item item in itemManager.storedItems) {
			if(item == null) continue;
			int index = itemManager.storedItems.IndexOf(item);
			if(item.consumable) {
				if(selIndex == -1)
					selIndex = index;
				continue;
			}
			Transform parentSocket = grid.transform.GetChild(index);
			BoxCollider collider = parentSocket.GetComponentInChildren<BoxCollider>();
			if(collider != null) {
				GameObject itemObject = collider.gameObject;
				UIButton button = itemObject.GetComponent<UIButton>();
				button.isEnabled = false;
			}
		}
		if(selIndex > 0)
			itemsMenu.SetSelectedThing(selIndex);
	}


	public override void Hide() {
		if(!IsDisplayed())
			return;

		switch(mode) {
		case Mode.Feed:
			gameManager.hudMan.blobInfoContextMenu.actionButton1.isEnabled = true;
			gameManager.hudMan.blobInfoContextMenu.actionButton2.isEnabled = true;
			break;
		}

		mode = Mode.None;
		base.Hide();
	}


	public override void Cleanup() {
		base.Cleanup();
		if(reopenMode != Mode.None)
			Invoke("Reopen", .01f);
	}


	void Reopen() {
		Show(reopenMode);
		reopenMode = Mode.None;
	}


	void ClearGrid() {
		foreach(Transform c in grid.transform) {
			c.DestroyChildren();
		}
	}



	void RebuildSlots(int slotCount) {
		grid.transform.DestroyChildren();
		for (int i = 0; i < slotCount; i++) {
			GameObject slot = (GameObject)GameObject.Instantiate(Resources.Load("Inventory Slot"));
			slot.transform.parent = grid.transform;
			slot.transform.localScale = new Vector3(1f,1f,1f);
			slot.GetComponent<UISprite>().depth = 1;
		}
		UIGrid gridComponent = grid.GetComponent<UIGrid>();
		gridComponent.Reposition();
	}


	public void IncreaseCapacityButtonPressed() {
		gameManager.gameVars.inventoryItemSlots +=5; 
		RebuildSlots(gameManager.gameVars.inventoryItemSlots);
		itemInfoPopup.ClearFields();
		itemsMenu.Show();
		storageCapacityLabel.text = gameManager.itemManager.storedItems.Count.ToString() + " / " + gameManager.gameVars.inventoryItemSlots.ToString();;
		itemsMenu.ShowInfo();
	}


	public void ActionButtonPressed() {
		switch(mode) {
		case Mode.Feed:
			Item item = itemsMenu.GetSelectedItem();
			if(item == null)
				gameManager.hudMan.ShowError("No item selected");
			else
				gameManager.hudMan.popup.Show("Feed Blob", "Feed [EEBE63]" + item.itemName + "[-] to the blob?", this, "FeedConfirmed");
			break;
		}
	}


	public void FeedConfirmed() {
		Item item = itemsMenu.GetSelectedItem();
		gameManager.hudMan.blobInfoContextMenu.blob.EatItem(item);
		itemsMenu.DeleteSelectedThing();
	}


	void DeleteSelectedItemAsk() {
		BaseThing thing = (BaseThing)itemsMenu.GetSelectedItem();
		if(thing.sellValue > 0)
			gameManager.hudMan.popup.Show("Sell", "Are you sure you want to sell [EEBE63]" + thing.itemName + "[-]?", this, "SellSelectedItem");
		else
			gameManager.hudMan.popup.Show("Delete", "Are you sure you want to delete [EEBE63]" + thing.itemName + "[-]?", this, "DeleteSelectedItem");
	}


	void SellSelectedItem() {
		BaseThing thing = (BaseThing)itemsMenu.GetSelectedItem();
		gameManager.AddGold(thing.sellValue);
		DeleteSelectedItem();
	}




}
