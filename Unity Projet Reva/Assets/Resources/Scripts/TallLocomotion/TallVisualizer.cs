using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallVisualizer : ArcVisualizer {
	[Tooltip("Bezier control point height when ArcRaycaster is at MIN distance")]
	public float shortControlHeight = 1.5f;
	[Tooltip("Bezier control point height when ArcRaycaster is at MAX distance")]
	public float longControlHeight = 2.0f;
	protected TallRaycaster raycaster;
    private GameObject m_TeleportCursor;
    void Awake() {
		if (arcRenderer == null) {
			arcRenderer = GetComponent<LineRenderer> ();
		}
		if (arcRaycaster == null) {
			arcRaycaster = GetComponent<TallRaycaster> ();
		}
		else if (!(arcRaycaster is TallRaycaster)) {
			Debug.LogError ("ArcVisualizer's Arc Ray is not TallRaycaster");
		}
		if (arcRenderer == null) {
			Debug.LogError ("ArcVisualizer's Line Renderer is not set");
		}
		if (arcRaycaster == null) {
			Debug.LogError ("ArcVisualizer's Arc Ray Caster is not set");
		}
		if (segments < 3) {
			segments = 3;
			Debug.LogWarning ("Should have at least 3 segments!");
		}

		arcRenderer.positionCount = segments;
		if (arcRaycaster != null) {
			raycaster = arcRaycaster as TallRaycaster;
		}
        GameObject prefabTeleportCursor = Resources.Load("Prefabs/magic_ring_01") as GameObject;
        m_TeleportCursor = Instantiate(prefabTeleportCursor, arcRaycaster.HitPoint, Quaternion.Euler(-90, 0, 0));

    }

    void Update() {
		#if !UNITY_EDITOR
		if (EarlyOut()) {
			return;
		}
		#endif

		float horizontalDistance = raycaster.HorizontalDistance;
		float maxDistance = raycaster.MaxDistnce;

		float recip = 1.0f / (float)(segments - 1);
		for (int i = 0; i < segments; ++i) {
			float t = (float)i * recip;
			Vector3 sample = SampleCurve(arcRaycaster.Start, arcRaycaster.HitPoint, Mathf.Clamp01(t), horizontalDistance, maxDistance);
			arcRenderer.SetPosition (i, sample);
		}
        m_TeleportCursor.transform.position = arcRaycaster.HitPoint;
        SetCurveVisuals();

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
        {
            TeleportPlayer();
        }
    }

	Vector3 SampleCurve(Vector3 start, Vector3 end, float time, float horizontalDistance, float maxDistance) {
		Vector3 middle = Vector3.Lerp (start, end, 0.5f);
		float height = shortControlHeight + (longControlHeight - shortControlHeight) * (horizontalDistance / maxDistance);
		middle += arcRaycaster.Up *  Mathf.Clamp (height, shortControlHeight, longControlHeight);

		return Vector3.Lerp(Vector3.Lerp(start, middle, time), Vector3.Lerp(middle, end, time), time);
	}

    private void TeleportPlayer()
    {
        GameObject playerObject = GameObject.Find("OVRPlayerController");
        Vector3 newPlayerPosition = new Vector3(this.m_TeleportCursor.transform.position.x, 0.97f, this.m_TeleportCursor.transform.position.z);
        playerObject.transform.position = newPlayerPosition;
    }
}
