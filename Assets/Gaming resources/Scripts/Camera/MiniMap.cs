using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {
    private Camera myCamera;
    public bool mouseOverMinimap = false;
	// Use this for initialization
	void Start () {
        myCamera = transform.GetComponent<Camera>();
        myCamera.depth = Camera.main.depth +5;
	}
	
	// Update is called once per frame
	void Update () {
        
        myCamera.rect = new Rect(6.37f / Screen.width, 6.37f / Screen.width, 178.36f / Screen.width, 160.44f / Screen.height);
        myCamera.fieldOfView = 60;
        if (Input.mousePosition.x > 3 && Input.mousePosition.x < 180 && Input.mousePosition.y > 3 && Input.mousePosition.y < 172)
        {
            //print("Over minimap");
            mouseOverMinimap = true;
        }
        else
        {
            //print("Anywhere");
            mouseOverMinimap = false;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (mouseOverMinimap)
            {
                RaycastHit hit;
                int layerMask = 1 << 8;
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 400.0f, layerMask))
                {
                    
                    GameObject.FindGameObjectWithTag("CameraParent").GetComponent<CameraMovement>().SetCameraByMinimap(hit.point);
                }
            }
        }
	}
    void OnGUI()
    {
        
    }
    /*
    void OnMouseEnter()
    {
        mouseOverMinimap = true;
    }

    void OnMouseExit()
    {
        mouseOverMinimap = false;
    }*/
}
//myCamera.rect = new Rect(0.01f, 0.01f, (Screen.height / 1910.0f - (Mathf.Abs(Screen.height - Screen.width) / 3408.0f)) + 0.019f, Screen.height / 1910.0f);