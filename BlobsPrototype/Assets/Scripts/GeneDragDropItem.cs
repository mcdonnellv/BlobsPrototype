using UnityEngine;
using System.Collections;

public class GeneDragDropItem : UIDragDropItem {

	protected override void OnDrag (Vector2 delta) {
		gameObject.GetComponent<GenePointer>().GenePressed();
		base.OnDrag(delta);
	}


	protected override void OnDragDropRelease (GameObject surface) {
		GenePointer gp = gameObject.GetComponent<GenePointer>();
		GeneDragDropContainer gddcFrom = gp.transform.parent.GetComponent<GeneDragDropContainer>();
		GeneDragDropContainer gddcTo = surface.GetComponent<GeneDragDropContainer>();
		base.OnDragDropRelease((gddcTo == null) ? null : surface);

		if (gddcTo == null || gddcTo == gddcFrom)
			return;

		if (gddcTo.type == gddcFrom.type)
			return;

		HudManager hudManager = GameObject.Find ("HudManager").GetComponent<HudManager>();
		Blob blob = hudManager.blobInfoContextMenu.DisplayedBlob();
		GeneManager geneManager = GameObject.Find ("GeneManager").GetComponent<GeneManager>();

		if (gddcFrom.type == GeneDragDropContainer.GeneSlotType.BlobInfoContextMenuGeneSlot && 
		    gddcTo.type == GeneDragDropContainer.GeneSlotType.GenePoolMenuGeneSlot)
			geneManager.GeneTransferFromBlobToGenePool(blob, gp.gene);

		if (gddcTo.type == GeneDragDropContainer.GeneSlotType.BlobInfoContextMenuGeneSlot && 
		    gddcFrom.type == GeneDragDropContainer.GeneSlotType.GenePoolMenuGeneSlot)
			geneManager.GeneTransferFromGenePoolToBlob(blob, gp.gene);
		
	}
}