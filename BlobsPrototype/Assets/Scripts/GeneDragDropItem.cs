using UnityEngine;
using System.Collections;

public class GeneDragDropItem : UIDragDropItem {

	protected override void OnDragDropRelease (GameObject surface) {
		base.OnDragDropRelease((surface.GetComponent<GeneDragDropContainer>() == null) ? null : surface);
	}
}
