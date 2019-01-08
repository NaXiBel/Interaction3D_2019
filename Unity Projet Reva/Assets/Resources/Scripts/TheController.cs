using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheController : MonoBehaviour {
    public GameObject translation;
    public GameObject rotation;
    public GameObject[] tab;
    public GameObject go;
    public GameObject Bspline;

    private GameObject maSpline;

    private float rotX;
    private float rotY;
    private float rotZ;

   /* private Vector3 rot;
    private Vector3 tran;
    private Vector3 tranTampon;
    */
    private GameObject[] lines;


    // Use this for initialization
    void Start () {
         GameObject controller;
        switch (Const.Controller)
        {
            case (int)Const.ControllerName.Oculus:
                controller = (GameObject)Instantiate(Resources.Load("Prefabs/OculusController"));
                break;
            case (int)Const.ControllerName.LeapMotion:
                
                controller = (GameObject)Instantiate(Resources.Load("Prefabs/LeapMotionController"));
                break;
        }

        //creation de l'objet modelisant la surface Bspline
        maSpline = new GameObject("Bspline");
        maSpline.AddComponent<MeshFilter>();
        maSpline.AddComponent<MeshRenderer>();
        maSpline.AddComponent<Rigidbody>();
        maSpline.GetComponent<Rigidbody>().useGravity = false;
        maSpline.GetComponent<Rigidbody>().isKinematic = true;
        maSpline.AddComponent<MeshCollider>();
        maSpline.GetComponent<Renderer>().material = Resources.Load("Materials/B_Spline", typeof(Material)) as Material;
        maSpline.layer = 10;
        maSpline.AddComponent<Bspline>();
        maSpline.GetComponent<Bspline>().Start();
        //maSpline.AddComponent(Type.GetType("Resources/NoSharedVertices"));
        //initialisation des points de controle
        tab = new GameObject[25];
        InitiateTab();

        //initialisation des lignes entre les points
        lines = new GameObject[5*4*2];
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
        for (int i = 0; i < 5 * 4 * 2; i++)
        {
            lines[i] = new GameObject(String.Format("Ligne {0}", i));
            lines[i].AddComponent<LineRenderer>();
            lines[i].GetComponent<LineRenderer>().positionCount = 2;
            //largeur de la ligne
            lines[i].GetComponent<LineRenderer>().widthMultiplier = 0.05f;
            lines[i].GetComponent<LineRenderer>().material = Resources.Load("Materials/LineMaterial", typeof(Material)) as Material;
            lines[i].layer = 10;
        }
        UpdateLines();

        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        // tranTampon = translation.transform.position;

    }

    //initialise les points de controle
    void InitiateTab()
    {
        for (int i = 0; i < 25; i++)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3(maSpline.GetComponent<Bspline>().xcontr[i], maSpline.GetComponent<Bspline>().ycontr[i], maSpline.GetComponent<Bspline>().zcontr[i]);
            //changement de la taille des points de controle
            go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = false;      
            go.GetComponent<Rigidbody>().isKinematic = true;      
            go.GetComponent<Renderer>().material = Resources.Load("Materials/Control", typeof(Material)) as Material;

            // si on a la leap on ajoute le script qui permet de grapper les points de contrôles
            if (Const.Controller == (int)Const.ControllerName.LeapMotion)
            {
                go.AddComponent<Leap.Unity.Interaction.InteractionBehaviour>();
            }
            go.transform.parent = this.transform;

            tab[i] = go;
            go.layer = 10;
            go.AddComponent<OVRGrabbable>();
            go.GetComponent<OVRGrabbable>().enabled = true;
            go.AddComponent<Point>();
            
        }
    }

    //mise à jour des lignes entre les points de controle
    void UpdateLines()
    {
        int ind = 0;
        for(int i = 0; i < 25; i++)
        {
            if(i < 20)
            {
                lines[ind].GetComponent<LineRenderer>().SetPosition(0, tab[i].transform.position);
                lines[ind].GetComponent<LineRenderer>().SetPosition(1, tab[i + 5].transform.position);
                ind++;
            }
            if(i % 5 != 4)
            {
                lines[ind].GetComponent<LineRenderer>().SetPosition(0, tab[i].transform.position);
                lines[ind].GetComponent<LineRenderer>().SetPosition(1, tab[i + 1].transform.position);
                ind++;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*rot = rotation.transform.position;
        tran = translation.transform.position;

        transform.position = new Vector3(tran.x - 2.0f, tran.y + 0.0f, tran.z + 0.0f);
        transform.rotation = rotation.transform.rotation;//Quaternion.FromToRotation( new Vector3(-2.0f, +2.0f,0.0f), new Vector3(rot.x -tranTampon.x, rot.y - tranTampon.y, rot.z - tranTampon.z));
        */
        //tranTampon = translation.transform.position;

        //mise a jour des points de controles
        for(int i = 0; i < 25; i++)
        {
            maSpline.GetComponent<Bspline>().xcontr[i] = tab[i].transform.position.x;
            maSpline.GetComponent<Bspline>().ycontr[i] = tab[i].transform.position.y;
            maSpline.GetComponent<Bspline>().zcontr[i] = tab[i].transform.position.z;
        }
        maSpline.GetComponent<MeshCollider>().sharedMesh = maSpline.GetComponent<MeshFilter>().mesh;
        //mise a jour de la surface
        maSpline.GetComponent<Bspline>().Calc();
        //maSpline.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        //mise a jour des lignes entre les pooints

        UpdateLines();
    }

}