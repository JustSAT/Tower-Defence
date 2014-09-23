using UnityEngine;
using System.Collections;

/*
 * Клас, що містить всю інформацію для створення хвиль монстрів.
 * enemyName - служить для відображення імені монстра в масиві.
 * enemyPrefab - префаб мостра
 * enemyHealth - кількість здоров'я
 * enemyCost - скільки дадуть грошей за вбивство
 * enemyDestroyParticle - частинки, що будуть створення після вбивста монстра
 * enemyDestroyedBody - об'єкт який будет створено після вбивста монстра. (можна не вказувати)
 * waveTime - скільки часу триває хвиля. Після закінчення часу стартує нова хвиля(не реалізовано)
 * timeToNextWave - часу до старту нової хвилі після закінчення цієї.
 * waveLEngth - скільки мострів містить хвиля.
 * (під поточною розуміється хвиля як елемент масиву)
 * enemiesOnScene - скільки мострів поточної хвилі на сцені
 * isWaveStarted - чи стартувала поточна хвиля
 * isWaveEnded - чи закінчилася поточна хвиля
 * spawnSpeed - швидкість, з якою спавняться монстри хвилі
 */

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

// WavePoint - клас, який містить дані: місце спавну монстрів; куди монстри йдуть.
[System.Serializable]
public class WavePoint
{
    public Transform startPoint;
    public Transform finishPoint;
}
// PlayerWavesPoints - WavePoint для кожного гравця.
[System.Serializable]
public class PlayersWavesPoints
{
    public WavePoint[] player = new WavePoint[4];
}

public class WaveManipulator : MonoBehaviour {

    public byte curWave = 0;
    public byte wavesCount = 3;

    public PlayersWavesPoints playersWavesPoints;
    public WaveEnemy[] wavesEnemies;

    public float timeToNextWave = 30.0f;

    public string guiText = "Time to start Wave:";

    public int startMoney = 100;
    private int curBotSpawned = 0;
    NetworkingLobby nl;
	// Use this for initialization
	void Start () {
        if (GameObject.FindGameObjectWithTag("Lobby"))
        {
            nl = GameObject.FindGameObjectWithTag("Lobby").GetComponent<NetworkingLobby>();
            for(int i = 0; i < 4; i++)
            {
                if (nl.connectedPlayers[i].isEmptySlot == false)
                {
                    nl.connectedPlayers[i].money = startMoney;
                }
            }
            GameObject.FindGameObjectWithTag("CameraParent").GetComponent<BuildTowersGUI>().myMoney = startMoney;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Network.isServer)
        {
            if (timeToNextWave > 0.0f)
            {
                guiText = "Часу до старту хвилі:";
                timeToNextWave -= Time.deltaTime;
                networkView.RPC("SyncData", RPCMode.Others, new object[] { timeToNextWave, int.Parse(curWave.ToString()), guiText });
            }
            else if (timeToNextWave < 0.0f)
            {
                timeToNextWave = 0.0f;
                guiText = "Хвиля вийшла!";
                if (curWave < wavesCount)
                    StartWave();
                networkView.RPC("SyncData", RPCMode.Others, new object[] { timeToNextWave, int.Parse(curWave.ToString()), guiText });
            }
        }
	}

    void StartWave()
    {
        if (Network.isServer)
        {
            wavesEnemies[curWave].isWaveStarted = true;
            for (int i = 0; i < 4; i++)
            {
                if (nl.connectedPlayers[i].isEmptySlot == false)
                {
                    StartCoroutine(StartSpawn(wavesEnemies[curWave].spawnSpeed, i));
                }
            }
        }
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

    IEnumerator StartSpawn(float delay, int playerId)
    {

        yield return new WaitForSeconds(delay);
        if (curBotSpawned < wavesEnemies[curWave].waveLength)
        {
            Transform go = Network.Instantiate(wavesEnemies[curWave].enemyPrefab, playersWavesPoints.player[playerId].startPoint.position, Quaternion.identity, 5)as Transform;
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
                go.GetComponent<MineBotAI>().target = playersWavesPoints.player[playerId].finishPoint;
            else if (go.GetComponent<BotAI>())
                go.GetComponent<BotAI>().target = playersWavesPoints.player[playerId].finishPoint;

            StartCoroutine(StartSpawn(wavesEnemies[curWave].spawnSpeed, playerId));
        }
    }
    [RPC]
    void SyncData(float time, int wave, string text)
    {
        if (Network.isClient)
        {
            timeToNextWave = time;
            curWave = byte.Parse(wave.ToString());
            guiText = text;
        }

    }

}
