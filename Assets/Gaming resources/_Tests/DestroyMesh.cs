using UnityEngine;
using System.Collections;

public class DestroyMesh : MonoBehaviour
{
    public float radius = 0.5f, dmg = 5.0f, sqrMagnitude, distance, falloff;
    private MeshFilter unappliedMesh;
    private Mesh newMesh;
    public Mesh curMesh;

    enum FallOff { Gauss, Linear, Needle };
    FallOff fallOff = FallOff.Needle;

    float LinearFalloff(float distance, float inRadius)
    {
        float tempVar = 1.0f - distance / inRadius;
        return Mathf.Clamp01(tempVar);
    }

    float GaussFalloff(float dist, float inRadius)
    {
        float tempVar = distance / inRadius;
        float tempVar2 = -Mathf.Pow(tempVar, 2.5f) - 0.01f;
        return Mathf.Clamp01(Mathf.Pow(360.0f, tempVar2));
    }

    float NeedleFalloff(float dist, float inRadius)
    {
        return -(dist * dist) / (inRadius * inRadius) + 1.0f;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // When no button is pressed we update the mesh collider
        if (!Input.GetMouseButton(0))
        {
            // Apply collision mesh when we let go of button
            ApplyMeshCollider();
            return;
        }
    }

    void OnTriggerEnter(Collider objCol)
    {
        RaycastHit hit;
        Vector3 origin = objCol.transform.position;
        Vector3 dir = transform.forward;

        if (Physics.Raycast(origin, dir, out hit))
        {
            Debug.Log("Raycast worked");
            MeshFilter filter = hit.collider.GetComponent<MeshFilter>();
            if (filter)
            {
                if (filter != unappliedMesh)
                {
                    ApplyMeshCollider();
                    unappliedMesh = filter;
                }

                // Deform mesh
                var relativePoint = filter.transform.InverseTransformPoint(hit.point);
                DeformMesh(filter.mesh, relativePoint, dmg * Time.deltaTime, radius);
            }
        }
        Destroy(objCol.gameObject);
    }

    Mesh DeformMesh(Mesh mesh, Vector3 position, float power, float inRadius)
    {
        Vector3[] vertices = new Vector3[mesh.vertices.Length];
        Vector3[] normals = new Vector3[mesh.normals.Length];
        float sqrRadius = inRadius * inRadius;

        vertices = mesh.vertices;
        normals = mesh.normals;

        Vector3 averageNormal = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            sqrMagnitude = (vertices[i] - position).sqrMagnitude;

            if (sqrMagnitude > sqrRadius)
                continue;

            distance = Mathf.Sqrt(sqrMagnitude);
            falloff = LinearFalloff(distance, inRadius);
            averageNormal += falloff * normals[i];
        }

        averageNormal = averageNormal.normalized;

        for (int x = 0; x < vertices.Length; x++)
        {
            sqrMagnitude = (vertices[x] - position).sqrMagnitude;

            if (sqrMagnitude > sqrRadius)
                continue;

            distance = Mathf.Sqrt(sqrMagnitude);
            falloff = NeedleFalloff(distance, inRadius);
            vertices[x] += averageNormal * falloff * -power;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void ApplyMeshCollider()
    {
        Debug.Log("Updated");
        GetComponent<MeshCollider>().mesh = newMesh;
        newMesh = null;
    }
}