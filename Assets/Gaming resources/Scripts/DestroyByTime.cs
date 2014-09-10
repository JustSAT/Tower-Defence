using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	// Use this for initialization
    void DestroyTime(float time)
    {
        StartCoroutine(DestroyMe(time));
    }
    void Start()
    {
        StartCoroutine(DestroyMe(2));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator DestroyMe(float s)
    {
        yield return new WaitForSeconds(s);
        Destroy(this.gameObject);
    }
}
