using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
    public enum UnitType
    {
        inhabitant = 0,
        woodcutter = 1,
        warrior = 2,
        bowman = 3
    };
    public enum UnitEvents
    {
        idle = 0,
        walkTo = 1,
        goChopWood = 2,
        chopTree = 3
    };
    public UnitEvents currentUnitEvent = UnitEvents.idle;
    public UnitType thisUnitType = UnitType.inhabitant;     //Current unit type

    public GameObject choppedTree;                          //Tree which will be chopped


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TryCutTree(GameObject tree)
    {
        if (thisUnitType == UnitType.woodcutter)
        {
            CutTree(tree);
        }
    }

    private void CutTree(GameObject tree)
    {

    }

}
