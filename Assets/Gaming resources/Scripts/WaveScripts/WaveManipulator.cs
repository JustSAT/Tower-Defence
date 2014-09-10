using UnityEngine;
using System.Collections;

[System.Serializable]
public class WaveEnemy
{
    public string enemyName;
    public Transform enemyPrefab;
    public float enemyHealth;
    public int enemyCost;
    public Transform enemyDestroyParticle;
    public Transform enemyDestroyedBody;
    public float waveTime;
    public float timeToNextWave;
    public int waveLength;
    public int enemiesOnScene = 0;
    public bool isWaveStarted = false;
    public bool isWaveEnded = false;
    public float spawnSpeed = 1.0f;
}

public class WaveManipulator : MonoBehaviour {

    public byte curWave = 0;
    public byte wavesCount = 3;
    public WaveEnemy[] wavesEnemies;

    public float timeToNextWave = 30.0f;

    public string guiText = "Time to start Wave:";

    public int startMoney = 100;
    private int curBotSpawned = 0;
	// Use this for initialization
	void Start () {
        GameObject.FindGameObjectWithTag("CameraParent").GetComponent<BuildTowersGUI>().myMoney = startMoney;
	}
	
	// Update is called once per frame
	void Update () {
        if (timeToNextWave > 0.0f)
        {
            guiText = "Часу до старту хвилі:";
            timeToNextWave -= Time.deltaTime;
        }
        else if (timeToNextWave < 0.0f)
        {
            timeToNextWave = 0.0f;
            guiText = "Хвиля вийшла!";
            if(curWave < wavesCount)
                StartWave();
        }
	}

    void StartWave()
    {
        wavesEnemies[curWave].isWaveStarted = true;
        StartCoroutine(StartSpawn(wavesEnemies[curWave].spawnSpeed));

    }

    public void CheckWavesEnd()
    {
        if (curWave < wavesCount-1)
        {
            for (int i = 0; i <= curWave; i++)
            {
                WaveEnemy we = wavesEnemies[i];
                if (we.isWaveEnded == false && we.isWaveStarted)
                {
                    if (we.enemiesOnScene <= 0 && curBotSpawned <= wavesEnemies[curWave].waveLength)
                    {
                        we.isWaveEnded = true;
                        timeToNextWave = wavesEnemies[curWave].timeToNextWave;
                        curWave++;
                        print("Wave[" + curWave + "] was ended. Last enemy is dead.");
                        curBotSpawned = 0;
                    }
                }
            }
        }
    }

    IEnumerator StartSpawn(float delay)
    {

        yield return new WaitForSeconds(delay);
        if (curBotSpawned < wavesEnemies[curWave].waveLength)
        {
            Transform go = Instantiate(wavesEnemies[curWave].enemyPrefab, transform.position + Vector3.up, Quaternion.identity) as Transform;
            EnemyUnit eu = go.GetComponent<EnemyUnit>();
            eu.maxHealth = wavesEnemies[curWave].enemyHealth;
            eu.curHealth = eu.maxHealth;
            eu.enemyCost = wavesEnemies[curWave].enemyCost;
            eu.enemyId = curWave;
            if (wavesEnemies[curWave].enemyDestroyedBody)
                eu.destroyedBody = wavesEnemies[curWave].enemyDestroyedBody;
            if (wavesEnemies[curWave].enemyDestroyParticle)
                eu.destroyParticle = wavesEnemies[curWave].enemyDestroyParticle;

            curBotSpawned++;
            wavesEnemies[curWave].enemiesOnScene++;
            if (go.GetComponent<MineBotAI>())
                go.GetComponent<MineBotAI>().target = GameObject.FindGameObjectWithTag("Finish").transform;
            if (go.GetComponent<BotAI>())
                go.GetComponent<BotAI>().target = GameObject.FindGameObjectWithTag("Finish").transform;

            StartCoroutine(StartSpawn(wavesEnemies[curWave].spawnSpeed));
        }
    }

}
