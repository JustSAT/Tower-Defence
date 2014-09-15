using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
    public GameObject prefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 9;
            if (Physics.Raycast(ray, out hit, 400.0f, layerMask))
            {
                Network.Instantiate(prefab, hit.point, Quaternion.identity, 2);
            }
        }
	}
}
