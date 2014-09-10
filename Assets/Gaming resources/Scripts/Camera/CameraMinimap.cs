using UnityEngine;
using System.Collections;

public class CameraMinimap : MonoBehaviour {
    public Vector3[] newVertices;
    public Vector2[] newUV;
    public int[] newTriangles;
	// Use this for initialization
	void Start () {
        Mesh mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;

        mesh.name = "CameraMinimapMesh";

        newVertices = mesh.vertices;
        newTriangles = mesh.triangles;
        newUV = mesh.uv;
        /*
        newTriangles[0] = 2;
        newTriangles[1] = 3;
        newTriangles[2] = 0;*/

        

        

        /*
        sp.transform.parent = transform;
        sp.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 30));
        newVertices[1] = sp.transform.localPosition;

        sp.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 30));
        newVertices[3] = sp.transform.localPosition;

        sp.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 30));
        newVertices[2] = sp.transform.localPosition;

        sp.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30));
        newVertices[0] = sp.transform.localPosition;
        */

        

        GetComponent<MeshFilter>().mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
        Mesh mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;

        GameObject sp = transform.GetChild(0).gameObject;
        sp.transform.parent = transform;
        sp.SetActive(false);
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(50, 50, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500.0f, 1 << 8))
        {
            hit.point = new Vector3(hit.point.x, 100, hit.point.z);
            sp.transform.position = hit.point;
            newVertices[1] = sp.transform.localPosition;
        }

        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width-50, 50, 0));
        if (Physics.Raycast(ray, out hit, 500.0f, 1 << 8))
        {
            hit.point = new Vector3(hit.point.x, 100, hit.point.z);
            sp.transform.position = hit.point;
            newVertices[3] = sp.transform.localPosition;
        }

        ray = Camera.main.ScreenPointToRay(new Vector3(50, Screen.height-50, 0));
        if (Physics.Raycast(ray, out hit, 500.0f, 1 << 8))
        {
            hit.point = new Vector3(hit.point.x, 100, hit.point.z);
            sp.transform.position = hit.point;
            newVertices[2] = sp.transform.localPosition;
        }

        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width-50, Screen.height-50, 0));
        if (Physics.Raycast(ray, out hit, 500.0f, 1 << 8))
        {
            hit.point = new Vector3(hit.point.x, 100, hit.point.z);
            sp.transform.position = hit.point;
            newVertices[0] = sp.transform.localPosition;
        }

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        GetComponent<MeshFilter>().mesh = mesh;
	}
}
