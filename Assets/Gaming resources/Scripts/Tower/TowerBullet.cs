using UnityEngine;
using System.Collections;

public class TowerBullet : MonoBehaviour {
    public Transform target;
    private Vector3 lastPosition = Vector3.zero;
    public float bulletSpeed = 10.0f;
    public float damage = 10.0f;
    public Transform particlePrefab;
    private bool destroyApllyed = false;
	// Use this for initialization
	void Start () {
        if(target != null)
        lastPosition = target.position + new Vector3(0, target.transform.localScale.y, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
        {
            lastPosition = target.position + new Vector3(0, target.transform.localScale.y, 0);
            transform.LookAt(target.position + new Vector3(0, target.transform.localScale.y, 0));
            transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        }
        else
        {
            transform.LookAt(lastPosition);
            transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, lastPosition) < 0.5f)
            {
                if (!destroyApllyed)
                    ApplyDestroy();
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (target != null)
            {
                if (other.transform == target.transform)
                {
                    other.GetComponent<EnemyUnit>().ApplyDamage(damage);
                    ApplyDestroy();
                }
            }
        }

    }
    void ApplyDestroy()
    {
        destroyApllyed = true;
        transform.GetComponent<SphereCollider>().enabled = false;
        //transform.parent = target;
        //Second child its our bullet model

        transform.GetChild(1).transform.gameObject.SetActive(false);
        Instantiate(particlePrefab, transform.GetChild(1).transform.position, transform.GetChild(1).transform.rotation);
        //First child its Particle System.
        transform.GetChild(0).GetComponent<ParticleSystem>().loop = false;
        transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().loop = false;
        Invoke("DestroyMe", 3);
    }
    void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
