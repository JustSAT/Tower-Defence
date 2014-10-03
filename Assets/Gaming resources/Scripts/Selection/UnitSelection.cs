using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSelection : MonoBehaviour {

    public List<GameObject> allUnits;               //Массив всех юнитов, которые есть на сцене
    public List<GameObject> selectedUnits;          //Массив выбраных юнитов

    public GameObject selectedEnemy;

    private Vector3 mouseButtonDownPosition;        //Переменные для хранения позиций нажатия кнопки мыши и ее оптускания
    private Vector3 mouseButtonUpPosition;

    public bool isMouseDrag = false;
    public bool isAnySelect = false;
    public bool isShiftPressed = false;
    public bool isEnemySelect = false;

    public bool cursorOnGUI = false;

    private bool missing = false;
    public Vector2 drawPositionS;


    public GUISkin skin;

    private Vector3[] myFourPoints;

    private BuildTowersGUI BTGUI;
	void Start () {
	    allUnits = new List<GameObject>();
        selectedUnits = new List<GameObject>();
        GameObject[] fUnits = GameObject.FindGameObjectsWithTag("Unit");    //Промежуточный массив для избегания невозможности приобразования типов
        foreach (GameObject go in fUnits)            //Для каждого ИгровогоОбъекта go в массиве fUnits. Каждый елемент массива поочереди помещается в переменную go
        {
            if (go.networkView.isMine)
            {
                allUnits.Add(go);
            }
        }
        BTGUI = GameObject.FindGameObjectWithTag("CameraParent").GetComponent<BuildTowersGUI>();

	}

    void Update()
    {
        if (Input.mousePosition.y < 200)
        {
            cursorOnGUI = true;
        }
        else
        {
            cursorOnGUI = false;
        }
        if (BTGUI.buildStatus == false && !cursorOnGUI)
        {
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!cursorOnGUI && !missing)
                {
                    OnMouseButtonDown(Input.mousePosition);
                    drawPositionS = Input.mousePosition;
                }
                else
                    missing = true;

            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (!cursorOnGUI && !missing)
                {
                    if (new Vector3(drawPositionS.x, drawPositionS.y, Input.mousePosition.z) != Input.mousePosition)
                        isMouseDrag = true;
                }
            }


            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                OnMouseButtonUp(Input.mousePosition);
                isMouseDrag = false;
                missing = false;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                isShiftPressed = true;
            }
            else
            {
                isShiftPressed = false;
            }

        }
        if (isAnySelect && Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelList();

            DeselectEnemy();
        }
    }
    void OnGUI()
    {
        if (isMouseDrag)
        {
            if (Vector2.Distance(drawPositionS, Input.mousePosition) > 20)
            {
                Vector3 p1 = drawPositionS;
                Vector3 p2 = Input.mousePosition;
                Vector3 temp;
                if (p1.x > p2.x)
                {
                    temp = p1;
                    p1 = new Vector3(p2.x, p1.y, p1.z);
                    p2 = new Vector3(temp.x, p2.y, p2.z);
                }
                if (p1.y < p2.y)
                {
                    temp = p1;
                    p1 = new Vector3(p1.x, p2.y, p2.z);
                    p2 = new Vector3(p2.x, temp.y, temp.z);
                }
                GUI.skin = skin;
                GUI.Box(new Rect(p1.x, Screen.height - p1.y, p2.x - p1.x, p1.y - p2.y), GUIContent.none);
            }
        }
    }
    //Узнаем координаты при нажатии кнопки
    public void OnMouseButtonDown(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (isAnySelect && !isShiftPressed)
                ClearSelList();
            if (isEnemySelect)
            {
                DeselectEnemy();
            }

            mouseButtonDownPosition = hit.point;
        }
    }
    //Узнаем координаты при отпускании кнопки
    public void OnMouseButtonUp(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mouseButtonUpPosition = hit.point;
              
            myFourPoints = new Vector3[4];
            Physics.Raycast(Camera.main.ScreenPointToRay(drawPositionS), out hit);
            myFourPoints[0] = hit.point;

            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            myFourPoints[1] = hit.point;

            Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, drawPositionS.y, 0)), out hit);
            myFourPoints[2] = hit.point;

            Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(drawPositionS.x, Input.mousePosition.y, 0)), out hit);
            myFourPoints[3] = hit.point;


            GetAllUnitsInArea();
        }
        if (Vector2.Distance(drawPositionS, Input.mousePosition) < 20)
        {
            if (hit.collider.tag == "Unit")
            {
                AddUnitToSelList(hit.transform.gameObject);
            }
            if (hit.collider.tag == "Enemy")
            {
                SelectEnemy(hit.transform.gameObject);
            }
        }
    }

    public bool Intersection(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2)
    {
        float v1, v2, v3, v4;

        v1 = (B2.x - B1.x) * (A1.z - B1.z) - (B2.z - B1.z) * (A1.x - B1.x);
        v2 = (B2.x - B1.x) * (A2.z - B1.z) - (B2.z - B1.z) * (A2.x - B1.x);
        v3 = (A2.x - A1.x) * (B1.z - A1.z) - (A2.z - A1.z) * (B1.x - A1.x);
        v4 = (A2.x - A1.x) * (B2.z - A1.z) - (A2.z - A1.z) * (B2.x - A1.x);

        return (v1*v2 < 0) && (v3*v4 < 0);
    }
    public void GetAllUnitsInArea()
    {
        Vector3 temp;
        if (myFourPoints[0].x > myFourPoints[1].x)
        {
            temp = myFourPoints[0];
            myFourPoints[0] = myFourPoints[1];
            myFourPoints[1] = temp;
            temp = myFourPoints[2];
            myFourPoints[2] = myFourPoints[3];
            myFourPoints[3] = temp;
        }
        if (myFourPoints[0].z < myFourPoints[1].z)
        {
            temp = myFourPoints[0];
            myFourPoints[0] = myFourPoints[3];
            myFourPoints[3] = temp;
            temp = myFourPoints[1];
            myFourPoints[1] = myFourPoints[2];
            myFourPoints[2] = temp;
        }


        foreach (GameObject unit in allUnits)
        {
            if (unit.transform.position.x > myFourPoints[0].x && unit.transform.position.x < myFourPoints[1].x && unit.transform.position.z < myFourPoints[0].z && unit.transform.position.z > myFourPoints[1].z)
            {
                AddUnitToSelList(unit);
            }
            else
            {
                if (Intersection(myFourPoints[3], myFourPoints[0], unit.transform.position, new Vector3(unit.transform.position.x - 10000, 0, unit.transform.position.z)) && Intersection(myFourPoints[2], myFourPoints[1], unit.transform.position, new Vector3(unit.transform.position.x+10000, 0, unit.transform.position.z)))
                {
                    AddUnitToSelList(unit);
                }
            }
        }
    }
    public void AddUnitToSelList(GameObject unit)
    {
        isAnySelect = true;
        if (!unit.GetComponent<IsSelectObject>().isSelect)
        {
            unit.GetComponent<IsSelectObject>().isSelect = true;
            unit.GetComponent<IsSelectObject>().SetAndPlay();
            selectedUnits.Add(unit);
        }
    }
    public void ClearSelList()
    {
        foreach (GameObject unit in selectedUnits)
        {
            unit.GetComponent<IsSelectObject>().isSelect = false;
            unit.GetComponent<IsSelectObject>().SetAndPlay();
        }

        selectedUnits.Clear();
        isAnySelect = false;
    }

    void SelectEnemy(GameObject enemy)
    {
        isEnemySelect = true;
        if (!enemy.GetComponent<IsSelectObject>().isSelect)
        {
            enemy.GetComponent<IsSelectObject>().isSelect = true;
            enemy.GetComponent<IsSelectObject>().SetAndPlay();
            selectedEnemy = enemy;
            
        }
    }
    void DeselectEnemy()
    {
        if (selectedEnemy)
        {
            selectedEnemy.GetComponent<IsSelectObject>().isSelect = false;
            selectedEnemy.GetComponent<IsSelectObject>().SetAndPlay();
        }
        isAnySelect = false;
    }
    
}
