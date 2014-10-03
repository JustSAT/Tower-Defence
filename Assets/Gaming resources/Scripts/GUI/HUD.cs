using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
    [System.Serializable]
    public enum HUDType
    {
        left = 0,
        middle = 1,
        right = 2,
        minimap = 3,
        leftbg = 4
    };
    public Texture texture;


    public HUDType myHudType;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        if (myHudType == HUDType.left)
        {
            GetComponent<GUITexture>().pixelInset = new Rect(-Screen.width / 2 - 1, -Screen.height / 2 - 1, 300, 200);
            
        }
        else if (myHudType == HUDType.middle)
        {
            GetComponent<GUITexture>().pixelInset = new Rect(-(Screen.width - 450) / 2, -Screen.height / 2 - 1, Screen.width - 450, 150);
        }
        else if (myHudType == HUDType.right)
        {
            GetComponent<GUITexture>().pixelInset = new Rect(Screen.width / 3 / 2, -Screen.height / 2 - 1, Screen.width / 3, 200);
        }
        
    }
    void OnGUI()
    {
        GUI.depth = 2;
        if (myHudType == HUDType.left)
        {
            GUI.DrawTexture(new Rect(-Screen.width / 2 - 1 + (Screen.width / 2), Screen.height-200, 300, 200), texture); 
        }
        else if (myHudType == HUDType.middle)
        {
            GUI.DrawTexture(new Rect(-(Screen.width - 450) / 2 + (Screen.width / 2), Screen.height-150, Screen.width - 450, 150), texture);
        }
        else if (myHudType == HUDType.right)
        {
            GUI.DrawTexture(new Rect(Screen.width / 3 / 2 + (Screen.width / 2), Screen.height - 200, Screen.width / 3, 200), texture);
        }
        else if (myHudType == HUDType.minimap)
        {
            GUI.depth = 1;
            Graphics.DrawTexture(new Rect(5, Screen.height - 205, 300, 200), texture);
        }
         

    }
    void OnMouseEnter()
    {
        //GameObject.FindGameObjectWithTag("CameraParent").GetComponent<UnitSelection>().cursorOnGUI = true;
    }
    void OnMouseExit()
    {
        //GameObject.FindGameObjectWithTag("CameraParent").GetComponent<UnitSelection>().cursorOnGUI = false;
    }
}
