using UnityEngine;
using System;
using System.Collections;

public class Behavior : IBehavior {
	public Action Initialize { protected get; set; }
	public Action<Status> Terminate {protected get; set; }
	public Func<Status> Update {protected get; set; }
	public Status Status { get; set; }
	public Composite Parent { get; set; }

	public Status Tick() {
		if(Status == Status.BhInvalid && Initialize != null) {
			Initialize();
		}
		
		Status = Update();
		
		if(Status != Status.BhRunning && Initialize != null)
			Terminate(Status);
		
		return Status;
	}


	public Composite End() { return Parent; }

	public IBehavior SetUpdate(Func<Status> update) {
		Update = update;
		return this;
	}

	public IBehavior SetInitialize(Action initialize) {
		Initialize = initialize;
		return this;
	}

	public IBehavior SetTerminate(Action<Status> terminate) {
		Terminate = terminate;
		return this;
	}



	public virtual void Reset() {}
}
