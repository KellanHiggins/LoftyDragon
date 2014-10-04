using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveDragon : MonoBehaviour 
{
	public float breath {get; set;}
	public float flapSpeed = 0.5f;
	public float moveSpeed = 3f;
	private bool path = false;
	private bool go = false;
	private GameObject currentPath;
	private float currentY = 0;
	private bool jetStreamStart = false;

	public float glidingGravity = -1f;

	public float gravity;
	private InputManager.IdealStateEnum breathCheck;
	private string breathState = "Exhale";

	[SerializeField]
	private GameObject jetStream;

	[SerializeField]
	private InputManager inputManager;

	void Start () 
	{
		//Start Character moving forward
		this.rigidbody2D.velocity = new Vector2(moveSpeed, 0);
	}

	void CheckToMove()
	{
		if (go == true)
		{
			if ((breath > -1)&&(breath < 1))
			{
				BoxCollider2D box = currentPath.GetComponent("BoxCollider2D")as BoxCollider2D;
				currentY = this.transform.position.y;
				if ((this.transform.position.y > box.bounds.center.y)||(this.transform.position.y < box.bounds.center.y))
				{
					MoveMe();
				}
				else if (this.transform.position.y == box.bounds.center.y+1)
				{
					Debug.Log("hit");
					go = false;
				}
			}
		}
	}

	void MoveMe()
	{
		BoxCollider2D box = currentPath.GetComponent("BoxCollider2D")as BoxCollider2D;
		this.transform.position = Vector3.Lerp(new Vector3(this.transform.position.x, currentY, 0), new Vector3(this.transform.position.x,box.bounds.center.y,0),Time.deltaTime*5);
		Debug.Log(this.transform.position.y);
		if ((this.transform.position.y < box.bounds.center.y+1)&&(this.transform.position.y > box.bounds.center.y-1))
		{
			this.transform.position = new Vector2(this.transform.position.x, box.bounds.center.y);
			go = false;
		}
	}

	void Update () 
	{
		if (inputManager.BreathingStatus == enumStatus.Exhale)
		{
			breathState = "Exhale";
		}
		else if (inputManager.BreathingStatus == enumStatus.Inhale)
		{
			breathState = "Inhale";
		}
		//When exhaling, move in paths
		if(breathState == "Exhale")
		{
			breath = inputManager.GetFlyingControl(); 
			//Checks if you are in the path
			if ((path)&&(go == false))
			{
				this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, (breath*10));
				if ((breath > -1)&&(breath < 1))
				{
					if(go == false)
					currentPath.transform.position = new Vector3 (currentPath.transform.position.x, this.transform.position.y, 0);
					this.rigidbody2D.velocity = new Vector2(moveSpeed, this.rigidbody2D.velocity.y + (flapSpeed*2));
					jetStream.renderer.material.color = Color.Lerp(jetStream.renderer.material.color,new Color(1f,1f,1f,1f),Time.deltaTime*3);
					if (jetStreamStart == false)
					{
						jetStream.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
						//jetStream.renderer.material.color = new Color(1f,1f,1f,1f);
						jetStreamStart = true;
					}
				}
				else
				{
					this.rigidbody2D.velocity = new Vector2(0, this.rigidbody2D.velocity.y);
				}

			}
			//If not in path
			else
			{
				go = true;
				CheckToMove();
				//If you breath properly
				if ((breath > -1)&&(breath < 1))
				{
				}
				//If you aren't breathing properly don't move higher or lower
				else
				{
					this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, 0);
				}
			}
		}

		//When in Inhale, Glide
		else if (breathState == "Inhale")
		{
			currentPath.transform.position = new Vector3 (currentPath.transform.position.x, this.transform.position.y, 0);
			this.rigidbody2D.velocity = new Vector2(moveSpeed, -flapSpeed);
			jetStreamStart = false;
			jetStream.renderer.material.color = Color.Lerp(jetStream.renderer.material.color,new Color(1f,1f,1f,0f),Time.deltaTime);

		}



	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "path")
		{
			path = true;
			currentPath = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "path")
		{
			path = false;
			currentPath = other.gameObject;
		}
	}

}
