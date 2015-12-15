using UnityEngine;
using System.Collections;

public class QuestCell : MonoBehaviour {
	public UILabel titleLabel;
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public UIGrid blobSlotGrid;
	public UISprite foreGround;
	public UILabel newLabel;

	public void SetupBlobCells(int count) {
		foreach(Transform child in blobSlotGrid.transform) {
			if(child.GetSiblingIndex() < count)
				child.GetComponent<UISprite>().alpha = 1f;
			else
				child.GetComponent<UISprite>().alpha = .2f;
		}
	}

	public void Pressed() {
		QuestListMenu questListMenu = gameObject.GetComponentInParent<QuestListMenu>();
		questListMenu.QuestCellPressed(this);
		Undim();
		foreach(Transform child in transform.parent) {
			if(child == transform)
				continue;
			child.SendMessage("Dim");
		}
	}

	public void Dim() {
		foreGround.alpha = .25f;
	}

	public void Undim() {
		foreGround.alpha = 0f;
	}
		                               

	public void DisplayBlobImage(Blob blob, int index) {
		Transform slot = blobSlotGrid.transform.GetChild(index);
		GameObject blobGameObject = blob.CreateDuplicateForUi(slot, false);
		blobGameObject.transform.localPosition = new Vector3(0f, -4f, 0f);
		blobGameObject.transform.localScale = blob.transform.localScale / 4f;
	}
}
