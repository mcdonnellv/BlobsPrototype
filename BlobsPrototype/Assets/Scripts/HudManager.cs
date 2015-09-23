﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HudManager : MonoBehaviour {

	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel chocolateLabel;
	public BlobInfoContextMenu blobInfoContextMenu;
	public RoomOptionsContextMenu roomOptionsContextMenu;
	public CheatMenu cheatMenu;
	public BuildRoomMenu buildRoomMenu;
	public Popup popup;
	
	public void UpdateGold(int gold) {goldLabel.text = gold.ToString() + "[gold]";}
	public void UpdateChocolate(int chocolate) {chocolateLabel.text = chocolate.ToString() + "[chocolate]";}
	public void UpdateAverageQuality(string qualityStr) {averageQualityLabel.text = "Average Quality: " + qualityStr;}

	// Use this for initialization
	void Start () {
		cheatMenu.Hide();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

