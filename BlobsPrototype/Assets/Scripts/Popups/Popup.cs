using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour 
{
	public UILabel headerLabel;
	public UILabel bodyLabel;
	public UILabel button1Label;
	public UIButton button1;
	
	public void Show(string header, string body)
	{
		Show(header);
		bodyLabel.text = body;
	}

	public void Show(string header)
	{
		headerLabel.text = header;
		button1.onClick.Clear();
		button1.onClick.Add(new EventDelegate(this, "Hide"));
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}


}

