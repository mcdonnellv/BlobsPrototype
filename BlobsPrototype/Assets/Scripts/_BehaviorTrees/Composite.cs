using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Composite : Behavior {
	protected List<IBehavior> Children { get; set; }

	protected Composite() {
		Children = new List<IBehavior>();
		Initialize = () => { };
		Terminate = status => { };
		Update = () => Status.BhRunning;
	}
	
	public void Add(Composite composite) { 
		Children.Add(composite); 
	}

	public T Add<T>() where T : Behavior, new() {
		var t = new T { Parent = this };
		Children.Add(t);
		return t;
	}

	public IBehavior GetChild(int index) { 
		return Children[index]; 
	}

	public int ChildCount { 
		get { return Children.Count;} 
	}

	public override void Reset () {
		Status = Status.BhInvalid;
		foreach (var behavior in Children)
			behavior.Reset();
	}

}

