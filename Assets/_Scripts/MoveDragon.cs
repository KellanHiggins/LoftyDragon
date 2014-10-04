using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveDragon : MonoBehaviour 
{
	public float breath {get; set;}
	public float flapSpeed = 0.5f;
	public float moveSpeed = 3f;
	private bool upDir = false;
	private bool path = false;
	private bool center = false;

	public float glidingGravity = -1f;

	public float gravity;

	private string breathState = "Exhale";

	void Start () 
	{
		//Start Character moving forward
		this.rigidbody2D.velocity = new Vector2(moveSpeed, 0);
	}

	void Update () 
	{
		//When exhaling, move in paths
		if(breathState == "Exhale")
		{
			//Checks if you are in the path
			if (path)
			{
			this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, (breath*10)+flapSpeed);
			}
			//If not in path
			else
			{
				//If you breath properly
				if ((breath > -1)&&(breath < 1))
				{
					//If you went below go up
					if (upDir == false)
					{
						this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, (breath*10)+flapSpeed);
					}
					//If you went up go down
					else if (center == false)
					{
						this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, -(breath*10)-flapSpeed);
					}
					else
					{
						this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x,0);
					}
				}
				//If you aren't breathing properly don't move higher or lower
				else
				{
					this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, 0);
				}
			}
		}

		//If you are in a path
		if (path)
		{
			//If breathing properly move forward properly
			if ((breath > -1)&&(breath < 1))
			{
				this.rigidbody2D.velocity = new Vector2(moveSpeed, this.rigidbody2D.velocity.y);
			}
			//Else if breathing too weak slow down
			else if (breath < -1)
			{
				this.rigidbody2D.velocity = new Vector2(moveSpeed/9, this.rigidbody2D.velocity.y);
			}
			//Else if breathing too strong slow down
			else if (breath > 1)
			{
				this.rigidbody2D.velocity = new Vector2(moveSpeed/9, this.rigidbody2D.velocity.y);
			}
		}

		//When in Inhale, Glide
		else if (breathState == "Inhale")
		{

		}

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "path")
		{
			path = true;
		}
		if (other.gameObject.tag == "center")
		{
			center = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "path")
		{
			path = false;
			if (breath < -1)
			{
				upDir = false;
			}
			else if (breath > 1)
			{
				upDir = true;
			}
		}
		if (other.gameObject.tag == "center")
		{
			center = false;
		}
	}

}
