using UnityEngine;
using System.Collections;

public class plusAnimation : MonoBehaviour {
    public string text;
    Color yellow = Color.yellow;
	// Use this for initialization
	void Start () {
	    GetComponent<TextMesh>().text = "+" + text;
	}
	
	// Update is called once per frame
	void Update () {
        if (yellow.a > 0)
        {
            yellow.a -= 0.009f;
        }
        else if (yellow.a < 0)
            yellow.a = 0;
        else
            Destroy(gameObject);
        transform.GetComponent<TextMesh>().color = yellow;
        transform.position += new Vector3(0, Time.deltaTime*10, 0);

	}

}
