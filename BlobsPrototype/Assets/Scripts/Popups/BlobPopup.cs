using UnityEngine;
using System.Collections;

public class BlobPopup : Popup
{
	public UISprite bodySprite;
	public UISprite faceSprite;
	public UISprite cheeksSprite;
	public UISprite bg;
	public UISprite qualityIndicator;

	public Vector3 buttonPos = new Vector3(0, -85f);

	public void ShowBlob(bool show)
	{
		bodySprite.gameObject.SetActive(show);
		faceSprite.gameObject.SetActive(show);
		cheeksSprite.gameObject.SetActive(show);
	}
	
	public void Hide()
	{
		base.Hide();
		ShowBlob(false);
	}
	
	public void Show(Blob blob, string header, string body)
	{
		Show(header, body);
		ShowBlob(true);
		Texture tex = blob.bodyPartSprites["Body"];
		bodySprite.spriteName = tex.name;
		tex = blob.bodyPartSprites["Eyes"];
		faceSprite.spriteName = tex.name;
		float c = 1.0f;
		bg.color = (blob.male) ? new Color(0.62f*c, 0.714f*c, 0.941f*c,1f) : new Color(0.933f*c, 0.604f*c, 0.604f*c, 1f);
		qualityIndicator.color = Blob.ColorForQuality(blob.quality);
		bodySprite.color = blob.color;
		cheeksSprite.gameObject.SetActive(!blob.male);

		//button1.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y);
		button1.onClick.Clear();
		button1.onClick.Add(new EventDelegate(this, "Hide"));
	}
	
	// Use this for initialization
	void Start () 
	{
		button1Label.text = "OK";
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}