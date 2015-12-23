using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class MonsterManager : MonoBehaviour {
	private static MonsterManager _monsterManager;
	public static MonsterManager monsterManager { get {if(_monsterManager == null) _monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>(); return _monsterManager; } }
	public UIAtlas iconAtlas;
	public List<BaseMonster> monsters = new List<BaseMonster>();

	public bool DoesNameExistInList(string nameParam){return (GetBaseMonsterWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseMonsterByID(idParam) != null); }
	
	public int GetNextAvailableID() {
		int lowestIdVal = 0;
		List<BaseMonster> sortedByID = monsters.OrderBy(x => x.id).ToList();
		foreach(BaseMonster i in sortedByID)
			if (i.id == lowestIdVal)
				lowestIdVal++;
		return lowestIdVal;
	}
	
	public BaseMonster GetBaseMonsterByID(int idParam) {
		foreach(BaseMonster i in monsters)
			if (i.id == idParam)
				return i;
		return null;
	}
	
	public BaseMonster GetBaseMonsterWithName(string nameParam) {
		foreach(BaseMonster i in monsters)
			if (i.itemName == nameParam)
				return i;
		return null;
	}
}
