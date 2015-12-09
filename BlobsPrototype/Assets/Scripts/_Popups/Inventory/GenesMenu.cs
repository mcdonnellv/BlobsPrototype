using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenesMenu : BaseThingMenu {
	
	Gene selectedGene = null;
	private GeneManager _gm;
	GeneManager geneManager { get{ if(_gm == null) _gm = GameObject.Find("GeneManager").GetComponent<GeneManager>(); return _gm; } }
	public override void SetSelectedThing(int index) { selectedGene = geneManager.storedGenes[index]; }
	public override BaseThing GetSelectedThing() {return (BaseThing)selectedGene; }
	public override void ShowInfo() { GenePressed(GetGenePointerFromGene(selectedGene)); }
	public override GameObject CreateGameObject(BaseThing g) { return ((Gene)g).CreateGeneGameObject(this); }
	public Gene GetSelectedGene() { return GetGeneFromIndex(selectedIndex); }

	public void Show() {
		foreach(Gene gene in geneManager.storedGenes) {
			if(gene == null) continue;
			SetupThingInSocket(gene ,geneManager.storedGenes.IndexOf(gene));
		}
		SelectedThingSetup(geneManager.storedGenes.Count);
	}


	public GenePointer GetGenePointerFromGene(Gene gene) {
		foreach(Transform socket in grid.transform) {
			GenePointer gp = socket.GetComponentInChildren<GenePointer>();
			if(gp != null && gp.gene == gene)
				return gp;
		}
		return null;
	}


	public Gene GetGeneFromIndex(int index) {
		if(index < 0 && index >= grid.transform.childCount)
			return null;
		Transform socket = grid.transform.GetChild(index);
		GenePointer gp = socket.GetComponentInChildren<GenePointer>();
		if(gp != null)
			return gp.gene;
		return null;
	}


	public override void DeleteSelectedThing() {
		Transform socket = grid.transform.GetChild(selectedIndex);
		GenePointer gp = socket.GetComponentInChildren<GenePointer>();
		geneManager.storedGenes.Remove(gp.gene);
		CleanUpAfterDelete(0, gp.gameObject);
	}


	public void GenePressed(GenePointer genePointer) {
		HudManager hudManager = HudManager.hudManager;
		ItemInfoPopup itemInfoPopup = hudManager.itemInfoPopup;
		if(genePointer == null) 
			return;
		itemInfoPopup.defaultStartPosition = PopupPosition.Right2;
		itemInfoPopup.Show(hudManager.inventoryMenu, genePointer.gene);
		itemInfoPopup.ShowDeleteButton(hudManager.inventoryMenu.mode == InventoryMenu.Mode.Inventory);
		CreateSlotHighlight(genePointer.transform.parent);
	}
}
