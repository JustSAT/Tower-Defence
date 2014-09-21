using UnityEngine;
using System.Collections;


public class MeshDeformation : MonoBehaviour {
    
    public float upValue = 1.0f;

    private Vector3[] newVertices;
    private Vector2[] newUV;
    private int[] newTriangles;
    private Transform myRaycastPoint;
    public bool doAlways = false;
    void Start()
    {
        myRaycastPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        myRaycastPoint.parent = transform;
        myRaycastPoint.transform.position = Vector3.zero;
        myRaycastPoint.renderer.enabled = false;

        Mesh mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;

        mesh.name = "DynamicMesh";

        newVertices = mesh.vertices;
        newUV = mesh.uv;
        newTriangles = mesh.triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }
	// Update is called once per frame
	void Update () {

        if (doAlways)
        {
            Mesh mesh = new Mesh();
            mesh = GetComponent<MeshFilter>().mesh;

            mesh.vertices = newVertices;
            mesh.uv = newUV;
            mesh.triangles = newTriangles;
            for (int i = 0; i < newVertices.Length; i++)
            {
                RaycastHit hit;
                myRaycastPoint.parent = transform;
                myRaycastPoint.transform.localPosition = newVertices[i] + Vector3.up * 20;
                int layerMask = 1 << 8;
                if (Physics.Raycast(myRaycastPoint.transform.position, -Vector3.up, out hit, 100.0f, layerMask))
                {
                    myRaycastPoint.transform.position = hit.point + new Vector3(0, upValue, 0);
                    newVertices[i] = myRaycastPoint.transform.localPosition;
                }
            }
            GetComponent<MeshFilter>().mesh = mesh;
        }
	}

    public void Calculate()
    {
        for (int q = 0; q < 1; q++)
        {
            Debug.LogWarning("q = " + q);
            Mesh mesh = new Mesh();
            mesh = GetComponent<MeshFilter>().mesh;

            mesh.vertices = newVertices;
            mesh.uv = newUV;
            mesh.triangles = newTriangles;
            for (int i = 0; i < newVertices.Length; i++)
            {
                RaycastHit hit;
                myRaycastPoint.parent = transform;
                myRaycastPoint.transform.localPosition = newVertices[i] + Vector3.up * 20;
                int layerMask = 1 << 8;
                if (Physics.Raycast(myRaycastPoint.transform.position, -Vector3.up, out hit, 100.0f, layerMask))
                {
                    myRaycastPoint.transform.position = hit.point + new Vector3(0, upValue, 0);
                    newVertices[i] = myRaycastPoint.transform.localPosition;
                }
            }
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
