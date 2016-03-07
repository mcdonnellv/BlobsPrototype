using UnityEngine;

public enum BlobAnchorPosition {
	Near,
	Mid,
	Far,
};

public class BlobAnchorHelper : MonoBehaviour {
	private Transform nearAnchor;
	private Transform midAnchor;
	private Transform farAnchor;
	[HideInInspector]public Vector3 anchorTargetPos;
	public float moveDistance = 5f;
	public float minDistanceBetweenBlobs = .5f;

	public void Awake() {
		nearAnchor = transform.FindChild("Near");
		midAnchor = transform.FindChild("Mid");
		farAnchor = transform.FindChild("Far");
	}

	public void Reset() {
		SetBlobAnchorPosition(new Vector3(0.01f , -.7f, -20f));
	}

	public void AdvanceBlobAnchorPosition() {
		TranslateBlobAnchorPosition(new Vector3(moveDistance,0,0));
	}

	public void RevertBlobAnchorPosition() {
		TranslateBlobAnchorPosition(new Vector3(-moveDistance,0,0));
	}

	public void TranslateBlobAnchorPosition(Vector3 offset) {
		SetBlobAnchorPosition(anchorTargetPos + offset);
	}

	public void SetBlobAnchorPosition(Vector3 pos) {
		pos.x = Mathf.Max(0f, pos.x);
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

	private void PositionToAnchor(Actor actor, Transform anchor) {
		GameObject go = new GameObject("Anchor");
		go.transform.parent = anchor;
		foreach(Transform t in anchor)
			t.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - (anchor.childCount * .5f * minDistanceBetweenBlobs), 0f, 0f);
		actor.SetBehaviorSharedVariable("Anchor", go);
	}

	public void Update() {
		transform.position = Vector3.MoveTowards(transform.position, anchorTargetPos, Time.deltaTime * 10f);
	}
}
