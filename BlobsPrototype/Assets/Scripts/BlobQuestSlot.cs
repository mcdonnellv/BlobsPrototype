using UnityEngine;
using System.Collections;

public class BlobQuestSlot : MonoBehaviour {
	public UISprite fulfilledSprite;
	public UISprite sigilSprite;
	public GameObject socket;
	public UISprite colorBgSprite;
	public UISprite socketSprite;

	void Start() {
		sigilSprite.gameObject.SetActive(false);
		fulfilledSprite.gameObject.SetActive(false);
		colorBgSprite.color = ColorDefines.defaultBlobSocketColor;
		socketSprite.alpha = .5f;
	}
}
