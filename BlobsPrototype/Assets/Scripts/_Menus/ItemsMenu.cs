using UnityEngine;
using System.Collections;

public class ItemsMenu : MonoBehaviour {

	public GameObject grid;
	public ItemInfoPopup popup;
	GameObject itemSlotHighlight;
	int selectedIndex = -1;
	
	public void Show() {
		ItemManager itemManager = GameObject.Find ("ItemManager").GetComponent<ItemManager>();
		
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
			itemSlotHighlight.gameObject.SetActive(false);
			selectedIndex = -1;
			popup.nameLabel.text = "";
			popup.rarityLabel.text = "";
			popup.infoLabel1.text = "";
			popup.infoLabel2.text = "";
			popup.Hide();
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
		popup.Show();
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
		
		popup.nameLabel.text = item.itemName;
		popup.rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + "[-]";
		popup.infoLabel1.text = item.description;
		popup.infoLabel2.text = "";
		UISprite originalIcon = itemPointer.gameObject.GetComponent<UISprite>();
		popup.icon.atlas = originalIcon.atlas;
		popup.icon.spriteName = originalIcon.spriteName;
	}

	void Update() {
		
		if(itemSlotHighlight != null)
			itemSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
	}
}
