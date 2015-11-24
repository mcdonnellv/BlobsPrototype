using UnityEngine;
using System.Collections;

public class GeneDragDropContainer : InventoryContainer {
	public enum GeneSlotType {
		BlobInfoContextMenuGeneSlot,
		GenePoolMenuGeneSlot
	};

	public GeneSlotType type;

}
