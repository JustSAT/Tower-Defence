using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    
    private int DistanceToMove = 3;
    private float cameraMoveSpeed = 100;
    private float cameraMoveSpeedFromButtons = 100;
    private int borderDistance = 75;
    private float mouseWheelZoomSpeed = 200.0f;
    private float mouseAutoScrollSpeed = 10.0f;
    private float distanceToGround = 0.0f;

    private float minDistToGround = 25;
    private float maxDistToGround = 80;

    [System.Serializable]
    public class MapBorder
    {
        public int leftBorder = 0;
        public int botBorder = 0;
        public int rightBorder = 0;
        public int topBorder = 0;

        public MapBorder(int left, int bot, int right, int top)
        {
            leftBorder = left;
            botBorder = bot;
            rightBorder = right;
            topBorder = top;
        }
    }
    public MapBorder mapBorder = new MapBorder(0, 0, 250, 250);

	// Use this for initialization
	void Start () {
        
        //DistanceToMove = Screen.width / 14;
        transform.position = new Vector3(transform.position.x, 50, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        distanceToGround = Vector3.Distance(transform.position, hit.point);
        //cameraMoveSpeed = 0.0110769f * distanceToGround;
        if (!Input.GetButton("Fire1"))
        {
            //ƒвижение камеры на приближение к бортам екрана
            if (transform.position.x < mapBorder.rightBorder && Input.mousePosition.x > Screen.width - DistanceToMove) //¬право
            {
                transform.Translate(Time.deltaTime * cameraMoveSpeed, 0, 0);
            }
            if (transform.position.z < mapBorder.topBorder && Input.mousePosition.y > Screen.height - DistanceToMove) // ¬верх
            {
                transform.Translate(0, 0, Time.deltaTime * cameraMoveSpeed);
            }
            if (transform.position.x > mapBorder.leftBorder && Input.mousePosition.x < DistanceToMove) //¬лево
            {
                transform.Translate(-Time.deltaTime * cameraMoveSpeed, 0, 0);
            }
            if (transform.position.z > mapBorder.botBorder && Input.mousePosition.y < DistanceToMove) //¬низ
            {
                transform.Translate(0, 0, -Time.deltaTime * cameraMoveSpeed);
            }

            //ƒвижение камеры при нажатии клавиш
            if (transform.position.x < mapBorder.rightBorder && Input.GetKey(KeyCode.RightArrow) && !Physics.Raycast(transform.position, Vector3.right, out hit, 100)) //¬право
            {
                transform.Translate(Time.deltaTime * cameraMoveSpeedFromButtons, 0, 0);
            }
            if (transform.position.z < mapBorder.topBorder && Input.GetKey(KeyCode.UpArrow) && !Physics.Raycast(transform.position, Vector3.forward, out hit, 100)) // ¬верх
            {
                transform.Translate(0, 0, Time.deltaTime * cameraMoveSpeedFromButtons);
            }
            if (transform.position.x > mapBorder.leftBorder && Input.GetKey(KeyCode.LeftArrow) && !Physics.Raycast(transform.position, Vector3.left, out hit, 100)) //¬лево
            {
                transform.Translate(-Time.deltaTime * cameraMoveSpeedFromButtons, 0, 0);
            }
            if (transform.position.z > mapBorder.botBorder && Input.GetKey(KeyCode.DownArrow) && !Physics.Raycast(transform.position, Vector3.back, out hit, 100)) //¬низ
            {
                transform.Translate(0, 0, -Time.deltaTime * cameraMoveSpeedFromButtons);
            }

            //«ум
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Physics.Raycast(transform.position, Vector3.down + new Vector3(0, 0.73f, 0), out hit);
                distanceToGround = Vector3.Distance(transform.position, hit.point);
                if (distanceToGround > minDistToGround + 5 && distanceToGround < maxDistToGround - 5)
                    transform.Translate(0, -Input.GetAxis("Mouse ScrollWheel") * 10 * (mouseWheelZoomSpeed * Time.deltaTime), 0);
                if (distanceToGround < minDistToGround + 5 && Input.GetAxis("Mouse ScrollWheel") < 0)
                    transform.Translate(0, -Input.GetAxis("Mouse ScrollWheel") * 10 * (mouseWheelZoomSpeed * Time.deltaTime), 0);
                if (distanceToGround > maxDistToGround - 5 && Input.GetAxis("Mouse ScrollWheel") > 0)
                    transform.Translate(0, -Input.GetAxis("Mouse ScrollWheel") * 10 * (mouseWheelZoomSpeed * Time.deltaTime), 0);
            }
        }
        if (distanceToGround < minDistToGround)
        {
                StartCoroutine(ChangeDist(1));
        }
	}

    IEnumerator ChangeDist(byte a)
    {
        transform.Translate(0, Time.deltaTime * a * mouseAutoScrollSpeed, 0);
        yield return new WaitForSeconds(1.0f);
    }
    public void SetCameraByMinimap(Vector3 point)
    {
        float centerDiff = 10.0f;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            centerDiff = hit.point.z - transform.position.z;
        }
        transform.position = new Vector3(point.x, transform.position.y, point.z - centerDiff);
    }
}
