using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBullet : MonoBehaviour {

    //[System.Serializable]
    public class Owner
    {
        public GameObject tower;
        public int arrayId;
    }

    public Owner myOwner;
    public Transform target;
    private Vector3 lastPosition = Vector3.zero;
    public float bulletSpeed = 10.0f;
    public float damage = 10.0f;
    public Transform particlePrefab;

    public List<AudioClip> hitAudioClips;

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
                    if(Network.isServer)
                        other.GetComponent<EnemyUnit>().ApplyDamage(damage, myOwner.tower, myOwner.arrayId);
                    ApplyDestroy();
                }
            }
        }

    }
    void ApplyDestroy()
    {
        destroyApllyed = true;
        audio.clip = hitAudioClips[Random.Range(0, 4)];
        audio.Play();
        transform.GetComponent<SphereCollider>().enabled = false;
        //transform.parent = target;
        //Second child its our bullet model

        transform.GetChild(1).transform.gameObject.SetActive(false);
        transform.GetChild(2).transform.gameObject.SetActive(false);
        Network.Instantiate(particlePrefab, transform.GetChild(1).transform.position, transform.GetChild(1).transform.rotation,7);
        //First child its Particle System.
        transform.GetChild(0).GetComponent<ParticleSystem>().loop = false;
        transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().loop = false;
        if(Network.isServer)
            Invoke("DestroyMe", 3);
    }
    void DestroyMe()
    {
        if (transform.GetComponent<NetworkView>())
        {
            Network.Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
