using UnityEngine;
using System.Collections;

public class GeneDragDropContainer : UIDragDropContainer {
	public enum GeneSlotType {
		BlobInfoContextMenuGeneSlot,
		GenePoolMenuGeneSlot
	};

	public GeneSlotType type;

}
