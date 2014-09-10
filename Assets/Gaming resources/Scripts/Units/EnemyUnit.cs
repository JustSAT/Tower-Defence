using UnityEngine;
using System.Collections;

public class EnemyUnit : MonoBehaviour {
    public string enemyName = "";
    public int enemyId = 0;
    public float maxHealth = 100.0f;
    public float curHealth = 100.0f;

    public int enemyCost = 20;

    public GUIStyle healthStyle;



    public Transform destroyParticle;
    public Transform destroyedBody;
    public Transform plusScore;

    public float explosionForce = 10000.0f;
    private bool startDeath = false;
	// Use this for initialization
	void Start () {
        curHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ApplyDamage(float damage)
    {
        if (!startDeath)
        {
            if (damage >= curHealth && !startDeath)
            {
                KillEnemy();
            }
            else
            {
                curHealth -= damage;
            }
        }
    }

    void KillEnemy()
    {
        curHealth = 0;
        startDeath = true;

        Transform particle = null;
        if(destroyParticle)
            particle = Instantiate(destroyParticle, transform.position, Quaternion.identity) as Transform;

        Transform go = null;
        if(destroyedBody)
            go = Instantiate(destroyedBody, transform.position, Quaternion.identity) as Transform;

        if(transform.GetComponent<MineBotAI>())
            transform.GetComponent<MineBotAI>().enabled = false;
        else
            transform.GetComponent<BotAI>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);

        if (particle != null)
        {
            Vector3 explosionPos = particle.position + new Vector3(Random.RandomRange(-2.0f,2.0f), Random.RandomRange(-2.0f,2.0f), Random.RandomRange(-2.0f,2.0f));
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 20);
            foreach (Collider hit in colliders)
            {
                if (hit && hit.rigidbody)
                {
                    hit.rigidbody.AddExplosionForce(Random.RandomRange(explosionForce / 4, explosionForce), explosionPos, 20, 3.0F);
                }

            }
        }
        Transform plusScoreInst = Instantiate(plusScore, transform.position, Quaternion.identity) as Transform;
        plusScoreInst.GetComponent<plusAnimation>().text = enemyCost.ToString();

        GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().wavesEnemies[enemyId].enemiesOnScene--;
        GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().CheckWavesEnd();
        
        GameObject.FindGameObjectWithTag("CameraParent").GetComponent<BuildTowersGUI>().myMoney += enemyCost;
        Destroy(this.gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        
        /*if (other.tag == "TowerBullet")
        {
            print("Opa " + other.tag);
            if (other.GetComponent<TowerBullet>().target == this.transform)
            {
                Destroy(other.gameObject);
            }
        }*/
    }
    void OnGUI()
    {
        Vector2 targetPos;
        targetPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up*8);
        if (curHealth > 0)
        {
            GUI.depth = 5;
            GUI.Box(new Rect(targetPos.x - 52, Screen.height - targetPos.y, 104, 5), "");
            GUI.Box(new Rect(targetPos.x - 50, Screen.height - targetPos.y + 1, curHealth/maxHealth*100, 3), "", healthStyle);
        }
    }

}
