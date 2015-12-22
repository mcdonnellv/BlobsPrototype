using UnityEngine;
using System.Collections;

public class ZoneMapItem : MonoBehaviour {
	public UILabel zoneLabel;
	public int zoneId;

	void Start() {
		Zone zone = ZoneManager.zoneManager.GetZoneByID(zoneId);
		if(zone != null)
			zoneLabel.text = zone.itemName;
		else {
			zoneLabel.text = "No Zone ID";
			zoneLabel.alpha = .5f;
			gameObject.GetComponent<UIButton>().isEnabled = false;
		}

	}
}
