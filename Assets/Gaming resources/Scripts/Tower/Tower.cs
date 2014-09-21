using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

    [System.Serializable]
    public class Owner
    {
        public NetworkPlayer owner;
        public int arrayId;
    }

    public Owner myOwner;
    public int kills = 0;

    public float damageRange = 15.0f;
    public float damage = 80.0f;
    public float attackSpeed = 1.0f;

    public float bulletSpeed = 100.0f;

    public float rotationSpeed = 10.0f;

    public float currentCooldown = 0.0f;

    public Transform target;
    public Transform bullet;
    public Transform bulletSpawnPosition;

    public Transform damageRangeBacklight;

    public List<Transform> targetMaybeNext;
	// Use this for initialization
	void Start () {
        
        if (Network.isServer)
        {
            myOwner = new Owner();
            NetworkingLobby nl = GameObject.FindGameObjectWithTag("Lobby").GetComponent<NetworkingLobby>();
            for (int i = 0; i < 4; i++)
            {
                if (myOwner.owner == nl.connectedPlayers[i].netPlayer && !nl.connectedPlayers[i].isEmptySlot)
                {
                    myOwner.arrayId = i;
                }
            }
        }
        if (transform.parent.networkView.isMine)
        {
            targetMaybeNext = new List<Transform>();
            target = null;
            transform.GetComponent<SphereCollider>().radius = damageRange;

            if (damageRangeBacklight)
                damageRangeBacklight.localScale = new Vector3(0.42f * damageRange, 0, 0.42f * damageRange);
        }
	}
   public bool tryShoot = false;
	// Update is called once per frame

   void Update()
   {
       if (!tryShoot)
           if (currentCooldown > 0.0f)
           {
               currentCooldown -= Time.deltaTime;
           }
           else if (currentCooldown < 0.0f)
           {
               currentCooldown = 0;
           }
           else if (currentCooldown == 0){
               if (target != null)
                   Shoot();
           }

       /*
       if (target != null)
       {
           if (!tryShoot)
           {
               StartCoroutine(Shoot(attackSpeed));
           }
           if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z)) / ((transform.localScale.x + transform.localScale.z) / 2) > damageRange)
           {
               StopCoroutine("Shoot");
           }
       }*/

       if (target == null && targetMaybeNext.Count > 0)
       {
           target = targetMaybeNext[0];
           targetMaybeNext.RemoveAt(0);
       }
   }

    void Shoot()
    {
        tryShoot = true;
        if (target != null && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z)) / ((transform.localScale.x + transform.localScale.z) / 2) <= damageRange)
        {
            
            if (bullet != null)
            {
                GameObject bul = Instantiate(bullet.gameObject, bulletSpawnPosition.position, Quaternion.identity) as GameObject;
                bul.GetComponent<TowerBullet>().target = target;
                bul.GetComponent<TowerBullet>().myOwner = new TowerBullet.Owner();
                bul.GetComponent<TowerBullet>().myOwner.arrayId = myOwner.arrayId;
                bul.GetComponent<TowerBullet>().myOwner.tower = transform.gameObject;
                bul.GetComponent<TowerBullet>().bulletSpeed = bulletSpeed;
                bul.GetComponent<TowerBullet>().damage = damage;
                currentCooldown = attackSpeed;
            }
        }
        
        tryShoot = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && target != null)
        {
            targetMaybeNext.Add(other.transform);
            for (int i = 0; i < targetMaybeNext.Count; i++)
            {
                if (targetMaybeNext[i] == null)
                {
                    targetMaybeNext.RemoveAt(i);
                }
            }
        }
        if (other.tag == "Enemy" && target == null)
        {
            target = other.transform;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            targetMaybeNext.Remove(other.transform);
        }
        if (other.transform == target)
        {
            target = null;
            if (targetMaybeNext.Count > 0)
            {
                target = targetMaybeNext[targetMaybeNext.Count-1];
                targetMaybeNext.RemoveAt(targetMaybeNext.Count - 1);
            }
        }
        
    }


}
