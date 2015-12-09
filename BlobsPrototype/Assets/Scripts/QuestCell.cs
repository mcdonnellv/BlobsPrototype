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

	public void DisplayBlobImage(Blob blob, int index) {
		Transform slot = blobSlotGrid.transform.GetChild(index);
		GameObject blobGameObject = (GameObject)GameObject.Instantiate(blob.gameObject);
		blobGameObject.transform.SetParent(slot);
		blobGameObject.transform.localPosition = new Vector3(0f, -4f, 0f);
		blobGameObject.transform.localScale = blob.transform.localScale / 4f;
		Destroy(blobGameObject.transform.Find("FloatingDisplay").gameObject);
		Destroy(blobGameObject.GetComponent("Blob"));
		Destroy(blobGameObject.GetComponent("BoxCollider"));
		Destroy(blobGameObject.GetComponent("BlobDragDropItem"));
		Destroy(blobGameObject.GetComponent("UIButton"));
	}
}
