using UnityEngine;
using System.Collections;

[System.Serializable]
public class TowerProperty
{
    public string towerName;
    public Transform towerPrefab;
    public Texture towerButtonTexture;
    public float damageRange = 15.0f;
    public float damage = 25.0f;
    public float attackSpeed = 1.0f;
    public float bulletSpeed = 100.0f;
    public float rotationSpeed = 10.0f;
    public int towerCost = 30;
    public Transform bullet;

}

public class BuildTowersGUI : MonoBehaviour {
    public TowerProperty[] towers;

    public bool buildStatus = false;
    private bool isShiftPressed = false;

    public int myMoney = 100;
    private int maxLevelHealth = 100;
    public int curLevelHealth = 100;

    public GUIStyle informationStyle;
    public GUIStyle levelHealthStyle;
    public GUIStyle towerButtonStyle;
    public GUIStyle levelHealthTextStyle;

    public Transform miniMap;

    public byte curBuildTowerId = 0;

    private Transform startWaveObject;
	// Use this for initialization
	void Start () {
        curLevelHealth = maxLevelHealth;
        startWaveObject = GameObject.FindGameObjectWithTag("Start").transform;
	}
	// Update is called once per frame
	void Update () {
        if (!GetComponent<UnitSelection>().cursorOnGUI)
        {
            if (buildStatus)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layerMask = 1 << 9;
                if (Physics.Raycast(ray, out hit, 400.0f, layerMask))
                {
                    if (hit.transform.tag == "BuildZone")
                    {
                        hit.transform.GetComponent<BuildZone>().OnMouseEnter();
                    }
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    if (myMoney >= towers[0].towerCost)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        layerMask = 1 << 9;
                        if (Physics.Raycast(ray, out hit, 400.0f, layerMask))
                        {
                            if (hit.transform.tag == "BuildZone" && !hit.transform.gameObject.GetComponent<BuildZone>().isBuilded)
                            {
                                GameObject go = Instantiate(towers[curBuildTowerId].towerPrefab.gameObject, hit.transform.gameObject.GetComponent<BuildZone>().buildPosition, Quaternion.identity) as GameObject;
                                Transform forProperties = go.transform.GetChild(1);

                                forProperties.GetComponent<Tower>().attackSpeed = towers[curBuildTowerId].attackSpeed;
                                forProperties.GetComponent<Tower>().bullet = towers[curBuildTowerId].bullet;
                                forProperties.GetComponent<Tower>().bulletSpeed = towers[curBuildTowerId].bulletSpeed;
                                forProperties.GetComponent<Tower>().damage = towers[curBuildTowerId].damage;
                                forProperties.GetComponent<Tower>().damageRange = towers[curBuildTowerId].damageRange;
                                forProperties.GetComponent<Tower>().rotationSpeed = towers[curBuildTowerId].rotationSpeed;

                                hit.transform.gameObject.GetComponent<BuildZone>().structure = go.transform;
                                hit.transform.gameObject.GetComponent<BuildZone>().SetBuilded(true);

                                myMoney -= towers[curBuildTowerId].towerCost;
                            }
                        }
                        if (!isShiftPressed)
                        {
                            buildStatus = false;
                            CutoutBacklight();
                        }
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buildStatus = false;
            CutoutBacklight();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isShiftPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isShiftPressed = false;
        }
        
	}
    void OnGUI()
    {
        if (GUI.Button(new Rect(310, Screen.height - 12 - 64, 64, 64), towers[0].towerButtonTexture, towerButtonStyle) && !buildStatus)
        {
            if (myMoney >= towers[0].towerCost)
            {
                StartCoroutine(SetBuildStatus());
            }
            
        }

        GUI.Box(new Rect(10, 10, 75, 20), "Гроші:", informationStyle);
        GUI.Box(new Rect(90, 10, 100, 20), myMoney.ToString(), informationStyle);
        GUI.Box(new Rect(10, 40, 75, 20), "Хвиля:", informationStyle);
        GUI.Box(new Rect(90, 40, 100, 20), (startWaveObject.GetComponent<WaveManipulator>().curWave + 1).ToString(), informationStyle);

        GUI.Box(new Rect(Screen.width / 2 - 180 / 2, 10, 180, 20), GameObject.FindGameObjectWithTag("Start").GetComponent<WaveManipulator>().guiText, informationStyle);
        GUI.Box(new Rect(Screen.width / 2 - 180 / 2, 40, 180, 20), Mathf.Round(startWaveObject.GetComponent<WaveManipulator>().timeToNextWave).ToString(), informationStyle);

        GUI.Box(new Rect(Screen.width - 190, 10, 180, 20), "Залишилось життів:", informationStyle);
        GUI.Box(new Rect(Screen.width - 190, 40, 180, 20), "");
        if (curLevelHealth > 0)
        {
            GUI.Box(new Rect(Screen.width - 188, 41, ((float)curLevelHealth / (float)maxLevelHealth) * (float)176, 18), "", levelHealthStyle);
            
        }
        GUI.Box(new Rect(Screen.width - 190, 40, 180, 20), curLevelHealth.ToString(), levelHealthTextStyle);
    }
    public void ApplyEnemyDamage()
    {
        if(curLevelHealth > 0)
            curLevelHealth--;
    }
    IEnumerator SetBuildStatus()
    {
        yield return new WaitForSeconds(0);        
        buildStatus = true;
        CutoutBacklight();
    }

    //Рубильник который будет отвечает за включение или отключение подсветки хайграундов на которых можно строить
    void CutoutBacklight()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("BuildZone");
        foreach (GameObject bl in gos)
        {
            bl.GetComponent<BuildZone>().backlight.gameObject.SetActive(buildStatus);
            //bl.GetComponent<BuildZone>().backlight.GetComponent<MeshDeformation>().Invoke("Calculate", 0.5f);
        }
    }
}
