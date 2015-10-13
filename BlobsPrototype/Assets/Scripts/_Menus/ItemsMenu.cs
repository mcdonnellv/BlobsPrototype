using UnityEngine;
using System.Collections;

public class ItemsMenu : MonoBehaviour {

	public GameObject grid;
	public ItemInfoPopup itemInfoPopup;
	GameObject itemSlotHighlight;
	int selectedIndex = -1;
	ItemManager itemManager;

	public void Show() {
		itemManager = GameObject.Find ("ItemManager").GetComponent<ItemManager>();
		
		foreach(Item i in itemManager.storedItems) {
			if(i == null)
				continue;
			Transform parentSocket = grid.transform.GetChild(itemManager.storedItems.IndexOf(i));
			
			GameObject go = i.CreateItemGameObject();
			go.transform.parent = parentSocket;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
		}
		
		if(selectedIndex == -1 && itemManager.storedItems.Count > 0)
			ShowInfoForItem(itemManager.storedItems[0]);
		else if(selectedIndex != -1 && selectedIndex < itemManager.storedItems.Count)
			ShowInfoForItem(itemManager.storedItems[selectedIndex]);
		else if(itemManager.storedItems.Count == 0) {
			if(itemSlotHighlight)
				itemSlotHighlight.gameObject.SetActive(false);
			selectedIndex = -1;
			itemInfoPopup.nameLabel.text = "";
			itemInfoPopup.rarityLabel.text = "";
			itemInfoPopup.infoLabel1.text = "";
			itemInfoPopup.infoLabel2.text = "";
			itemInfoPopup.Hide();
		}
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
		HudManager hudManager = GameObject.Find ("HudManager").GetComponent<HudManager>();
		itemInfoPopup.Show();
		bool showDeleteButton = hudManager.inventoryMenu.mode == InventoryMenu.Mode.Inventory;
		itemInfoPopup.deleteButton.gameObject.SetActive(showDeleteButton);
		Item item = itemPointer.item;
		Transform parentSocket = itemPointer.transform.parent;
		selectedIndex = parentSocket.GetSiblingIndex();

		if(itemSlotHighlight != null)
			GameObject.Destroy(itemSlotHighlight);
		itemSlotHighlight = (GameObject)GameObject.Instantiate(Resources.Load("Gene Slot Highlight"));
		itemSlotHighlight.transform.parent = parentSocket;
		itemSlotHighlight.transform.localScale = new Vector3(1f,1f,1f);
		itemSlotHighlight.GetComponent<UISprite>().depth = parentSocket.GetComponent<UISprite>().depth;
		itemSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
		
		itemInfoPopup.nameLabel.text = item.itemName;
		itemInfoPopup.rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + "[-]";
		itemInfoPopup.infoLabel1.text = item.description;
		itemInfoPopup.infoLabel2.text = "";
		UISprite originalIcon = itemPointer.gameObject.GetComponent<UISprite>();
		itemInfoPopup.icon.atlas = originalIcon.atlas;
		itemInfoPopup.icon.spriteName = originalIcon.spriteName;
	}

	public Item GetItemFromIndex(int index) {
		if(index < 0 && index >= grid.transform.childCount)
			return null;

		Transform socket = grid.transform.GetChild(index);
		ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
		return ip.item;
	}

	public Item GetSelectedItem() {
		return GetItemFromIndex(selectedIndex);
	}

	public void DeteteSelectedItem() {
		Transform socket = grid.transform.GetChild(selectedIndex);
		ItemPointer ip = socket.GetComponentInChildren<ItemPointer>();
		Item item = ip.item;
		socket.DestroyChildren();
		itemManager.storedItems.Remove(item);

		selectedIndex = -1;
		itemInfoPopup.Hide();
	}

	void Update() {
		
		if(itemSlotHighlight != null)
			itemSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
	}
}
