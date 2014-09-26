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

    public string deathAnimation;

    public float explosionForce = 10000.0f;
    private bool startDeath = false;
	// Use this for initialization
	void Start () {
        curHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ApplyDamage(float damage, GameObject tower, int killerId)
    {
        if (!startDeath)
        {
            if (damage >= curHealth && !startDeath)
            {
                KillEnemy(killerId);
                
            }
            else
            {
                curHealth -= damage;
            }
        }
    }

    void KillEnemy(int killerId)
    {
        //Убиваю юнита
        curHealth = 0;
        startDeath = true;

        //создаю частицы смерти и убитое тело если они существуют
        Transform particle = null;
        if(destroyParticle)
            particle = Network.Instantiate(destroyParticle, transform.position, Quaternion.identity,5) as Transform;

        Transform go = null;
        if (destroyedBody)
        {
            go = Network.Instantiate(destroyedBody, transform.position, transform.rotation, 5) as Transform;
            if(go.animation)
                go.animation.Play(deathAnimation);
        }

        //отключаю скрипт движения
        if(transform.GetComponent<MineBotAI>())
            transform.GetComponent<MineBotAI>().enabled = false;
        else
            transform.GetComponent<BotAI>().enabled = false;
        //transform.GetChild(0).gameObject.SetActive(false);

        //Создаю удар для разрушенного тела
        if (go != null)
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

        //Херня показывающая сколько денег получил игрок
        Transform plusScoreInst = Instantiate(plusScore, transform.position, Quaternion.identity) as Transform;
        plusScoreInst.GetComponent<plusAnimation>().text = enemyCost.ToString();

        GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().wavesEnemies[enemyId].enemiesOnScene--;
        GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().CheckWavesEnd();


        GameObject.FindGameObjectWithTag("CameraParent").GetComponent<BuildTowersGUI>().myMoney += enemyCost;

        DestroyMe();
    }
    void DestroyMe()
    {
        Network.Destroy(this.gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        
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
