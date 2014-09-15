using UnityEngine;
using System.Collections;

public class LobbyMainSong : MonoBehaviour {

    private bool isMainSongPlaing = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        isMainSongPlaing = GUI.Toggle(new Rect(Screen.width - 100, 10, 100, 20), isMainSongPlaing, "Music");
        transform.audio.mute = !isMainSongPlaing;
    }
}
