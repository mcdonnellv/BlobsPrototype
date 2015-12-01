using UnityEngine;
using System.Collections;

public class QuestCell : MonoBehaviour {
	public UILabel titleLabel;
	public UISprite icon;
	public UILabel durationLabel;
	public UILabel rarityLabel;
	public UIGrid blobSlotGrid;

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
	}
}
