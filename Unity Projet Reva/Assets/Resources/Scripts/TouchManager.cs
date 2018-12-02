using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour {

	public GameObject m_TeleportCursor = null;

	// Use this for initialization
	void Start () {
		
	}
	
	
	void Update () {

		/**
			Pression du joystick gauche
			=> on fait apparaître le curseur de déplacement
		 */
		if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)) {
			Debug.Log(GameObject.Find("LeftEyeAnchor"));
			SpawnTeleportCursor();
		}

		/**
			Appui du joystick gauche (en continu)
			=> on déplace le curseur on fonction de l'état de l'axe 2D du joystick
		 */
		if(this.m_TeleportCursor != null) {
			if(OVRInput.Get(OVRInput.Button.PrimaryThumbstick)) {
				Vector2 axis2DLeftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
				if(axis2DLeftThumbstick != null) {
					MoveTeleportCursor(axis2DLeftThumbstick);
				}
			}
		}


		/**
			Le joystick gauche n'est plus appuyé :
			=> on fait disparaître le curseur
		 */
		if(OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick)) {
			TeleportPlayer();
			Destroy(m_TeleportCursor);
			this.m_TeleportCursor = null;
		}
	}

	private void SpawnTeleportCursor() {

		GameObject playerEyeObject = GameObject.Find("LeftEyeAnchor");
		GameObject playerObject = GameObject.Find("OVRPlayerController");
		Vector3 teleportCursorPosition = playerEyeObject.transform.position + playerEyeObject.transform.forward * 4;
		GameObject prefabTeleportCursor = Resources.Load("Prefabs/TeleportCursor") as GameObject;
		this.m_TeleportCursor = Instantiate(prefabTeleportCursor, new Vector3(teleportCursorPosition.x, -2.34f, teleportCursorPosition.z), Quaternion.identity);
		this.m_TeleportCursor.transform.parent = playerObject.transform;

	}

	/**
		Vector2 axis2DValues : x => valeur horizontale du joystick
							   y => valeur verticale du joystick

		Donc on utilise le y comme z, comme le y du l'espace Unity est invariant (pour l'instant)
		

		BUG QUE FIFI DOIT RESOUDRE : Les offsets de déplacement se font selon le World.Space, il faut que je change ça par rapport à la cam' 
	 */
	private void MoveTeleportCursor(Vector2 axis2DValues) {
		Vector3 newCursorPosition = this.m_TeleportCursor.transform.localPosition + new Vector3(axis2DValues.x, 0.0f, axis2DValues.y);
		this.m_TeleportCursor.transform.localPosition = newCursorPosition;

	}

	/**
		Le joueur est téléporté à l'endroit du curseur
	 */
	private void TeleportPlayer() {
		GameObject playerObject = GameObject.Find("OVRPlayerController");
		Vector3 newPlayerPosition = new Vector3(this.m_TeleportCursor.transform.position.x, 0.97f, this.m_TeleportCursor.transform.position.z);
		playerObject.transform.position = newPlayerPosition;
	}


}
