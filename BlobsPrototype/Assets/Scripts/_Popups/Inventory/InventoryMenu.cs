using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryMenu : GenericGameMenu {

	public enum Tab {
		None = -1,
		GenesTab,
		ItemsTab,
	};

	public enum Mode {
		None = -1,
		Inventory,
		Feed,
		AddGene,
	};
	
	public UILabel storageCapacityLabel;
	public UILabel instructionalLabel;
	public UISprite genesTab;
	public UISprite itemsTab;
	public GameObject grid;
	public GameObject storageContainer;
	public GameObject actionContainer;
	public GenesMenu genesMenu;
	public ItemsMenu itemsMenu;
	public ItemInfoPopup itemInfoPopup;
	Tab activeTab;
	GameManager2 gameManager;
	public Mode mode = Mode.None;
	public Mode reopenMode = Mode.None;

	

	public void Pressed() {	Show(); }

	public void Show() { Show(Mode.Inventory); }

	public void Show(Mode modeParam) {
		if(IsDisplayed()) {
			if (mode != Mode.None) {
				reopenMode = modeParam;
				Hide();
				return;
			}
		}

		gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();
		mode = modeParam;
		base.Show();

		switch(mode) {
		case Mode.None:	
		case Mode.Inventory:	
			ActivateGenesTab();
			instructionalLabel.text = "";
			headerLabel.text = "INVENTORY";
			genesTab.gameObject.SetActive(true);
			itemsTab.gameObject.SetActive(true);
			actionContainer.gameObject.SetActive(false);
			storageContainer.gameObject.SetActive(true);
			break;

		case Mode.Feed:
			ActivateItemsTab();
			genesTab.gameObject.SetActive(false);
			itemsTab.gameObject.SetActive(false);
			instructionalLabel.text = "Select an item";
			headerLabel.text = "FEED BLOB";
			actionContainer.gameObject.SetActive(true);
			storageContainer.gameObject.SetActive(false);
			actionContainer.GetComponentInChildren<UILabel>().text = "FEED";
			break;

		case Mode.AddGene:
			ActivateGenesTab();
			instructionalLabel.text = "Chose a gene to add";
			headerLabel.text = "ADD GENE";
			genesTab.gameObject.SetActive(false);
			itemsTab.gameObject.SetActive(false);
			actionContainer.gameObject.SetActive(true);
			storageContainer.gameObject.SetActive(false);
			actionContainer.GetComponentInChildren<UILabel>().text = "ADD";
			break;
		}
	}


	public void Hide() {
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

	public void GenesTabPressed() { 
		ActivateGenesTab();
		genesMenu.ShowInfo();

	}

	public void ItemsTabPressed() { 
		ActivateItemsTab();
		itemsMenu.ShowInfo();
	}


	public void ActivateGenesTab() {
		activeTab = Tab.GenesTab;
		genesTab.alpha = 1f;
		itemsTab.alpha = .5f;
		RebuildSlots(gameManager.gameVars.inventoryGeneSlots);
		ClearContextText();
		genesMenu.Show();
		storageCapacityLabel.text = gameManager.geneManager.storedGenes.Count.ToString() + " / " + gameManager.gameVars.inventoryGeneSlots.ToString();
	}

	public void ActivateItemsTab() {
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
		itemInfoPopup.ClearFields();
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
		switch (activeTab) {
		case Tab.GenesTab: gameManager.gameVars.inventoryGeneSlots +=5; GenesTabPressed(); break;
		case Tab.ItemsTab: gameManager.gameVars.inventoryItemSlots +=5; ItemsTabPressed(); break;
		}
	}

	public void ActionButtonPressed() {
		switch(mode) {
		case Mode.Feed:
			Item item = itemsMenu.GetSelectedItem();
			if(item == null)
				gameManager.hudMan.popup.Show("Feed Blob", "No item selected");
			else
				gameManager.hudMan.popup.Show("Feed Blob", "Feed [EEBE63]" + item.itemName + "[-] to the blob?", this, "FeedConfirmed");
			break;
		case Mode.AddGene:
			Gene gene = genesMenu.GetSelectedGene();
			if(gene == null)
				gameManager.hudMan.popup.Show("Add Gene", "No gene selected");
			else
				gameManager.hudMan.popup.Show("Add Gene", "Add [EEBE63]" + gene.itemName + "[-] to the blob?", this, "GeneAddConfirmed");
			break;
		}
	}

	public void FeedConfirmed() {
		Item item = itemsMenu.GetSelectedItem();
		gameManager.hudMan.blobInfoContextMenu.blob.EatItem(item);
		itemsMenu.DeleteSelectedThing();
	}


	public void GeneAddConfirmed() {
		Gene gene = genesMenu.GetSelectedGene();
		gameManager.hudMan.blobInfoContextMenu.AddGeneToBlob(gene);
		genesMenu.DeleteSelectedThing();
		Hide();
	}

	public void DeteteSelectedItemAsk() {
		string itemName = "";
		switch(activeTab) {
		case Tab.GenesTab: itemName = genesMenu.GetSelectedGene().itemName; break; 
		case Tab.ItemsTab: itemName = itemsMenu.GetSelectedItem().itemName; break;
		}
		gameManager.hudMan.popup.Show("Delete", "Are you sure you want to delete [EEBE63]" + itemName + "[-]?", this, "DeteteSelectedItem");
	}

	public void DeteteSelectedItem() {
		switch(activeTab) {
		case Tab.GenesTab: genesMenu.DeleteSelectedThing(); break; 
		case Tab.ItemsTab: itemsMenu.DeleteSelectedThing(); break;
		}
	}

}
