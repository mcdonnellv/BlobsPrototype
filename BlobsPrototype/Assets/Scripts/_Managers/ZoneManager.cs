using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ZoneManager : MonoBehaviour {
	private static ZoneManager _zoneManager;
	public static ZoneManager zoneManager { get {if(_zoneManager == null) _zoneManager = GameObject.Find("ZoneManager").GetComponent<ZoneManager>(); return _zoneManager; } }

	public List<Zone> zones = new List<Zone>();

	public bool DoesNameExistInList(string nameParam){return (GetZoneWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetZoneByID(idParam) != null); }

	public int GetNextAvailableID() {
		int lowestIdVal = 0;
		List<Zone> sortedByID = zones.OrderBy(x => x.id).ToList();
		foreach(Zone i in sortedByID)
			if (i.id == lowestIdVal)
				lowestIdVal++;
		return lowestIdVal;
	}

	public Zone GetZoneByID(int idParam) {
		foreach(Zone i in zones)
			if (i.id == idParam)
				return i;
		return null;
	}

	public Zone GetZoneWithName(string nameParam) {
		foreach(Zone i in zones)
			if (i.itemName == nameParam)
				return i;
		return null;
	}

	public Zone GetZonewithQuestID(int questId) {
		foreach(Zone zone in zones)
			foreach(int qId in zone.questIds)
				if (qId == questId)
					return zone;
		return null;
	}

}
