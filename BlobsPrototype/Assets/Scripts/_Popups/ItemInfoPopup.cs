using UnityEngine;
using System.Collections;

public class ItemInfoPopup : MonoBehaviour {

	public UITweener animationWindow;
	public UIButton deleteButton;
	public UILabel nameLabel;
	public UILabel rarityLabel;
	public UILabel infoLabel1;
	public UISprite icon;
	public GameObject singlePanel;
	public GameObject doublePanel;
	public GameObject singlePanelLabel;
	public UIGrid doublePanelUpperGrid;
	public UIGrid doublePanelLowerGrid;

	public void Show() {
		gameObject.SetActive(true);
		transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
		deleteButton.gameObject.SetActive(false);
	}
	
	public void Hide() {
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();
	}

	void DisableWindow() {
		animationWindow.onFinished.Clear();
		gameObject.SetActive(false);
	}

	public void DeleteButtonPressed() {}


	public void ShowInfoForGene(Gene gene) {
		doublePanel.gameObject.SetActive(true);
		singlePanel.gameObject.SetActive(false);

		nameLabel.text = gene.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		infoLabel1.text = gene.description;
		doublePanelUpperGrid.transform.DestroyChildren();
		doublePanelLowerGrid.transform.DestroyChildren();

		//UISprite originalIcon = gene.CreateGeneGameObject .genePointer.gameObject.GetComponent<UISprite>();
		//icon.atlas = null;//originalIcon.atlas;
		icon.spriteName = Gene.GetSpriteNameWithQuality(gene.quality);
		
		foreach(Stat s in gene.stats) {
			int index = gene.stats.IndexOf(s);
			GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Info Panel Stat Container"));
			statGameObject.transform.SetParent(doublePanelUpperGrid.transform);
			statGameObject.transform.localScale = new Vector3(1f,1f,1f);
			statGameObject.transform.localPosition = new Vector3(0f, -14f + index * -26f, 0f);
			UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
			labels[0].text = (s.modifier == Stat.Modifier.Added) ? ("+" + s.amount.ToString()) : ("+" + s.amount.ToString() + "%");
			labels[1].text = s.id.ToString();
		}

		foreach(GeneReq gr in gene.activationReq) {
			int index = gene.activationReq.IndexOf(gr);
			GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Requirement Container"));
			statGameObject.transform.SetParent(doublePanelLowerGrid.transform);
			statGameObject.transform.localScale = new Vector3(1f,1f,1f);
			statGameObject.transform.localPosition = new Vector3(0f, -14f + index * -26f, 0f);
			UISprite[] sprites = statGameObject.GetComponentsInChildren<UISprite>();
			sprites[0].atlas = gr.item.iconAtlas;
			sprites[0].spriteName = gr.item.iconName;
			UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
			labels[0].text = "Feed " + gr.amount.ToString() + " " + gr.item.itemName;
			labels[1].text = gr.fulfilledAmount.ToString() + " / " + gr.amount.ToString();
		}

		doublePanelUpperGrid.Reposition();
		doublePanelLowerGrid.Reposition();
	}

	public void ShowInfoForItem(Item item) {
		doublePanel.gameObject.SetActive(false);
		singlePanel.gameObject.SetActive(true);
		
		nameLabel.text = item.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + " Item[-]";
		infoLabel1.text = item.description;
		icon.atlas = item.iconAtlas;
		icon.spriteName = item.iconName;
	}
}
