using UnityEngine;
using System.Collections;

public class ItemsMenu : MonoBehaviour {

	public GameObject grid;
	public ItemInfoPopup itemInfoPopup;
	GameObject itemSlotHighlight;
	int selectedIndex = -1;
	ItemManager itemManager;
	Item selectedItem = null;

	public void Show() {
		itemManager = GameObject.Find ("ItemManager").GetComponent<ItemManager>();
		ResetSockets();
		foreach(Item i in itemManager.storedItems) {
			if(i == null)
				continue;
			Transform parentSocket = grid.transform.GetChild(itemManager.storedItems.IndexOf(i));
			UILabel countLabel = parentSocket.gameObject.GetComponentInChildren<UILabel>();
			GameObject go = i.CreateItemGameObject();
			UIGrid socketGrid = parentSocket.gameObject.GetComponentInChildren<UIGrid>();
			go.transform.parent = socketGrid.transform;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
			socketGrid.Reposition();
		}

		UpdateItemCounts();

		if(selectedIndex == -1 && itemManager.storedItems.Count > 0)
			selectedIndex = 0;
		
		if(itemManager.storedItems.Count == 0) {
			selectedIndex = -1;
			itemInfoPopup.ClearFields();
			itemInfoPopup.Hide();
		}
		
		if(selectedIndex < itemManager.storedItems.Count && selectedIndex >= 0) //check within bounds
			selectedItem = itemManager.storedItems[selectedIndex];

		if (selectedItem != null)
			Invoke("ShowInfoForSelectedItem", .3f);
	}

	void ResetSockets() {
		foreach(Transform inventorySocket in grid.transform) {
			UILabel countLabel = inventorySocket.gameObject.GetComponentInChildren<UILabel>();
			countLabel.text = "";
		}
	}

	void UpdateItemCounts() {
		foreach(Transform inventorySocket in grid.transform) {
			UILabel countLabel = inventorySocket.gameObject.GetComponentInChildren<UILabel>();
			ItemPointer ip = inventorySocket.GetComponentInChildren<ItemPointer>();
			Item item = null;
			if(ip != null)
				item = ip.item;

			if(item != null && item.count > 1)
				countLabel.text = item.count.ToString();
			else
				countLabel.text = "";
		}
	}


	public void ShowInfoForSelectedItem() {
		ShowInfoForItem(selectedItem);
	}
	
	public void ShowInfoForItem(Item item) {
		ItemPointer itemPointer = null;
		foreach(Transform socket in grid.transform) {
			ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
			if(ip != null && ip.item == item)
				itemPointer = ip;
		}
		
		if(itemPointer != null)
			ShowInfoForItemGameObject(itemPointer);
	}
	


	public void ShowInfoForItemGameObject(ItemPointer itemPointer) {
		if(itemPointer.item == itemInfoPopup.item)
			return;

		HudManager hudManager = GameObject.Find ("HudManager").GetComponent<HudManager>();
		itemInfoPopup.Show();
		bool showDeleteButton = hudManager.inventoryMenu.mode == InventoryMenu.Mode.Inventory;
		itemInfoPopup.deleteButton.gameObject.SetActive(showDeleteButton);
		Item item = itemPointer.item;
		Transform parentSocket = itemPointer.transform.parent.parent;
		selectedIndex = parentSocket.GetSiblingIndex();

		if(itemSlotHighlight != null)
			GameObject.Destroy(itemSlotHighlight);
		itemSlotHighlight = (GameObject)GameObject.Instantiate(Resources.Load("Gene Slot Highlight"));
		itemSlotHighlight.transform.parent = parentSocket;
		itemSlotHighlight.transform.localScale = new Vector3(1f,1f,1f);
		UISprite slotSpriteBg = parentSocket.GetComponent<UISprite>();
		UISprite highLightSprite = itemSlotHighlight.GetComponent<UISprite>();
		highLightSprite.depth = slotSpriteBg.depth;
		itemSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
		itemInfoPopup.ShowInfoForItem(item);
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


	public Item GetSelectedItem() {
		return GetItemFromIndex(selectedIndex);
	}


	public void DeteteSelectedItem() {
		Transform socket = grid.transform.GetChild(selectedIndex);
		ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
		Item item = ip.item;
		itemManager.RemoveItemFromStorage(item);

		if(item.count <= 0) {
			GameObject.Destroy(ip.gameObject);
			GameObject.Destroy(itemSlotHighlight);
			selectedIndex = -1;
		}

		itemInfoPopup.Hide();
		UpdateItemCounts();
	}

	void Update() {
		
		if(itemSlotHighlight != null)
			itemSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
	}
}
