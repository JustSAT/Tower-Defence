using UnityEngine;
using System.Collections;

public class NodeP : MonoBehaviour {

    public bool canL = false;
    public bool canR = false;
    public bool canF = false;
    public bool canB = false;
    public bool canLF = false;
    public bool canRF = false;
    public bool canLB = false;
    public bool canRB = false;

    public int i;
    public int j;
	// Use this for initialization
	void Start () {
        //GameObject.FindGameObjectWithTag("Build").GetComponent<BuildAll>().SetNodes(this.transform);
        transform.parent = GameObject.FindGameObjectWithTag("MainNode").transform;
        transform.name = "Node";
        StartCoroutine(Start(1.0f));
	}
    IEnumerator Start(float s)
    {
        yield return new WaitForSeconds(s);
        GetComponent<Collider>().enabled = false;
        //GetComponent<MeshRenderer>().enabled = false;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
