using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {
    public float seconds = 2.0f;
	// Use this for initialization
    void DestroyTime(float time)
    {
        StartCoroutine(DestroyMe(time));
    }
    void Start()
    {
        StartCoroutine(DestroyMe(seconds));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator DestroyMe(float s)
    {
        yield return new WaitForSeconds(s);
        if (transform.GetComponent<NetworkView>())
        {
            if(transform.animation)
                if(transform.animation.isPlaying)
                    StartCoroutine(DestroyMe(2));
                else
                    Network.Destroy(this.gameObject);
            else
                Network.Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
