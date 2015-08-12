using UnityEngine;
using System.Collections;

public class BlobPopup : Popup
{
	public UISprite bodySprite;
	public UISprite faceSprite;
	public UISprite cheeksSprite;
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

		bodySprite.color = blob.color;
		cheeksSprite.gameObject.SetActive(!blob.male);

		button1.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y);
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