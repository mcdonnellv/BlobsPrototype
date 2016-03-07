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
	[HideInInspector] public Vector3 anchorTargetPos;
	public float marchDistance = 5f;
	public float minDistanceBetweenBlobs = .5f;
	public float farAnchorDistance = -2f;
	public float midAnchorDistance = 2f;
	public float nearAnchorDistance = 4f;
	public bool drawDebug = false;

	void OnValidate() {
		UpdateSubanchorDistances();
	}

	public void Awake() {
		CreateSubAnchors();
		UpdateSubanchorDistances();
	}

	public void CreateSubAnchors() {
		nearAnchor = new GameObject("Near").transform;
		midAnchor = new GameObject("Mid").transform;
		farAnchor = new GameObject("Far").transform;
		nearAnchor.parent = transform;
		midAnchor.parent = transform;
		farAnchor.parent = transform;
	}

	private void UpdateSubanchorDistances() {
		if(nearAnchor != null && farAnchor != null) {
			nearAnchor.localPosition = new Vector3(nearAnchorDistance, 0f, 0f);
			midAnchor.localPosition = new Vector3(midAnchorDistance, 0f, 0f);
			farAnchor.localPosition = new Vector3(farAnchorDistance, 0f, 0f);
			foreach(Transform t in nearAnchor)
				t.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - ((nearAnchor.childCount - 1) * .5f * minDistanceBetweenBlobs), 0f, 0f);
			foreach(Transform t in midAnchor)
				t.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - ((midAnchor.childCount - 1) * .5f * minDistanceBetweenBlobs), 0f, 0f);
			foreach(Transform t in farAnchor)
				t.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - ((farAnchor.childCount - 1) * .5f * minDistanceBetweenBlobs), 0f, 0f);
		}
	}

	public void Reset() {
		SetBlobAnchorPosition(new Vector3(0.01f , 0f, 0f));
	}

	public void AdvanceBlobAnchorPosition() {
		TranslateBlobAnchorPosition(new Vector3(marchDistance,0,0));
	}

	public void RevertBlobAnchorPosition() {
		TranslateBlobAnchorPosition(new Vector3(-marchDistance,0,0));
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
			t.localPosition = new Vector3(minDistanceBetweenBlobs * t.GetSiblingIndex() - ((anchor.childCount - 1) * .5f * minDistanceBetweenBlobs), 0f, 0f);
		actor.SetBehaviorSharedVariable("Anchor", go);
	}

	public void Update() {
		transform.position = Vector3.MoveTowards(transform.position, anchorTargetPos, Time.deltaTime * 10f);
		if(drawDebug) {
			DrawDebugForAnchor(nearAnchor);
			DrawDebugForAnchor(midAnchor);
			DrawDebugForAnchor(farAnchor);
		}
	}

	private void DrawDebugForAnchor(Transform anchor) {
		Debug.DrawRay(anchor.position, new Vector3(-.2f,-.5f,0f));
		Debug.DrawRay(anchor.position, new Vector3(.2f,-.5f,0f));
	}
}
