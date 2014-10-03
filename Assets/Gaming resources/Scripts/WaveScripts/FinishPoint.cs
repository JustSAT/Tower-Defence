using UnityEngine;
using System.Collections;

public class FinishPoint : MonoBehaviour
{
    public AudioClip enemyDespawn;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            audio.PlayOneShot(enemyDespawn, 0.20f);
            GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().wavesEnemies[other.GetComponent<EnemyUnit>().enemyId].enemiesOnScene--;
            GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().CheckWavesEnd();
            GameObject.FindGameObjectWithTag("CameraParent").GetComponent<BuildTowersGUI>().ApplyEnemyDamage();
            Network.Destroy(other.transform.gameObject);
        }
    }
}
