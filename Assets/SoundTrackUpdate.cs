using UnityEngine;
using System.Collections;

public class SoundTrackUpdate : MonoBehaviour {

	private Camera mainCamera;

	// Use this for initialization
	void Start () 
	{
		mainCamera = GameObject.FindObjectOfType<Camera>();
		DontDestroyOnLoad(this);
	}


	
	// Update is called once per frame
	void Update () 
	{
		if(mainCamera == null)
		{
			mainCamera = GameObject.FindObjectOfType<Camera>();
		}

		transform.position = mainCamera.transform.position;
	}
}
