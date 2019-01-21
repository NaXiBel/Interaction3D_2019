/************************************************************************************

Copyright   :   Copyright 2017-Present Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

public class RayInteraction : MonoBehaviour {

    private static GameObject m_GrabbedPoint = null;

    private static GameObject m_HitPoint = null;
    public Material m_HoverMaterial = null;
    public Material m_BSplineMaterial = null;
    public Material m_ControlPointMaterial = null;

    public static GameObject GrabbedPoint {
        get {
            return RayInteraction.m_GrabbedPoint;
        }

        set {
            RayInteraction.m_GrabbedPoint = value;
        }
    }

    public static GameObject HitPoint {
        get {
            return RayInteraction.m_HitPoint;
        }

        set {
            RayInteraction.m_HitPoint = value;
        }
    }

    public void OnHoverEnter(Transform t) {
        Debug.Log(t.gameObject.name);

        if(t.gameObject.name == "Bspline") {

        } else {
            t.gameObject.GetComponent<Renderer>().material = this.m_HoverMaterial;
        }
    

    }

    public void OnHoverExit(Transform t) {
        if(t.gameObject.name == "Bspline") {
            t.gameObject.GetComponent<Renderer>().material = this.m_BSplineMaterial;
        } else if(t.gameObject.name == "Sphere") {
            t.gameObject.GetComponent<Renderer>().material = this.m_ControlPointMaterial;
        }
    }

    public void OnSelected(Transform t) {
        if(t.gameObject.name == "Sphere") {
            if(!Const.m_ControlPoints.Contains(t.gameObject)) {
                Const.m_ControlPoints.Add(t.gameObject);
            } else {
                Const.m_ControlPoints.Remove(t.gameObject);
            }
            Debug.Log(Const.m_ControlPoints.Count);

        }

    }
    
         
    public void OnGrabbed(Transform t) {
        if(t.gameObject.name == "Sphere") {
            RayInteraction.m_GrabbedPoint = t.gameObject;
            GameObject ray = GameObject.Find("SelectionVisualizer");
            Debug.Log(ray);

            RayInteraction.m_HitPoint = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            RayInteraction.m_HitPoint.transform.parent = ray.transform;
            RayInteraction.m_HitPoint.transform.localPosition = t.position;
        }

    }
}
