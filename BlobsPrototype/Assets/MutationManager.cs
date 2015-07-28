using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MutationManager: MonoBehaviour
{
	public List<Mutation> mutations = new List<Mutation>();

	public Mutation GetMutationByName(string name)
	{
		foreach(Mutation m in mutations)
			if (m.name == name)
				return m;
		return null;
	}

	public bool DoesNameExistInList(string name)
	{
		foreach(Mutation m in mutations)
			if (m.name == name)
				return true;
		return false;
	}
}
