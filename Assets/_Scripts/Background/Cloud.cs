using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "GameObjectPoolCollector")
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}
