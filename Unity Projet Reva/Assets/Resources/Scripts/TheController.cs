using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class TheController : MonoBehaviour {
    public GameObject translation;
    public GameObject rotation;
    public GameObject[] tab;
    public GameObject go;
    public GameObject Bspline;
    public bool hasToken;
    public bool modified;
    public TCPConnection myTCP;
    public GameObject player;
    private List<GameObject> users;
    public static SortedList<int, GameObject> usersList;
    public static int userHasToken;
    private float playerLastX;
    private float playerLastY;
    private float playerLastZ;

    private GameObject maSpline;

    private float rotX;
    private float rotY;
    private float rotZ;

   /* private Vector3 rot;
    private Vector3 tran;
    private Vector3 tranTampon;
    */
    private GameObject[] lines;

    private void Awake() {
        //add a copy of TCPConnection to this game object
        myTCP = TCPController.myTCP;
    }


    // Use this for initialization
    void Start () {
         GameObject controller;
        //add a copy of TCPConnection to this game object
        //myTCP = TCPController.myTCP;
        switch (Const.Controller)
        {
            case (int)Const.ControllerName.Oculus:
                controller = (GameObject)Instantiate(Resources.Load("Prefabs/OculusController"));
                GameObject eye = GameObject.Find("CenterEyeAnchor");
                GameObject touchEvent = GameObject.Find("TouchEvent");
                touchEvent.GetComponent<OVRInputModule>().rayTransform = eye.transform;
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
        tab = new GameObject[Const.m_NumberControlPoints];
        InitiateTab();
       // maSpline.GetComponent<MeshCollider>().convex = true;
       // maSpline.GetComponent<MeshCollider>().inflateMesh = true;
        maSpline.tag = "pointableSpine";
        //initialisation des lignes entre les points
        lines = new GameObject[(int)Math.Sqrt(Const.m_NumberControlPoints) * 4*2];
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
        for (int i = 0; i < (int)Math.Sqrt(Const.m_NumberControlPoints) * 4 * 2; i++)
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
        users = new List<GameObject>();
        usersList = new SortedList<int, GameObject>();
        if (Const.Controller == (int)Const.ControllerName.Oculus) player = GameObject.Find("OVRPlayerController");
        else player = GameObject.Find("Camera");
        Debug.Log("in Start : ");
        Debug.Log(TCPController.hasToken);
        if (!TCPController.hasToken) modified = false;
        else modified = true;
        playerLastX = 0.0f;
        playerLastY = 0.0f;
        playerLastZ = 0.0f;
        InvokeRepeating("SocketResponse", 0f, 0.01f);  //1s delay, repeat every 1s


        //if (Const.Controller == (int)Const.ControllerName.LeapMotion){
            GameObject bsplineControle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bsplineControle.transform.parent = this.transform;
            bsplineControle.name = "ControleB-Spline";
            //changement de la taille des points de controle
            bsplineControle.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            bsplineControle.AddComponent<Rigidbody>();
            bsplineControle.GetComponent<Rigidbody>().useGravity = false;
            bsplineControle.GetComponent<Rigidbody>().isKinematic = true;
            bsplineControle.GetComponent<Renderer>().material = Resources.Load("Materials/Control", typeof(Material)) as Material;
            bsplineControle.tag = "pointableSpine";

       // }
    }

    //initialise les points de controle
    void InitiateTab()
    {
        for (int i = 0; i < Const.m_NumberControlPoints; i++)
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
                go.tag = "pointable";
            }
            maSpline.tag = "pointableSpine";
            go.transform.parent = this.transform;

            tab[i] = go;
            go.layer = 10;
            go.AddComponent<OVRGrabbable>();
            go.GetComponent<OVRGrabbable>().enabled = true;
            go.AddComponent<Point>();
            go.GetComponent<Point>().Indice = i;
           // Const.m_ControlPoints.Add(go); // only test
        }
    }

    //mise à jour des lignes entre les points de controle
    public void UpdateLines()
    {
        int ind = 0;
        for(int i = 0; i < Const.m_NumberControlPoints; i++)
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
    
    public void RemovePoint(int indice)
    {

        for (int i = indice; i < Const.m_NumberControlPoints - 1; ++i)
        {
            maSpline.GetComponent<Bspline>().xcontr[i] = maSpline.GetComponent<Bspline>().xcontr[i + 1];
            maSpline.GetComponent<Bspline>().ycontr[i] = maSpline.GetComponent<Bspline>().ycontr[i + 1];
            maSpline.GetComponent<Bspline>().zcontr[i] = maSpline.GetComponent<Bspline>().zcontr[i + 1];
            tab[i] = tab[i + 1];
        }
        maSpline.GetComponent<Bspline>().xcontr[Const.m_NumberControlPoints - 1] = 0;
        maSpline.GetComponent<Bspline>().ycontr[Const.m_NumberControlPoints - 1] = 0;
        maSpline.GetComponent<Bspline>().zcontr[Const.m_NumberControlPoints - 1] = 0;
        tab[Const.m_NumberControlPoints - 1] = null;

        UpdateLines();
    }

    IEnumerator YieldingWork() {
        bool workDone = false;
        while (!workDone) {
            SocketResponse();
            yield return null;
        }
    }
    //socket reading script
    void SocketResponse() {
        Debug.Log(TCPController.hasToken);
        if (TCPController.hasToken) {
            //send 
            string strX = String.Join(",", maSpline.GetComponent<Bspline>().xcontr.Select(p => p.ToString()).ToArray());
            string strY = String.Join(",", maSpline.GetComponent<Bspline>().ycontr.Select(p => p.ToString()).ToArray());
            string strZ = String.Join(",", maSpline.GetComponent<Bspline>().zcontr.Select(p => p.ToString()).ToArray());
            string update = "3;"+TCPController.userId+";25;" + strX + ";" + strY + ";" + strZ ;
            if (modified) {
                Debug.Log(update);
                myTCP.writeSocket(update);
                modified = false;
            }
            
        }
        //Sending user position
        float x, y, z;
        x = player.transform.position.x;
        y = player.transform.position.y;
        z = player.transform.position.z;
        if (x != playerLastX || y != playerLastY || z != playerLastZ)
        {
            playerLastX = x;
            playerLastY = y;
            playerLastZ = z;
            string updatePlayerPos = "4" + ";" + x + ";" + y + ";" + z ;
            Debug.Log("Sending player position : " + updatePlayerPos);
            myTCP.writeSocket(updatePlayerPos);

        }
        //retreive 
        string serverSays = myTCP.readSocket();
        if (serverSays != "") {
            UpdateFromServer(serverSays);
        }
        
    }

    // {5; nbUser; nbPointsDeControles; tabUserX; tabUserY; tabUserZ; tabPtX; tabPtY; tabPtZ}
    void UpdateFromServer(string serverSays)
    {
        if (serverSays != "")
        {
            Debug.Log(serverSays);
            String[] data;
            data = serverSays.Split(';');
            if (data[0] == "0")
            {
                if (!TCPController.isIdObtained)
                {
                    //First update : 0;id;posx;posy;posz
                    TCPController.UserId = Int32.Parse(data[1]);
                    Debug.Log("Id has been obtained");
                    TCPController.isIdObtained = true;
                    usersList.Add(TCPController.UserId, null);
                    TCPController.AskSummary();
                }
                //data[2],data[3],data[4] are positions
                //data[5] = id1,id2,id3,id4
                string[] ids = data[5].Split(',');
                Debug.Log("ids length : " + ids.Length + "ids : " + ids);
                for(int j = 0; j < ids.Length; ++j)
                {
                    Debug.Log("ids[j] : " + ids[j]);
                    string replacement = Regex.Replace(ids[j], @"\t|\n|\r", " ");
                    string[] cleanID = replacement.Split(' ');
                    int parsedId = Int32.Parse(cleanID[0]);
                    if (TCPController.UserId != parsedId)
                    {
                        //Si c'est pas le meme
                        Debug.Log("About to create User : " + parsedId);
                        if (!usersList.ContainsKey(parsedId))
                        {
                            Debug.Log("Create User : " + parsedId);
                            GameObject tmp = (GameObject)Instantiate(Resources.Load("Prefabs/User"));
                            usersList.Add(parsedId, tmp);
                            Debug.Log("GameObject tmp :" + tmp);
                        }
                    } 
                }
                UserScrollViewIhm.UpdateUserList();


            }
            if (data[0] == "1")
            {
                //Player connected
                int user = Int32.Parse(data[1]);
                Debug.Log("user" + user + "isConnected");
                usersList.Add(user, (GameObject)Instantiate(Resources.Load("Prefabs/User")));
                UserScrollViewIhm.UpdateUserList();
            }
            if (data[0] == "2")
            {
                //Player disconnected
                int user = Int32.Parse(data[1]);
                Debug.Log("user" + user + "is disconnected");
                foreach (KeyValuePair<int, GameObject> kvp in usersList)
                {
                    if (kvp.Key == user) Destroy(kvp.Value);
                }
                usersList.Remove(user);
                UserScrollViewIhm.UpdateUserList();
            }
            if (Int32.Parse(data[0]) == 5)
            {
                Debug.Log("Update summary");
                int nbUser = Int32.Parse(data[1]);
                int nbPt = Int32.Parse(data[2]);
                string[] tabUserX = data[3].Split(',');
                string[] tabUserY = data[4].Split(',');
                string[] tabUserZ = data[5].Split(',');
                
                for (int i = 0; i < usersList.Count && nbUser>1; ++i)
                {
                    
                    Debug.Log("update other user position");
                    IList<GameObject> ilistValues = usersList.Values;
                    if (i != usersList.IndexOfKey(TCPController.UserId))
                    {
                        Debug.Log("Updating id from key: " + i);
                        ilistValues[i].transform.position = new Vector3(float.Parse(tabUserX[i], CultureInfo.InvariantCulture.NumberFormat),
                              float.Parse(tabUserY[i], CultureInfo.InvariantCulture.NumberFormat),
                              float.Parse(tabUserZ[i], CultureInfo.InvariantCulture.NumberFormat));
                    }
                    /*
                    foreach (KeyValuePair<int, GameObject> kvp in usersList)
                    {
                        
                    }
                    */
                }
                if (!TCPController.hasToken)
                { // !hasToken No need to retreive sended BSpline coordonate
                    Debug.Log("Updating points");
                    string[] tabX = data[6].Split(',');
                    string[] tabY = data[7].Split(',');
                    string[] tabZ = data[8].Split(',');
                    for (int i = 0; i < nbPt; ++i)
                    {
                        maSpline.GetComponent<Bspline>().xcontr[i] = float.Parse(tabX[i], CultureInfo.InvariantCulture.NumberFormat);
                        maSpline.GetComponent<Bspline>().ycontr[i] = float.Parse(tabY[i], CultureInfo.InvariantCulture.NumberFormat);
                        maSpline.GetComponent<Bspline>().zcontr[i] = float.Parse(tabZ[i], CultureInfo.InvariantCulture.NumberFormat);
                        tab[i].transform.position = new Vector3(float.Parse(tabX[i], CultureInfo.InvariantCulture.NumberFormat), float.Parse(tabY[i], CultureInfo.InvariantCulture.NumberFormat), float.Parse(tabZ[i], CultureInfo.InvariantCulture.NumberFormat));
                        Debug.Log("Vec3 : " + float.Parse(tabX[i], CultureInfo.InvariantCulture.NumberFormat)+","+ float.Parse(tabY[i], CultureInfo.InvariantCulture.NumberFormat) + "," + float.Parse(tabZ[i], CultureInfo.InvariantCulture.NumberFormat));
                        SetActiveGrabScript(false);


                    }
                    maSpline.GetComponent<MeshCollider>().sharedMesh = maSpline.GetComponent<MeshFilter>().mesh;
                    //mise a jour de la surface
                    maSpline.GetComponent<Bspline>().Calc();
                    //maSpline.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                    //mise a jour des lignes entre les pooints
                    UpdateLines();
                }
                string replacement = Regex.Replace(data[9], @"\t|\n|\r", " ");
                string[] cleanID = replacement.Split(' ');
                //Debug.Log("replacement "+ cleanID);
                //int j = 0;
                //while (!Char.IsWhiteSpace(replacement[j++])) { };
                
                Debug.Log(" ID :" + Int32.Parse(cleanID[0]) + " has the token");
                Debug.Log("Result from test +" + (Int32.Parse(cleanID[0]) == TCPController.UserId) + "userID=" + TCPController.UserId + "cleanID=" + Int32.Parse(cleanID[0]));

                if (!TCPController.hasToken && Int32.Parse(cleanID[0]) == TCPController.UserId)
                {
                    TCPController.hasToken = true;
                    TCPController.userHasToken = TCPController.UserId;
                    SetActiveGrabScript(true);
                }
                else if (Int32.Parse(cleanID[0]) == TCPController.UserId)
                {
                    TCPController.hasToken = true;
                    TCPController.userHasToken = TCPController.UserId;
                }
                else
                {
                    TCPController.hasToken = false;
                    TCPController.userHasToken = Int32.Parse(cleanID[0]);
                    SetActiveGrabScript(false);
                }
                UserScrollViewIhm.UpdateUserList();
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
   {
        if (TCPController.hasToken)
        {
            for (int i = 0; i < Const.m_NumberControlPoints; i++)
            {
                if (modified || maSpline.GetComponent<Bspline>().xcontr[i] != tab[i].transform.position.x)
                {
                    maSpline.GetComponent<Bspline>().xcontr[i] = tab[i].transform.position.x;
                    modified = true;
                }
                if (modified || maSpline.GetComponent<Bspline>().ycontr[i] != tab[i].transform.position.y)
                {
                    maSpline.GetComponent<Bspline>().ycontr[i] = tab[i].transform.position.y;
                    modified = true;
                }
                if (modified || maSpline.GetComponent<Bspline>().zcontr[i] != tab[i].transform.position.z)
                {
                    maSpline.GetComponent<Bspline>().zcontr[i] = tab[i].transform.position.z;
                    modified = true;
                }
            }
            if (modified)
            {
                Debug.Log("Modified - MAJ BSpline");
                maSpline.GetComponent<MeshCollider>().sharedMesh = maSpline.GetComponent<MeshFilter>().mesh;
                //mise a jour de la surface
                maSpline.GetComponent<Bspline>().Calc();
                //maSpline.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                //mise a jour des lignes entre les pooints
                UpdateLines();
            }
        }

    }
    private void SetActiveGrabScript (bool b)
    {
        for (int i = 0; i < tab.Length; ++i)
        {
            if (Const.Controller == (int)Const.ControllerName.Oculus) tab[i].GetComponent<OVRGrabbable>().enabled = b;
            else tab[i].GetComponent<Leap.Unity.Interaction.InteractionBehaviour>().enabled = b;
        }
    }
    
}