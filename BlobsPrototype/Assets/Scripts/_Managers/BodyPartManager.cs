using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyPartManager : MonoBehaviour {
	private static BodyPartManager _bodyPartManager;
	public static BodyPartManager bodyPartManager { get {if(_bodyPartManager == null) _bodyPartManager = GameObject.Find("BodyPartManager").GetComponent<BodyPartManager>(); return _bodyPartManager; } }

	public List<Texture> bodyTextures = new List<Texture>();
	public List<Texture> eyeTextures = new List<Texture>();
}
