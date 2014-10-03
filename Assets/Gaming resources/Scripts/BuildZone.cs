using UnityEngine;
using System.Collections;

public class BuildZone : MonoBehaviour {
    public Transform structure;
    public Transform backlight;
    public bool isBuilded = false;
    private float transparent = 1.0f;
    public Vector3 buildPosition;
	// Use this for initialization
	void Start () {
        backlight = transform.GetChild(0);
        buildPosition = transform.position + new Vector3(0, transform.localScale.y / 2, 0);
        RaycastHit hit;
        int layerMask = 1 << 8;
        if (Physics.Raycast(transform.position + Vector3.up * 20, -Vector3.up, out hit, 100.0f, layerMask))
        {
            buildPosition = hit.point;
        }
        backlight.transform.position = buildPosition + new Vector3(0, 0.4f, 0);
        Invoke("DisableBacklight", .1f);
        SetBuilded(false);
	}
	
	// Update is called once per frame
	void Update () {

	}
    void FixedUpdate()
    {
        if(isYellowed)
            OnMouseExit();
    }
    void DisableBacklight()
    {
        backlight.gameObject.SetActive(false);
        
    }
    bool isYellowed = false;
    public void OnMouseEnter()
    {
        if (backlight.gameObject.active && !isBuilded)
        {
            Color color = Color.yellow;
            /*color.a = transparent;
            backlight.renderer.material.color = color;
            */
            Material mat = new Material(backlight.GetComponent<Projector>().material);
            mat.color = color;
            backlight.GetComponent<Projector>().material = mat;
            isYellowed = true;
        }
    }
    public void OnMouseExit()
    {
        if (isYellowed)
        {
            if (!isBuilded)
            {
                Color color = Color.green;
                //color.a = transparent;
                Material mat = new Material(backlight.GetComponent<Projector>().material);
                mat.color = color;
                backlight.GetComponent<Projector>().material = mat;
            }
            else
            {
                Color color = Color.red;
                //color.a = transparent;
                Material mat = new Material(backlight.GetComponent<Projector>().material);
                mat.color = color;
                backlight.GetComponent<Projector>().material = mat;
            }
            isYellowed = false;
        }
    }

    public void SetBuilded(bool status)
    {
        isBuilded = status;
        if (status)
        {
            Color color = Color.red;
            //color.a = transparent;
            Material mat = new Material(backlight.GetComponent<Projector>().material);
            mat.color = color;
            backlight.GetComponent<Projector>().material = mat;
        }
        else
        {
            Color color = Color.green;
            //color.a = transparent;
            Material mat = new Material(backlight.GetComponent<Projector>().material);
            mat.color = color;
            backlight.GetComponent<Projector>().material = mat;
        }
    }
    
}
