using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	[SerializeField]
	private GameObject creator;

	// Use this for initialization
	void Start () 
	{
		creator = GameObject.Find("GameObjectPoolCreator");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "GameObjectPoolCollector")
		{
			if (creator)
			{
				Debug.Log("position" + creator.transform.position.y);
				float y = Random.Range((creator.transform.position.y - 80/2) - this.renderer.bounds.size.y, 80 + creator.transform.position.y);
				this.transform.position = new Vector2 (creator.transform.position.x + this.renderer.bounds.size.x, y);
			}
		}
	}
}
