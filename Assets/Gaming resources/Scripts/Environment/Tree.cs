using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {
    public enum TreeTypes
    {
        defaultTree = 50,
        palm = 30,
        oak = 70

    };
    public TreeTypes thisTreeType = TreeTypes.defaultTree;
    public int currentTreeResource;

	// Use this for initialization
	void Start () {
        currentTreeResource = (int)thisTreeType;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
