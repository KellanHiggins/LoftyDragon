using UnityEngine;
using System.Collections;

public class MoveDragon : MonoBehaviour 
{

	public float flapSpeed = 1f;
	public float moveSpeed = 3f;

	public float flyingGravity = 0.5f;
	public float glidingGravity = -1f;

	public float gravity;



	// Use this for initialization
	void Start () 
	{
		this.rigidbody2D.velocity = new Vector2(moveSpeed, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.Space))
		{
			this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, this.rigidbody2D.velocity.y + flapSpeed);
		}
		else if(this.rigidbody2D.velocity.y > -10)
		{
			this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, this.rigidbody2D.velocity.y - this.flyingGravity);
		}

	}
}
