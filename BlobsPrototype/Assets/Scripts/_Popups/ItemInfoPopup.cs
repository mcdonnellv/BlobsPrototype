using UnityEngine;
using System.Collections;

public class ItemInfoPopup : MonoBehaviour {

	public UITweener animationWindow;
	public UIButton deleteButton;
	public UILabel nameLabel;
	public UILabel rarityLabel;
	public UILabel infoLabel1;
	public UILabel infoLabel2;

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



	public void ShowInfoForGene(Gene gene) {
		nameLabel.text = gene.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + "[-]";
		infoLabel1.text = gene.description;
		infoLabel2.text = "";
		infoLabel2.transform.DestroyChildren();
		
		foreach(Stat s in gene.stats) {
			int index = gene.stats.IndexOf(s);
			GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Stat Container"));
			statGameObject.transform.SetParent(infoLabel2.transform);
			statGameObject.transform.localScale = new Vector3(1f,1f,1f);
			statGameObject.transform.localPosition = new Vector3(0f, -14f + index * -26f, 0f);
			UISprite sprite = statGameObject.GetComponentInChildren<UISprite>();
			sprite.alpha = 0f;
			UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
			labels[0].text = (s.modifier == Stat.Modifier.Added) ? ("+" + s.amount.ToString()) : ("+" + s.amount.ToString() + "%");
			labels[0].depth = infoLabel2.depth + 2;
			labels[1].text = "[fist]" + s.id.ToString();
			labels[1].depth = infoLabel2.depth + 2;
		}
	}
}
