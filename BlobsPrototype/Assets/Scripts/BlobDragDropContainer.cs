using UnityEngine;
using System.Collections;

public class BlobContainerPackage {
	public Blob blob;
	public BlobDragDropContainer container;
}

public class BlobDragDropContainer : UIDragDropContainer {

	public bool uiContainer = true;
	public bool hasBlob {
		get { return (gameObject.GetComponentInChildren<BlobGameObject>() != null);}
	}

	public void BlobAdded(Blob blob) {
		blob.gameObject.transform.localScale = new Vector3(.6f, .6f, .6f);
		blob.gameObject.transform.localPosition = new Vector3(.0f, -15f, 1f);
		BlobContainerPackage package = new BlobContainerPackage();
		package.blob = blob;
		package.container = this;

		transform.SendMessageUpwards("BlobAddedToContainer", package);
	}
}
