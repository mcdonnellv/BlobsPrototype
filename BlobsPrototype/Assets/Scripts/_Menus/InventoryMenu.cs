using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMenu : MonoBehaviour {

	public enum Tab {
		None = -1,
		GenesTab,
		ItemsTab,
	};

	public UITweener animationWindow;
	public UILabel windowTitleLabel;
	public UILabel storageCapacityLabel;
	public UISprite genesTab;
	public UISprite itemsTab;
	public GameObject grid;
	public GenesMenu genesMenu;
	public ItemsMenu itemsMenu;
	public ItemInfoPopup popup;
	Tab activeTab;
	GameManager2 gameManager;

	
	public void Show() {
		if(gameManager == null)
			gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();

		gameObject.SetActive(true);
		transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
		GenesTabPressed();
	}
	
	public void Hide() {
		popup.Hide();
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();
	}
	
	void DisableWindow() {
		animationWindow.onFinished.Clear();
		gameObject.SetActive(false);
	}

	public void GenesTabPressed() {
		activeTab = Tab.GenesTab;
		genesTab.alpha = 1f;
		itemsTab.alpha = .5f;
		RebuildSlots(gameManager.gameVars.inventoryGeneSlots);
		ClearContextText();
		genesMenu.Show();
		storageCapacityLabel.text = gameManager.geneManager.storedGenes.Count.ToString() + " / " + gameManager.gameVars.inventoryGeneSlots.ToString();
	}

	public void ItemsTabPressed() {
		activeTab = Tab.ItemsTab;
		genesTab.alpha = .5f;
		itemsTab.alpha = 1f;
		RebuildSlots(gameManager.gameVars.inventoryItemSlots);
		ClearContextText();
		itemsMenu.Show();
		storageCapacityLabel.text = gameManager.itemManager.storedItems.Count.ToString() + " / " + gameManager.gameVars.inventoryItemSlots.ToString();
	}

	void ClearGrid() {
		foreach(Transform c in grid.transform) {
			c.DestroyChildren();
		}
	}

	void ClearContextText() {
		popup.nameLabel.text = "";
		popup.rarityLabel.text = "";
		popup.infoLabel1.text = "";
		popup.infoLabel2.text = "";
		popup.infoLabel1.transform.DestroyChildren();
		popup.infoLabel2.transform.DestroyChildren();
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

	public void increaseCapacityButtonPressed() {
		switch (activeTab) {
		case Tab.GenesTab: gameManager.gameVars.inventoryGeneSlots +=5; GenesTabPressed(); break;
		case Tab.ItemsTab: gameManager.gameVars.inventoryItemSlots +=5; ItemsTabPressed(); break;
		}
	}

}
