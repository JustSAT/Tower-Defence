using UnityEngine;
using System.Collections;

public class TowerHead : MonoBehaviour {

    private Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        target = transform.parent.parent.GetComponent<Tower>().target;

        if (target)
        {
            Quaternion neededRotation = Quaternion.LookRotation(target.position - transform.position);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, neededRotation, Time.deltaTime * transform.parent.parent.GetComponent<Tower>().rotationSpeed);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
        /*else
        {
            Quaternion neededRotation = Quaternion.identity;
            transform.rotation = Quaternion.Slerp(transform.rotation, neededRotation, Time.deltaTime * transform.parent.parent.GetComponent<Tower>().rotationSpeed);
        }*/
	}
}
