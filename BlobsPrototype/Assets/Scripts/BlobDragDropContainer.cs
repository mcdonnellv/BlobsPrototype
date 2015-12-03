using UnityEngine;
using System.Collections;

public class BlobDragDropContainer : UIDragDropContainer {

	public bool hasBlob {
		get {
			Blob blob = gameObject.GetComponentInChildren<Blob>();
			return (blob != null);
		}
	}

	public void BlobAdded(Blob blob) {
		blob.gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
		transform.SendMessageUpwards("BlobAddedToContainer", this);
	}
}
