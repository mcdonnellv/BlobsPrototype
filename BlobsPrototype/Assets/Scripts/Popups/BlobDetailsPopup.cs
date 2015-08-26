using UnityEngine;
using System.Collections;

public class BlobDetailsPopup : BlobPopupChoice
{
	public UILabel genderLabel;
	public UILabel qualityLabel;
	public UILabel levelLabel;


	void Start () 
	{
	}

	public void ShowChoice(Blob blob, string header, string body, 
	                       MonoBehaviour okTarget, string okMethodName, 
	                       MonoBehaviour cancelTarget, string cancelMethodName)
	{
		
		Show(blob, header, body);

		genderLabel.text = (blob.male) ? "Gender: Male" : "Gender: Female";
		qualityLabel.text = "Quality: " + blob.quality.ToString();
		levelLabel.text = "Level: " + blob.level.ToString();

		int pixels = (int)(.4f * 50f);
		bodySprite.SetDimensions(pixels, pixels);
		faceSprite.SetDimensions(pixels, pixels);
		cheeksSprite.SetDimensions(pixels, pixels);

		button1.onClick.Add(new EventDelegate(okTarget, okMethodName));
		button2.onClick.Clear();
		button2.onClick.Add(new EventDelegate(this, "Hide"));
		
		if(cancelTarget != null && cancelMethodName != null)
			button2.onClick.Add(new EventDelegate(cancelTarget, cancelMethodName));
	}
}
