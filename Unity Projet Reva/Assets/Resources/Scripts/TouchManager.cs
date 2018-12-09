using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour {

	public GameObject m_TeleportCursor = null;
    // Use this for initialization
	void Start () {
    }


    void Update() {
        /** 
            Utilisation de A et B pour monter ou descendre 
         */
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            GameObject playerObject = GameObject.Find("OVRPlayerController");
            playerObject.transform.Translate(0f, 0.3f, 0f);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            GameObject playerObject = GameObject.Find("OVRPlayerController");
            playerObject.transform.Translate(0f, -0.3f, 0f);
        }
        /**
			Pression du joystick gauche
			=> on fait apparaître le curseur de déplacement
		 */

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
        {
            //Debug.Log("Create");
            SpawnTeleportCursor();
        }
        /**
			Le joystick gauche n'est plus appuyé :
			=> on fait disparaître le curseur
		 */
        if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick))
        {
            TeleportPlayer();
            Destroy(m_TeleportCursor);
            this.m_TeleportCursor = null;
        }
        /**
			Appui du joystick gauche (en continu)
			=> on déplace le curseur on fonction de l'état de l'axe 2D du joystick
		 */
       /* if (this.m_TeleportCursor != null) {
			if(OVRInput.Get(OVRInput.Button.PrimaryThumbstick)) {
				Vector2 axis2DLeftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
				if(axis2DLeftThumbstick != null) {
		//			MoveTeleportCursor(axis2DLeftThumbstick);
				}
			}
		}*/



	}

	private void SpawnTeleportCursor() {

		GameObject playerEyeObject = GameObject.Find("LeftEyeAnchor");
		GameObject playerObject = GameObject.Find("OVRPlayerController");
		Vector3 teleportCursorPosition = playerEyeObject.transform.position + playerEyeObject.transform.forward * 4;
		GameObject prefabTeleportCursor = Resources.Load("Prefabs/TallLocomotion") as GameObject;
		this.m_TeleportCursor = Instantiate(prefabTeleportCursor, new Vector3(teleportCursorPosition.x, -2.34f, teleportCursorPosition.z), Quaternion.Euler(-90, 0, 0));
		this.m_TeleportCursor.transform.parent = playerObject.transform;
        
	}

    /**
		Vector2 axis2DValues : x => valeur horizontale du joystick
							   y => valeur verticale du joystick

		Donc on utilise le y comme z, comme le y du l'espace Unity est invariant (pour l'instant)
		

		BUG QUE FIFI DOIT RESOUDRE : Les offsets de déplacement se font selon le World.Space, il faut que je change ça par rapport à la cam' 
	 */
    /*	private void MoveTeleportCursor(Vector2 axis2DValues) {
            Vector3 newCursorPosition = this.m_TeleportCursor.transform.localPosition + new Vector3(axis2DValues.x, 0.0f, axis2DValues.y);
            this.m_TeleportCursor.transform.localPosition = newCursorPosition;

        }*/

    /**
		Le joueur est téléporté à l'endroit du curseur
	 */
    private void TeleportPlayer()
    {
        GameObject playerObject = GameObject.Find("OVRPlayerController");
        // recupere les coordonnées du tp 
        GameObject tp = GameObject.Find("magic_ring_01(Clone)");
        Vector3 newPlayerPosition = new Vector3(tp.transform.position.x, playerObject.transform.position.y , tp.transform.position.z);
        playerObject.transform.position = newPlayerPosition;

    }

}
