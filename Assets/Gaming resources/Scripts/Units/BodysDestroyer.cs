using UnityEngine;
using System.Collections;

public class BodysDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(DisableCol());
        StartCoroutine(DestroyMe());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator DisableCol()
    {
        yield return new WaitForSeconds(2.0f);
        foreach (Transform child in transform)
        {
            foreach (Transform child2 in child)
            {
                child2.collider.enabled = false;
            }
            child.collider.enabled = false;
            //child.rigidbody.active = false;
        }
    }
    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
}
