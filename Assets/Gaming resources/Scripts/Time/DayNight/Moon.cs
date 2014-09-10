using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {

    private GameObject mySun;

	// Use this for initialization
	void Start () {
        mySun = GameObject.FindGameObjectWithTag("Sun");
	    
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler((mySun.transform.rotation.eulerAngles.x + 180), 309, mySun.transform.rotation.eulerAngles.z);
        if (!(mySun.transform.rotation.eulerAngles.x > 180 && mySun.transform.rotation.eulerAngles.x < 360))
        {
            transform.gameObject.SetActive(false);
        }
        else
            transform.gameObject.SetActive(true);
        
	}
}
