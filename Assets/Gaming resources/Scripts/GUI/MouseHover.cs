using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MouseHover : MonoBehaviour {

    public bool mouseOverGUI = false;

    public List<Rect> guiRects;

	// Use this for initialization
    void Start()
    {
        guiRects = new List<Rect>();
        guiRects.Add(new Rect(310, Screen.height - 12 - 64, 64, 64));   //Построить тавер
        guiRects.Add(new Rect(400, Screen.height - 12 - 64, 64, 64));   //Апгрейд тавера
        guiRects.Add(new Rect(10, 10, 75, 20));     //Гроші
        guiRects.Add(new Rect(90, 10, 100, 20));    //Гроші    
        guiRects.Add(new Rect(10, 40, 75, 20));     //Хвиля
        guiRects.Add(new Rect(90, 40, 100, 20));    //Хвиля



        
        guiRects.Add(new Rect(Screen.width / 2 - 180 / 2, 10, 180, 20));
        guiRects.Add(new Rect(Screen.width / 2 - 180 / 2, 40, 180, 20));
        guiRects.Add(new Rect(Screen.width - 190, 10, 180, 20));
        guiRects.Add(new Rect(Screen.width - 190, 40, 180, 20));
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
    void FixedUpdate()
    {
        Debug.Log(Input.mousePosition);
        mouseOverGUI = false;
        for (int i = 0; i < guiRects.Count; i++)
        {
            if (guiRects[i].Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
            {
                mouseOverGUI = true;
            }
        }
    }
}
