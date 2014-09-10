using UnityEngine;
using System.Collections;

public class TargetMover : MonoBehaviour
{

    /** Mask for the raycast placement */
    public LayerMask mask;

    public Transform target;

    /** Determines if the target position should be updated every frame or only on double-click */
    public bool onlyOnDoubleClick;

    Camera cam;
    GameObject go;

    public void Start()
    {
        //Cache the Main Camera
        go = GameObject.FindGameObjectWithTag("CameraParent");
        cam = Camera.main;
    }
    private bool goCutTree = false;
    public void OnGUI()
    {

        if (onlyOnDoubleClick && Input.GetKeyDown(KeyCode.Mouse1) && go.GetComponent<UnitSelection>().isAnySelect)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask))
            {
                target.position = hit.point;
                if (hit.transform.tag == "Tree")
                {
                    if (!hit.transform.GetComponent<IsSelectObject>().selObj.transform.animation.isPlaying)
                    {
                        StartCoroutine(hit.transform.GetComponent<IsSelectObject>().JustPlay());
                    }
                    target.position = hit.transform.position;
                }
            }

            foreach (GameObject unit in transform.parent.GetComponent<UnitSelection>().selectedUnits)
            {
                if (unit.GetComponent<MineBotAI>().target)
                {
                    Destroy(unit.GetComponent<MineBotAI>().target.gameObject);
                }
                //Destroy(unit.GetComponent<MineBotAI>().target);
                unit.GetComponent<MineBotAI>().target = Instantiate(target, target.position, Quaternion.identity) as Transform;
                if (hit.transform.tag == "Tree")
                {
                    unit.GetComponent<Unit>().choppedTree = hit.transform.gameObject;
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!onlyOnDoubleClick && cam != null)
        {
            UpdateTargetPosition();
        }

    }

    public void UpdateTargetPosition()
    {
        //Fire a ray through the scene at the mouse position and place the target where it hits
        RaycastHit hit;
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask))
        {
            target.position = hit.point;
            if (hit.transform.tag == "Tree")
            {
                if (!hit.transform.GetComponent<IsSelectObject>().selObj.transform.animation.isPlaying)
                {
                    StartCoroutine(hit.transform.GetComponent<IsSelectObject>().JustPlay());
                }
                target.position = hit.transform.position;
            }
        }
    }

}
