using UnityEngine;
using System.Collections;

public class TestSpawn : MonoBehaviour {
    public Transform spawnBot;
    bool spawnWave = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    IEnumerator InfinitySpawn(float delay)
    {

        yield return new WaitForSeconds(delay);
        if (spawnWave)
        {
            Transform go = Instantiate(spawnBot, transform.position + Vector3.up, Quaternion.identity) as Transform;
            go.GetComponent<MineBotAI>().target = GameObject.FindGameObjectWithTag("Finish").transform;

            StartCoroutine(InfinitySpawn(1));
        }
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 280, 100, 35), "Spawn Wave"))
        {
            if (!spawnWave)
            {
                StartCoroutine(InfinitySpawn(1));
                spawnWave = true;
            }
            else
                spawnWave = false;
        }
    }
}
