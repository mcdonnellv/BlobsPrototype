using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class QuestMonster {
	public int id;
	public int level;
	public QuestMonster(int idParam, int levelParam) {id = idParam; level = levelParam; }
}