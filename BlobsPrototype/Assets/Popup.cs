﻿using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour 
{
	public UILabel headerLabel;
	public UILabel bodyLabel;
	public UILabel button1Label;
	public UILabel button2Label;
	public UIButton button1;
	public UIButton button2;

	Vector3 buttonPos = new Vector3(0, -85f);

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Show(string header, string body)
	{
		button2.gameObject.SetActive(false);
		button1.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y);
		headerLabel.text = header;
		bodyLabel.text = body;
		gameObject.SetActive(true);
	}

	public void ShowChoice(string header, string body)
	{
		button2.gameObject.SetActive(true);
		button2.transform.localPosition = new Vector3(buttonPos.x + 100f, buttonPos.y);
		button1.transform.localPosition = new Vector3(buttonPos.x - 100f, buttonPos.y);

		headerLabel.text = header;
		bodyLabel.text = body;
		gameObject.SetActive(true);
	}

	public void Button1Pressed()
	{
		if (button2.gameObject.activeSelf ==  false)
			Hide();
	}

	public void Button2Pressed()
	{
		Hide();
	}

	// Use this for initialization
	void Start () 
	{
		button1Label.text = "OK";
		button2Label.text = "Cancel";
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
