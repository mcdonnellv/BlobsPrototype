using UnityEngine;
using System.Collections;

public class StoreCard : MonoBehaviour {
	public UILabel headerLabel;
	public UILabel rarityLabel;
	public UILabel upperPanelInfoLabel;
	public UILabel costLabel;
	public UISprite icon;
	public UIWidget upperPanel;
	public UIWidget lowerPanel;
	public UIWidget unavailableCover;
	public UIGrid lowerPanelGrid;
	[HideInInspector] public bool cardActive = false;
	[HideInInspector] public BaseGene gene = null;


	public void PopulateInfoFromGene(BaseGene g) {
		gene = g;
		headerLabel.text = gene.itemName.ToUpper();
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		icon.spriteName = g.iconName;
		icon.atlas = g.iconAtlas;
		upperPanelInfoLabel.text = gene.description;
		costLabel.text = (g.sellValue * 10f).ToString() + "[gold]";
		lowerPanelGrid.transform.DestroyChildren();
		foreach(GeneActivationRequirement req in gene.activationRequirements) {
			int index = gene.activationRequirements.IndexOf(req);
			GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Requirement Container Store"));
			statGameObject.transform.SetParent(lowerPanelGrid.transform);
			statGameObject.transform.localScale = Vector3.one;
			statGameObject.transform.localPosition = Vector3.zero;
			UISprite[] sprites = statGameObject.GetComponentsInChildren<UISprite>();
			BaseItem item = ItemManager.itemManager.GetBaseItemByID(req.itemId);
			sprites[0].atlas = item.iconAtlas;
			sprites[0].spriteName = item.iconName;
			sprites[0].color = ColorDefines.IconColorFromIndex(item.iconTintIndex);
			UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
			labels[0].text = req.amountNeeded.ToString() + " x " + item.itemName;

			if(ItemManager.itemManager.DoesItemExistInStorage(item.id))
				labels[0].color = Color.white;
			else
				labels[0].color = ColorDefines.inactiveTextColor;
		}
		lowerPanelGrid.Invoke("Reposition", .1f);
	}

	public void Pressed() {
		HudManager.hudManager.storeMenu.AttemptPurchase(this);
	}

	public void SetCardActive(bool active) {
		cardActive = active;
		unavailableCover.gameObject.SetActive(!active);
	}
}
