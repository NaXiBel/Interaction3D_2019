using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour {

	public GameObject m_TeleportCursor = null;
    public GameObject m_SelectionRay = null;

    private GameObject m_Selected = null;

    // Temporaire, amené à changer
    private GameObject m_BSpline = null;
    private enum SelectionStates {
        NONE,
        RAY,
        HAND
    }

    private SelectionStates m_State = SelectionStates.NONE;
    // Use this for initialization
    private ControllerSelection.OVRRawRaycaster m_RayHandlerScript = null;
	void Start () {
        this.m_RayHandlerScript = this.GetComponent<ControllerSelection.OVRRawRaycaster>();
        this.m_RayHandlerScript.enabled = false;
        this.m_SelectionRay.SetActive(false);
        this.m_State = SelectionStates.HAND;
        this.m_BSpline = GameObject.Find("Controller");
    }
    void Update() {

            /**
            
                Gestion du rayon de sélection
                Si l'index droit est levé et (état sélection main)
                => on active le script / l'objet du rayon dans notre main |ETAT SELECTION RAYON

                Si l'index droit est collé à la cachette, (état sélection rayon)
                => on désactive le script / l'objet du rayon dans notre main |ETAT SELECTION RAYON
            */
            if(!OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger) && this.m_State == SelectionStates.HAND) {
                this.m_SelectionRay.SetActive(true);
                this.m_RayHandlerScript.enabled = true;
                this.m_State = SelectionStates.RAY;

            } else if(OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger) && this.m_State == SelectionStates.RAY) {
                this.m_SelectionRay.SetActive(false);
                this.m_RayHandlerScript.enabled = false;
                this.m_State = SelectionStates.HAND;

            }

            if(this.m_State == SelectionStates.HAND) {
                if(OVRInput.Get(OVRInput.RawButton.A)) {
                    Debug.Log("OK");
                        
                    Vector2 axis2DLeftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                    if(axis2DLeftThumbstick != null) {
                        MoveBSpline(axis2DLeftThumbstick);
                    }
                    
                } else if(OVRInput.Get(OVRInput.RawButton.B)) {
                    Vector2 axis2DLeftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                    if(axis2DLeftThumbstick != null) {
                        RotateBSpline(axis2DLeftThumbstick);
                    }
                }
                /** 
                    Utilisation de A et B pour monter ou descendre 
                */
                if (OVRInput.GetDown(OVRInput.RawButton.Y)) {
                    GameObject playerObject = GameObject.Find("OVRPlayerController");
                    playerObject.transform.Translate(0f, 0.3f, 0f);
                }
                if (OVRInput.GetDown(OVRInput.RawButton.X)) {
                    GameObject playerObject = GameObject.Find("OVRPlayerController");
                    playerObject.transform.Translate(0f, -0.3f, 0f);
                }
                /**
                    Pression du joystick gauche
                    => on fait apparaître le curseur de déplacement
                */

                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)) {
                    //Debug.Log("Create");
                    SpawnTeleportCursor();
                }
                /**
                    Le joystick gauche n'est plus appuyé :
                    => on fait disparaître le curseur
                */
                if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick)) {
                    TeleportPlayer();
                    Destroy(m_TeleportCursor);
                    this.m_TeleportCursor = null;
                }
            } else if(this.m_State == SelectionStates.RAY) {
                /** 
                    Utilisation de A et B pour monter ou descendre 
                */
                if (OVRInput.GetDown(OVRInput.RawButton.Y)) {
                    GameObject playerObject = GameObject.Find("OVRPlayerController");
                    playerObject.transform.Translate(0f, 0.3f, 0f);
                }
                if (OVRInput.GetDown(OVRInput.RawButton.X)) {
                    GameObject playerObject = GameObject.Find("OVRPlayerController");
                    playerObject.transform.Translate(0f, -0.3f, 0f);
                }
                /**
                    Pression du joystick gauche
                    => on fait apparaître le curseur de déplacement
                */

                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)) {
                    //Debug.Log("Create");
                    SpawnTeleportCursor();
                }
                /**
                    Le joystick gauche n'est plus appuyé :
                    => on fait disparaître le curseur
                */
                if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick)) {
                    TeleportPlayer();
                    Destroy(m_TeleportCursor);
                    this.m_TeleportCursor = null;
                }
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

    // Temporaire, amenée à changer
    private void MoveBSpline(Vector2 axis2DValues) {
        Debug.Log(axis2DValues);
        GameObject playerObject = GameObject.Find("OVRPlayerController");
        Vector3 move = new Vector3(axis2DValues.x, 0.0f, axis2DValues.y) * 0.2f; 
        this.m_BSpline.transform.Translate(move, playerObject.transform);
    }

    private void RotateBSpline(Vector2 axis2DValues) {
        Vector3 move = new Vector3(axis2DValues.y, axis2DValues.x, 0.0f); 
        this.m_BSpline.transform.Rotate(move);
    }
}
