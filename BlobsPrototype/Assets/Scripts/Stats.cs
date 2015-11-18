using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stats {
	public List<int> values;
	
	public void Reset(int val) { 
		if(values == null) {
			values = new List<int>((int)Stat.Identifier.StatIdCount);
		}
		
		values.Clear();
		for(int i = 0; i < (int)Stat.Identifier.StatIdCount; i++)
			values.Add(val);
	}
}