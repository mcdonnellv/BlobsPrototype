using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MutationManager: MonoBehaviour
{
	public List<Mutation> mutations = new List<Mutation>();

	public Mutation GetMutationByName(string nameParam)
	{
		foreach(Mutation m in mutations)
			if (m.mutationName == nameParam)
				return m;
		return null;
	}

	public bool DoesNameExistInList(string nameParam)
	{
		foreach(Mutation m in mutations)
			if (m.mutationName == nameParam)
				return true;
		return false;
	}
}
