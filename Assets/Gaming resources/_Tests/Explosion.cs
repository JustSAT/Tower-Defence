using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float radius = 5.0F;
    public float power = 10.0F;
    void Start()
    {
        StartCoroutine(Boom());
    }

    void Update()
    {

    }
    IEnumerator Boom()
    {
        yield return new WaitForSeconds(1);
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        print("Prepare");
        foreach (Collider hit in colliders)
        {

            if (hit && hit.rigidbody)
            {
                hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 3.0F);
                print("Boom " + hit.name);
            }

        }
    }
}
