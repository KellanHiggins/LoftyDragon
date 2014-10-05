using UnityEngine;
using System.Collections;

public class enemyBehavior : MonoBehaviour {
	private bool moving = false;
	[SerializeField]
	private GameObject enemySpawn1;
	[SerializeField]
	private GameObject enemySpawn2;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (moving == false)
		{
			int rand = Random.Range(0,2);
			Debug.Log(rand);
			if (rand == 0)
			{
				this.transform.position = new Vector2(enemySpawn1.transform.position.x,enemySpawn1.transform.position.y);
				this.rigidbody2D.velocity = new Vector2(-5,0);
				moving = true;
			}
			else
			{
				this.transform.position = new Vector2(enemySpawn2.transform.position.x,enemySpawn2.transform.position.y);
				this.rigidbody2D.velocity = new Vector2(-5,0);
				moving = true;
			}
		}
	}
}
