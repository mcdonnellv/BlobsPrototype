using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class StoreMenu : GenericGameMenu {
	public StoreMenuListGrid grid;
	public UILabel goldLabel;
	public UILabel titleLabel;
	public UILabel rarityLabel;
	public UILabel descriptionLabel;
	public UILabel reserachLabel;
	public UISprite icon;
	public UIGrid materialsGrid;
	public UIButton craftButton;
	public UIButton researchButton;
	public GameObject researchPanel;
	public GameObject geneDetailsPanel;
	public List<RequirementContainer> materials;

	[HideInInspector] public StoreListCell selectedCell;
	public StoreManager storeManager { get {return StoreManager.storeManager;} }
	public ItemManager itemManager { get {return ItemManager.itemManager;} }
	public HudManager hudManager { get {return HudManager.hudManager;} }
	public GameManager2 gameManager { get {return GameManager2.gameManager;} }
	public GeneManager geneManager { get {return GeneManager.geneManager;} }

	BaseGene gene;
	
	public void UpdateGold() { goldLabel.text = GameManager2.gameManager.gameVars.gold.ToString() + "[gold]"; }
	public void Pressed() {	Show(); }


	public override void Show() {
		base.Show();
		storeManager.BuildStoreItems();
		UpdateGold();
		grid.SetupStoreCells();
		selectedCell = null;
		PressCellByIndex(0);
	}


	void PressCellByIndex(int index) {
		StoreListCell cell = grid.GetCellFromIndex(index);
		if(cell != null) cell.Pressed();
	}


	public void SelectCellByIndex(int index) {
		GeneStoreItem item = storeManager.baseGenes[index];
		StoreListCell cell = grid.GetCellFromIndex(index);
		cell.newLabel.gameObject.SetActive(false);
		selectedCell = cell;
		item.alreadySeen = true;
		PopulateInfoFromGene(geneManager.GetBaseGeneByID(cell.baseGeneId)); 
	}


	public void PopulateInfoFromGene(BaseGene g) {
		if(g == null)
			return;
		bool requirementsMet = true;
		gene = g;
		titleLabel.text = gene.itemName.ToUpper();
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		icon.spriteName = g.iconName;
		icon.atlas = g.iconAtlas;

		GeneStoreItem gsi = storeManager.GetGeneStoreItemWithId(g.id);
		if(gsi.researched == false) {
			geneDetailsPanel.SetActive(false);
			researchPanel.SetActive(true);
			researchButton.isEnabled = !gsi.researching;
			if(gsi.researching) {
				TimeSpan ts = gsi.actionReadyTime - System.DateTime.Now;
				reserachLabel.text = "Time: " + GlobalDefines.TimeToString(ts);
			}
			else
				reserachLabel.text = "Cost: " + gsi.researchCost.ToString() + "[gold]\nTime: " + GlobalDefines.TimeToString(StoreManager.reseachTime);
		}
		else {
			geneDetailsPanel.SetActive(true);
			researchPanel.SetActive(false);
			descriptionLabel.text = gene.description;
			int i = 0;
			foreach(GeneActivationRequirement req in gene.activationRequirements) {
				RequirementContainer mat = materials[i];
				mat.gameObject.SetActive(true);
				BaseItem item = ItemManager.itemManager.GetBaseItemByID(req.itemId);
				mat.icon.atlas = item.iconAtlas;
				mat.icon.spriteName = item.iconName;
				mat.icon.color = ColorDefines.IconColorFromIndex(item.iconTintIndex);
				int inventoryCount = itemManager.GetItemCountById(req.itemId);
				mat.titleLabel.text = item.itemName;
				mat.amoutLabel.text = inventoryCount.ToString() + " / " + req.amountNeeded.ToString();
				
				if(inventoryCount >= req.amountNeeded) {
					mat.titleLabel.color = Color.white;
					mat.amoutLabel.color = Color.white;
				}
				else {
					requirementsMet = false;
					mat.titleLabel.color = ColorDefines.inactiveTextColor;
					mat.amoutLabel.color = ColorDefines.inactiveTextColor;
				}
				i++;
			}
			
			for(;i<materials.Count;i++) {
				materials[i].gameObject.SetActive(false);
			}
			
			craftButton.isEnabled = requirementsMet;
		}
	}


	public void CraftButtonPressed() {
		int cost = gene.sellValue * 10;
		if(gameManager.gameVars.gold < cost)
			hudManager.ShowError("Not enough gold");
		else
			hudManager.popup.Show("Craft Gene", "Are you sure you want to craft [EEBE63]" + titleLabel.text + "[-] for " + selectedCell.costLabel.text + "?", this, "CraftGene");
	
	}


	public void ResearchButtonPressed() {
		GeneStoreItem gsi = storeManager.GetGeneStoreItemWithId(gene.id);
		int cost = gsi.researchCost;
		if(gameManager.gameVars.gold < cost)
			hudManager.ShowError("Not enough gold");
		else
			hudManager.popup.Show("Research Gene", "Research [EEBE63]" + titleLabel.text + "[-] for " + selectedCell.costLabel.text + "?", this, "ResearchGene");
		
	}


	void ResearchGene() {
		GeneStoreItem gsi = storeManager.GetGeneStoreItemWithId(gene.id);
		int cost = gsi.researchCost;
		gsi.actionDuration = StoreManager.reseachTime;
		gsi.actionReadyTime = System.DateTime.Now + gsi.actionDuration;
		gsi.researching = true;
		researchButton.isEnabled = false;
	}


	void CraftGene() {
		int cost = gene.sellValue * 10;
		gameManager.AddGold(-cost);
		UpdateGold();
		foreach(GeneActivationRequirement req in gene.activationRequirements)
			itemManager.RemoveItemFromStorage(req.itemId, req.amountNeeded);
		hudManager.blobInfoContextMenu.AddGeneToBlob(new Gene(gene));
		hudManager.ShowNotice("Gene Added to Blob");
		base.Hide();
	}


	public void UpdateGeneStoreItem(GeneStoreItem g) {
		StoreListCell cell = grid.GetCellFromStoreItem(g);
		BaseGene baseGene = geneManager.GetBaseGeneByID(g.baseGeneId);
		cell.costLabel.color = ColorDefines.inactiveTextColor;
		cell.titleLabel.color = ColorDefines.inactiveTextColor;
		cell.costLabel.text = "";
		if(g.researched) {
			PopulateInfoFromGene(gene);
			cell.costLabel.text = (baseGene.sellValue * 10f).ToString() + "[gold]";
			if(ItemManager.itemManager.HaveRequiredMaterialsForGene(baseGene)) {
				cell.costLabel.color = Color.white;
				cell.titleLabel.color = Color.white;
			}
		}
	}

	void UpdateTimeForCell(GeneStoreItem g, StoreListCell cell) {
		TimeSpan ts = g.actionReadyTime - System.DateTime.Now;
		cell.costLabel.text = GlobalDefines.TimeToString(ts, false, 1);

		if(selectedCell == cell) 
			reserachLabel.text = "Time Left: " + GlobalDefines.TimeToString(ts);
	}

	void Update() {
		foreach(GeneStoreItem g in storeManager.baseGenes) {
			if(g.researching)
				UpdateTimeForCell(g, grid.GetCellFromStoreItem(g));
		}
	}
}
