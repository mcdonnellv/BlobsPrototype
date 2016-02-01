using UnityEngine;
using System;
using System.Collections;

public class Condition : Behavior {

	public Func<bool> CanRun { protected get; set; }

	public Condition() {
		Update = () => {
			if(CanRun != null && CanRun())
				return Status.BhSuccess;
			return Status.BhFailure;
		};
	}
}


