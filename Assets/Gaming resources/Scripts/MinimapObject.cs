using UnityEngine;
using System.Collections;

public class MinimapObject : MonoBehaviour {
    public Color objectColor;
    public Transform tObject;

	// Use this for initialization
	void Start () {
        objectColor.a = 1.0f;
        tObject.transform.renderer.material.color = objectColor;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
