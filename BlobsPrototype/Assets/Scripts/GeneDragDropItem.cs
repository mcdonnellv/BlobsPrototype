using UnityEngine;
using System.Collections;

public class GeneDragDropItem : UIDragDropItem {
	GeneManager geneManager  { get { return GeneManager.geneManager; } }

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
		
		Blob blob = HudManager.hudManager.blobInfoContextMenu.DisplayedBlob();

		if (gddcFrom.type == GeneDragDropContainer.GeneSlotType.BlobInfoContextMenuGeneSlot && 
		    gddcTo.type == GeneDragDropContainer.GeneSlotType.GenePoolMenuGeneSlot)
			geneManager.GeneTransferFromBlobToGenePool(blob, gp.gene);

		if (gddcTo.type == GeneDragDropContainer.GeneSlotType.BlobInfoContextMenuGeneSlot && 
		    gddcFrom.type == GeneDragDropContainer.GeneSlotType.GenePoolMenuGeneSlot)
			geneManager.GeneTransferFromGenePoolToBlob(blob, gp.gene);
		
	}
}