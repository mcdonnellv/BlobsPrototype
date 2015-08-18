using UnityEngine;
using System.Collections;

public class BlobPopupChoice : BlobPopup 
{
	public UILabel button2Label;
	public UIButton button2;

	public void ShowChoice(Blob blob, string header, string body, 
	                       MonoBehaviour okTarget, string okMethodName, 
	                       MonoBehaviour cancelTarget, string cancelMethodName)
	{

		Show(blob, header, body);

		button2.gameObject.SetActive(true);
		button2.transform.localPosition = new Vector3(buttonPos.x + 100f, buttonPos.y);
		button1.transform.localPosition = new Vector3(buttonPos.x - 100f, buttonPos.y);
		
		button1.onClick.Clear();
		button1.onClick.Add(new EventDelegate(this, "Hide"));
		button1.onClick.Add(new EventDelegate(okTarget, okMethodName));
		button1Label.text = "Ok";

		button2Label.text = "Cancel";
		button2.onClick.Clear();
		button2.onClick.Add(new EventDelegate(this, "Hide"));

		if(cancelTarget != null && cancelMethodName != null)
			button2.onClick.Add(new EventDelegate(cancelTarget, cancelMethodName));
	}
}
