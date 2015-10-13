using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenesMenu : MonoBehaviour {
	
	public GameObject grid;
	public ItemInfoPopup itemInfoPopup;
	GameObject geneSlotHighlight;
	int selectedIndex = -1;
	GeneManager geneManager;

	public void Show() {
		geneManager = GameObject.Find ("GeneManager").GetComponent<GeneManager>();

		foreach(Gene g in geneManager.storedGenes) {
			if(g == null)
				continue;
			Transform parentSocket = grid.transform.GetChild(geneManager.storedGenes.IndexOf(g));

			GameObject go = g.CreateGeneGameObject();
			go.transform.parent = parentSocket;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
		}

		if(selectedIndex == -1 && geneManager.storedGenes.Count > 0)
			ShowInfoForGene(geneManager.storedGenes[0]);
		else if(selectedIndex != -1 && selectedIndex < geneManager.storedGenes.Count)
			ShowInfoForGene(geneManager.storedGenes[selectedIndex]);
		else if(geneManager.storedGenes.Count == 0) {
			selectedIndex = -1;

			itemInfoPopup.nameLabel.text = "";
			itemInfoPopup.rarityLabel.text = "";
			itemInfoPopup.infoLabel1.text = "";
			itemInfoPopup.infoLabel2.text = "";
			itemInfoPopup.Hide();
		}
	}


	public void ShowInfoForGene(Gene gene) {
		GenePointer genePointer = null;
		foreach(Transform socket in grid.transform) {
			GenePointer gp = socket.GetComponentInChildren<GenePointer>();
			if(gp != null && gp.gene == gene)
				genePointer = gp;
		}

		if(genePointer != null)
			ShowInfoForGeneGameObject(genePointer);
	}


	public void ShowInfoForGeneGameObject(GenePointer genePointer) {
		HudManager hudManager = GameObject.Find ("HudManager").GetComponent<HudManager>();
		itemInfoPopup.Show();
		bool showDeleteButton = hudManager.inventoryMenu.mode == InventoryMenu.Mode.Inventory;
		itemInfoPopup.deleteButton.gameObject.SetActive(showDeleteButton);
		Gene gene = genePointer.gene;
		Transform parentSocket = genePointer.transform.parent;
		selectedIndex = parentSocket.GetSiblingIndex();

		if(geneSlotHighlight != null)
			GameObject.Destroy(geneSlotHighlight);
		geneSlotHighlight = (GameObject)GameObject.Instantiate(Resources.Load("Gene Slot Highlight"));
		geneSlotHighlight.transform.parent = parentSocket;
		geneSlotHighlight.transform.localScale = new Vector3(1f,1f,1f);
		geneSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
		geneSlotHighlight.GetComponent<UISprite>().depth = parentSocket.GetComponent<UISprite>().depth;
		itemInfoPopup.ShowInfoForGene(gene);
	}


	public Gene GetGeneFromIndex(int index) {
		if(index < 0 && index >= grid.transform.childCount)
			return null;

		Transform socket = grid.transform.GetChild(index);
		GenePointer gp = socket.GetComponentInChildren<GenePointer>();
		return gp.gene;
	}
	
	public Gene GetSelectedGene() {
		return GetGeneFromIndex(selectedIndex);
	}
	
	public void DeteteSelectedGene() {
		Transform socket = grid.transform.GetChild(selectedIndex);
		GenePointer gp = socket.GetComponentInChildren<GenePointer>();
		Gene gene = gp.gene;
		socket.DestroyChildren();
		geneManager.storedGenes.Remove(gene);
		
		selectedIndex = -1;
		itemInfoPopup.Hide();
	}

	void Update() {

		if(geneSlotHighlight != null)
			geneSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
	}
}
