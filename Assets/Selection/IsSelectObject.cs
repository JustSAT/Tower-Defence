using UnityEngine;
using System.Collections;

public class IsSelectObject : MonoBehaviour
{
    
    public bool isSelect = false;
    public GameObject selObj;
	// Use this for initialization
	void Start () {
        selObj.SetActive(false);
        if (transform.tag == "Unit")
        {
            selObj.renderer.material.color = new Color(0, 255, 0);
        }
        if (transform.tag == "Enemy")
        {
            selObj.renderer.material.color = new Color(255, 0, 0);
        }
        
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void SetAndPlay()
    {
        if (isSelect)
        {
            selObj.SetActive(true);
            selObj.GetComponent<Animation>().Play("selAnim");
        }
        else
        {
            selObj.SetActive(false);
        }
    }

    public IEnumerator JustPlay()
    {
        selObj.SetActive(true);
        selObj.GetComponent<Animation>().Play("selAnim");
        yield return new WaitForSeconds(0.2f);
        selObj.SetActive(false);
    }
}
