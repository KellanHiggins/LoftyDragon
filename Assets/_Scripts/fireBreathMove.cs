using UnityEngine;
using System.Collections;

public class fireBreathMove : MonoBehaviour {
	private Vector2 initialPosition;

	// Use this for initialization
	void Start () {
		initialPosition = this.transform.position;
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void UpButton()
	{
			this.transform.position = initialPosition;
	}

	public void DownButton()
	{
		if(this.transform.position.y != initialPosition.y - 5)
			this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y - 5);
	}
}
