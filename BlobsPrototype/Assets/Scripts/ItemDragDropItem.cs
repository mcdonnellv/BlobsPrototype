using UnityEngine;
using System.Collections;

public class ItemDragDropItem : UIDragDropItem {

	protected override void OnDrag (Vector2 delta) {
		gameObject.GetComponent<ItemPointer>().ItemPressed();
		base.OnDrag(delta);
	}
	
	
	protected override void OnDragDropRelease (GameObject surface) {
		base.OnDragDropRelease(null);
	}
}
