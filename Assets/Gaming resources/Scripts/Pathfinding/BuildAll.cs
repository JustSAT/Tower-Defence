using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BuildAll : MonoBehaviour {
    public Transform prefab2;   
    public Transform prefab;                    //Префаб нода.
    public int step = 5;                        //Шаг персонажа. Радиус и т.д.
    public Transform startPositionObject;       //Объект с которого берется стартовая позиция, сделано для удобства.
    public Vector3 nullPosition;

    Vector3 startPosition;                      //Изначальная позиция нашего персонажа. Из нее строиться все карта
    Vector3 plusHeightVector;                   //Промежуточный вектор. Используется при раставлении кубов на чуть большую высоту чем точка соприкосновения с поверхностью
    public Queue myQueue = new Queue();                //Очередь раставленных кубов в потсроении карты проходимости
    public int[,] field;                        //Матрица
    int sizex, sizez;                           //Размеры матрицы исходя из ландшафта
    float angle = 0.5f;                   //Угол на который может подняться персонаж

    List<List<GameObject>> NodePs;

    public static int pathCellId = 9996;
    public static int endCellId = 9997;
    public static int emptyCellId = 9998;
    public static int wallCellId = 9999;
    public static int startCellId = 0;
    
    string ss1 = "";
    string ss2 = "";

    // For tests
    Vector3 s1;
    Vector3 s2;
    Vector3 s3;
    Vector3 s4;
    Vector3 s5;
    Vector3 s6;
    Vector3 s7;
    Vector3 s8;
    RaycastHit hit;
    Transform clone;
    Color green;
	// Use this for initialization
	void Start () {
        angle = angle / step;
        s1 = new Vector3(1, angle, 0);
        s2 = new Vector3(0, angle, 1);
        s3 = new Vector3(-1, angle, 0);
        s4 = new Vector3(0, angle, -1);
        s5 = new Vector3(1, angle, 1);
        s6 = new Vector3(1, angle, -1);
        s7 = new Vector3(-1, angle, 1);
        s8 = new Vector3(-1, angle, -1);

        NodePs = new List<List<GameObject>>();
        NodePs.Add(new List<GameObject>());
        plusHeightVector = new Vector3(0, step / 2, 0);
        prefab.localScale = new Vector3(step, step + step / 3, step);
        startPosition = startPositionObject.transform.position;

        field = new int[(int)(Terrain.activeTerrain.terrainData.size.x / step)*2, (int)(Terrain.activeTerrain.terrainData.size.z / step)*2];
        sizex = (int)(Terrain.activeTerrain.terrainData.size.x / step) * 2;
        sizez = (int)(Terrain.activeTerrain.terrainData.size.z / step) * 2;
	}
	
	// Update is called once per frame
	void Update () {
        /*if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (NodePs[Trunc(hit.point.x / step)][Trunc(hit.point.z / step)] != null)
                {
                    NodePs[Trunc(hit.point.x / step)][Trunc(hit.point.z / step)].renderer.material.color = Color.red;
                    BuildPath(Trunc(startPositionObject.transform.position.x / step) + 1, Trunc(startPositionObject.transform.position.z / step) + 1, Trunc(hit.point.x / step) + 1, Trunc(hit.point.z / step) + 1);
                }
            }
        }*/
	}

    public int Trunc(float x)
    {
        int r;
        float a = x;
        x = Mathf.Abs(x - Mathf.Round(x));
        r = (int)(a - x);
        return r;
    }
    private Vector2 scrollViewVector = Vector2.zero;
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Build"))
        {
            StartCoroutine(SetPoints());
            //SetPoints();
        }
        
    }

    IEnumerator SetPoints()
    {
        spStart:
        int distanceForRay = 100;
        if (!Physics.Raycast(startPosition, -Vector3.up, out hit, distanceForRay))
        {
            startPosition += new Vector3(0, distanceForRay, 0);
            goto spStart;
            
        }
        hit.point += plusHeightVector;
        green = Color.green;
        green.a = 0.2f;
        Transform sClone;
        clone = Instantiate(prefab2, hit.point, Quaternion.identity) as Transform;
        sClone = clone;
        CheckingSides(clone);
        nullPosition = startPosition;
        do
        {
            if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s1, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
            {
                if (Physics.Raycast(clone.transform.position + s1 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
                {
                    Vector3 hitpos = clone.transform.position + s1 * step;
                    Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform;
                    forQueue.transform.renderer.material.color = green;
                    myQueue.Enqueue(forQueue);
                    CheckingSides(forQueue);
                }
            }
            if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s2, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
            {
                if (Physics.Raycast(clone.transform.position + s2 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
                {
                    Vector3 hitpos = clone.transform.position + s2 * step;
                    Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform; ;
                    forQueue.transform.renderer.material.color = green;
                    myQueue.Enqueue(forQueue);
                    CheckingSides(forQueue);
                }
            }
            if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s3, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
            {
                if (Physics.Raycast(clone.transform.position + s3 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
                {
                    Vector3 hitpos = clone.transform.position + s3 * step;
                    Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform;
                    forQueue.transform.renderer.material.color = green;
                    myQueue.Enqueue(forQueue);
                    CheckingSides(forQueue);
                }
            }
            if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s4, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
            {
                if (Physics.Raycast(clone.transform.position + s4 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
                {
                    Vector3 hitpos = clone.transform.position + s4 * step;
                    Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform;
                    forQueue.transform.renderer.material.color = green;
                    myQueue.Enqueue(forQueue);
                    CheckingSides(forQueue);
                }
            }
            clone = myQueue.Dequeue() as Transform;
            if (clone.transform.position.x <= nullPosition.x && clone.transform.position.z <= nullPosition.z)
            {
                nullPosition = clone.transform.position;
            }
            
        } while (myQueue.Count != 0);
        yield return new WaitForSeconds(0);
        startPosition = new Vector3(nullPosition.x, 250, nullPosition.z);
        
        for (int i = 0; i < (Terrain.activeTerrain.terrainData.size.x / step); i++)
        {
            for (int j = 0; j < (Terrain.activeTerrain.terrainData.size.z / step); j++)
            {
                NodePs[i].Add(null);
                if (Physics.Raycast(startPosition, -Vector3.up, out hit, 300))
                {
                    if (hit.collider.tag == "EditorOnly")
                    {
                        NodePs[i][j] = hit.transform.gameObject;
                        
                    }
                }
                startPosition += new Vector3(0, 0, step);
                
            }
            NodePs.Add(new List<GameObject>());
            startPosition = new Vector3(startPosition.x + step, startPosition.y, nullPosition.z);
        }
        /*
        int ci = 0;
        int cj = 0;
        for (int i = 1; i < sizex-1; i+=2)
        {
            for (int j = 1; j < sizez-1; j+=2)
            {
                if (NodePs[ci][cj] != null)          //Ошибка индексации на 41 итерации второго цыкла
                {
                    
                    field[i, j] = 0;
                    GameObject obj = NodePs[ci][cj];
                    obj.GetComponent<NodeP>().i = ci;
                    obj.GetComponent<NodeP>().j = cj;

                    if (obj.GetComponent<NodeP>().canB)
                    {
                        field[i, j - 1] = emptyCellId;
                    }
                    else
                    {
                        field[i, j - 1] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canF)
                    {
                        field[i, j + 1] = emptyCellId;
                    }
                    else
                    {
                        field[i, j + 1] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canR)
                    {
                        field[i + 1, j] = emptyCellId;
                    }
                    else
                    {
                        field[i + 1, j] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canL)
                    {
                        field[i - 1, j] = emptyCellId;
                    }
                    else
                    {
                        field[i - 1, j] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canLB)
                    {
                        field[i - 1, j - 1] = emptyCellId;
                    }
                    else
                    {
                        field[i - 1, j - 1] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canRB)
                    {
                        field[i + 1, j - 1] = emptyCellId;
                    }
                    else
                    {
                        field[i + 1, j - 1] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canLF)
                    {
                        field[i - 1, j + 1] = emptyCellId;
                    }
                    else
                    {
                        field[i - 1, j + 1] = wallCellId;
                    }
                    if (obj.GetComponent<NodeP>().canRF)
                    {
                        field[i + 1, j + 1] = emptyCellId;
                    }
                    else
                    {
                        field[i + 1, j + 1] = wallCellId;
                    }
                }
                else
                {
                    field[i, j] = wallCellId;
                }
                cj++;
            }
            cj = 0;
            ci++;
        }*/
        sClone.GetComponent<NodeLink>().end = NodePs[(int)(Terrain.activeTerrain.terrainData.size.x / step)-1][(int)(Terrain.activeTerrain.terrainData.size.z / step)-1].transform;
    }
    public void SetNodePs(Transform clone)
    {
        
        if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s1, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
        {
            if (Physics.Raycast(clone.transform.position + s1 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
            {
                Vector3 hitpos = clone.transform.position + s1 * step;
                Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform;
                forQueue.transform.renderer.material.color = green;
                myQueue.Enqueue(forQueue);
                CheckingSides(forQueue);
            }
        }
        if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s2, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
        {
            if (Physics.Raycast(clone.transform.position + s2 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
            {
                Vector3 hitpos = clone.transform.position + s2 * step;
                Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform; ;
                forQueue.transform.renderer.material.color = green;
                myQueue.Enqueue(forQueue);
                CheckingSides(forQueue);
            }
        }
        if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s3, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
        {
            if (Physics.Raycast(clone.transform.position + s3 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
            {
                Vector3 hitpos = clone.transform.position + s3 * step;
                Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform;
                forQueue.transform.renderer.material.color = green;
                myQueue.Enqueue(forQueue);
                CheckingSides(forQueue);
            }
        }
        if (!Physics.Raycast(clone.transform.position + new Vector3(0, step / 3, 0), s4, out hit, step * 1.18f) && !Physics.Raycast(clone.transform.position, Vector3.up, out hit, step * 1.18f))
        {
            if (Physics.Raycast(clone.transform.position + s4 * step, Vector3.down, out hit, step) && hit.collider.tag != "EditorOnly")
            {
                Vector3 hitpos = clone.transform.position + s4 * step;
                Transform forQueue = Instantiate(prefab, new Vector3(hitpos.x, hit.point.y, hitpos.z) + plusHeightVector, Quaternion.identity) as Transform;
                forQueue.transform.renderer.material.color = green;
                myQueue.Enqueue(forQueue);
                CheckingSides(forQueue);
            }
        }
    }
    void CheckingSides(Transform obj)
    {
        RaycastHit hit;
        if (!Physics.Raycast(obj.transform.position, s1, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s1 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canR = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s2, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s2 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canF = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s3, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s3 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canL = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s4, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s4 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canB = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s5, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s5 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canRF = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s6, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s6 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canRB = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s7, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s7 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canLF = true;
            }
        }
        if (!Physics.Raycast(obj.transform.position, s8, out hit, step, 9) || hit.collider.tag == "EditorOnly")
        {
            if (Physics.Raycast(obj.transform.position + s8 * step, Vector3.down, out hit, step + 1))
            {
                obj.GetComponent<NodeP>().canLB = true;
            }
        }
    }
    void BuildPath(int spi, int spj, int epi, int epj)
    {
        int ni = startCellId;
        NodePs[Trunc(startPositionObject.transform.position.x / step)][Trunc(startPositionObject.transform.position.z / step)].renderer.material.color = Color.green;
        spi *= 2;
        spj *= 2;
        epi *= 2;
        epj *= 2;

        spi -= 1;
        spj -= 1;
        epi -= 1;
        epj -= 1;
        field[spi, spj] = emptyCellId;
        field[epi, epj] = endCellId;
        do
        {
            for (int i = 1; i < sizex - 1; i++)
            {
                for (int j = 1; j < sizez - 1; j++)
                {
                    if (field[i, j] == ni)
                    {
                        if (i + 1 < sizex)
                            if (field[i + 1, j] == emptyCellId)
                            {
                                field[i + 1, j] = ni + 1;
                            }
                        if (j - 1 >= 0)
                            if (field[i, j - 1] == emptyCellId)
                            {
                                field[i, j - 1] = ni + 1;
                            }
                        if (i - 1 >= 0)
                            if (field[i - 1, j] == emptyCellId)
                            {
                                field[i - 1, j] = ni + 1;
                            }
                        if (j + 1 < sizez)
                            if (field[i, j + 1] == emptyCellId)
                            {
                                field[i, j + 1] = ni + 1;
                            }
                    }
                }

            }
            ni++;
            if (ni > sizex * sizez + 100)
            {
                break;
            }
        } while (true);
        

    }
}
