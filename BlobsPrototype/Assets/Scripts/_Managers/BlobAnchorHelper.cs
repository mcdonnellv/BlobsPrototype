using UnityEngine;

public enum BlobAnchorPosition {
	Near,
	Mid,
	Far,
};

public class BlobAnchorHelper : MonoBehaviour {
	public BattleAnchor nearAnchor;
	public BattleAnchor midAnchor;
	public BattleAnchor farAnchor;
	[HideInInspector] public Vector3 anchorTargetPos;
	public float marchDistance = 5f;
	public float minDistanceBetweenBlobs = .5f;
	public float farAnchorDistance = -2f;
	public float midAnchorDistance = 2f;
	public float nearAnchorDistance = 4f;

	void OnValidate() {
		UpdateSubanchorDistances();
	}

	public void Awake() {
		UpdateSubanchorDistances();
	}

	private void UpdateSubanchorDistances() {
		foreach(Transform t in transform) {
			BattleAnchor anchor = t.GetComponent<BattleAnchor>();
			if(anchor == null)
				continue;
			if(anchor == nearAnchor)
				anchor.transform.localPosition = new Vector3(nearAnchorDistance, 0f, 0f);
			if(anchor == midAnchor)
				anchor.transform.localPosition = new Vector3(midAnchorDistance, 0f, 0f);
			if(anchor == farAnchor)
				anchor.transform.localPosition = new Vector3(farAnchorDistance, 0f, 0f);
			foreach(Transform t1 in anchor.transform)
				t1.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - ((anchor.transform.childCount - 1) * .5f * minDistanceBetweenBlobs), 0f, 0f);
		}
	}

	public void Reset() {
		SetBlobAnchorPosition(new Vector3(0.01f , 0f, 0f), true);
		nearAnchor.transform.DestroyChildren();
		midAnchor.transform.DestroyChildren();
		farAnchor.transform.DestroyChildren();
	}

	public void AdvanceBlobAnchorPosition() {
		TranslateBlobAnchorPosition(new Vector3(marchDistance,0,0));
	}

	public void RevertBlobAnchorPosition() {
		TranslateBlobAnchorPosition(new Vector3(-marchDistance,0,0));
	}

	public void TranslateBlobAnchorPosition(Vector3 offset) {
		SetBlobAnchorPosition(anchorTargetPos + offset, false);
	}

	public void SetBlobAnchorPosition(Vector3 pos, bool immediate) {
		pos.x = Mathf.Max(0f, pos.x);
		if(immediate) 
			transform.position = pos;
		else 
			anchorTargetPos = pos;
	}

	public void SetBlobActorPosition(Actor actor, BlobAnchorPosition pos) {
		if(pos == BlobAnchorPosition.Near)
			PositionToAnchor(actor, nearAnchor);
		if(pos == BlobAnchorPosition.Mid)
			PositionToAnchor(actor, midAnchor);
		if(pos == BlobAnchorPosition.Far)
			PositionToAnchor(actor, farAnchor);
	}

	private void PositionToAnchor(Actor actor, BattleAnchor anchor) {
		GameObject go = new GameObject("Anchor");
		go.transform.parent = anchor.transform;
		foreach(Transform t in anchor.transform)
			t.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - ((anchor.transform.childCount - 1) * .5f * minDistanceBetweenBlobs), 0f, 0f);
		actor.SetBehaviorSharedVariable("Anchor", go);
	}

	public void Update() {
		transform.position = Vector3.MoveTowards(transform.position, anchorTargetPos, Time.deltaTime * 10f);
	}
}
