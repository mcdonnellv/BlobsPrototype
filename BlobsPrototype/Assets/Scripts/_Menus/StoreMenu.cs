using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreMenu : GenericGameMenu {
	public UIGrid grid;
	public UILabel goldLabel;
	[HideInInspector] public StoreCard selectedCard;
	public StoreManager storeManager { get {return StoreManager.storeManager;} }
	public HudManager hudManager { get {return HudManager.hudManager;} }
	public GameManager2 gameManager { get {return GameManager2.gameManager;} }
	public GeneManager geneManager { get {return GeneManager.geneManager;} }
	public ItemManager itemManager { get {return ItemManager.itemManager;} }

	public void Pressed() {	Show(); }

	public override void Show() {
		base.Show();
		storeManager.BuildStoreItems();
		UpdateGold();
		PopulateStoreCards();
		selectedCard = null;
	}

	public void PopulateStoreCards() {
		grid.transform.DestroyChildren();
		foreach(BaseGene b in storeManager.baseGenes) {
			GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("StoreCard"));
			go.transform.parent = grid.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			StoreCard storeCard = go.GetComponent<StoreCard>();
			storeCard.PopulateInfoFromGene(b);

			if((b.activationRequirements.Count > 0 && itemManager.seenItems.Contains(b.activationRequirements[0].itemId)))
				storeCard.SetCardActive(true);
		}
		grid.Reposition();
	}


	public void UpdateGold() {
		goldLabel.text = GameManager2.gameManager.gameVars.gold.ToString() + "[gold]";
	}


	public void AttemptPurchase(StoreCard card) {
		selectedCard = card;
		int cost = card.gene.sellValue * 10;
		if(gameManager.gameVars.gold < cost)
			hudManager.ShowError("Not enough gold");
		else
			hudManager.popup.Show("Purchase", "Purchase [EEBE63]" + card.headerLabel.text + "[-] for " + card.costLabel.text +"?", this, "PurchaseConfirmed");
	}


	public void PurchaseConfirmed() {
		if(selectedCard == null)
			return;
		int cost = selectedCard.gene.sellValue * 10;
		gameManager.AddGold(-cost);
		geneManager.storedGenes.Add(new Gene(selectedCard.gene));
		hudManager.ShowNotice("Purchase Complete");
		UpdateGold();
		selectedCard = null;
	}
}
