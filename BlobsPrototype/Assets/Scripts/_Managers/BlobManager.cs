using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlobManager : MonoBehaviour {

	private static BlobManager _blobManager;
	public static BlobManager blobManager { get {if(_blobManager == null) _blobManager = GameObject.Find("BlobManager").GetComponent<BlobManager>(); return _blobManager; } }


	public List<Blob> blobs = new List<Blob>();
}
