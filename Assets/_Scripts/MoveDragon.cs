using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveDragon : MonoBehaviour 
{
	public SpriteRenderer DragonSprite;

//	private float dragonForwardSpeed;
//	private float stallForwardSpeed;

	private float flapUpSpeed = 3.5f;
	private float flapForwardSpeed = 20;
	private float glideDownSpeed = -2f;
	private float glideForwardSpeed = 8f;

	private float stallSpeed = 5f;
	private float dropSpeed = 5f;


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

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private GameObject woosh;

	[SerializeField]
	private GameObject flapSound;

	[SerializeField]
	private AudioClip[] flapSounds;

	[SerializeField]
	private AudioClip[] wooshSounds;

	private float maxHeight = 20;
	private float minHeight = 0;

	void Start () 
	{
		//Start Character moving forward
//		this.rigidbody2D.velocity = new Vector2(moveSpeed, 0);
		animator = GetComponentInChildren<Animator>();
		DragonSprite = gameObject.transform.FindChild("DragonSprite").GetComponent<SpriteRenderer>();
	}

//	void CheckToMove()
//	{
//		if (go == true)
//		{
//			if ((breath > -1)&&(breath < 1))
//			{
//				BoxCollider2D box = currentPath.GetComponent("BoxCollider2D")as BoxCollider2D;
//				currentY = this.transform.position.y;
////				if ((this.transform.position.y > box.bounds.center.y)||(this.transform.position.y < box.bounds.center.y))
////				{
////					MoveMe();
////				}
////				else if (this.transform.position.y == box.bounds.center.y+1)
////				{
////					Debug.Log("hit");
////					go = false;
////				}
//			}
//		}
//	}

	void MoveMe()
	{
		BoxCollider2D box = currentPath.GetComponent("BoxCollider2D")as BoxCollider2D;
		this.transform.position = Vector3.Lerp(new Vector3(this.transform.position.x, currentY, 0), new Vector3(this.transform.position.x,box.bounds.center.y,0),Time.deltaTime*5);
		if ((this.transform.position.y < box.bounds.center.y+1)&&(this.transform.position.y > box.bounds.center.y-1))
		{
			this.transform.position = new Vector2(this.transform.position.x, box.bounds.center.y);
			go = false;
		}
	}

	void Update()
	{
		float thisFrameFlight = inputManager.GetFlyingControl();

		if(inputManager.BreathingStatus == enumStatus.Inhale)
		{
			this.rigidbody2D.velocity = new Vector2(glideForwardSpeed, glideDownSpeed);
			this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Glide);
			this.gameObject.transform.position = this.gameObject.transform.position + DragonSprite.gameObject.transform.localPosition;
			DragonSprite.gameObject.transform.localPosition = new Vector2(0,0);
			jetStream.rigidbody2D.velocity = new Vector2(0f,0f);
			jetStreamStart = false;
			jetStream.renderer.material.color = Color.Lerp(jetStream.renderer.material.color,new Color(1f,1f,1f,0f),Time.deltaTime);
		}
		else if(inputManager.BreathingStatus == enumStatus.Exhale && inputManager.IdealState == InputManager.IdealStateEnum.KeepExhaling)
		{
			// spawns the jet stream if it isn't already there.
			jetStream.renderer.material.color = Color.Lerp(jetStream.renderer.material.color,new Color(1f,1f,1f,1f),Time.deltaTime*3);
			if (jetStreamStart == false)
			{
				jetStream.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + maxHeight / 2f, 0);
				int randWoosh = Random.Range(0,2);
				woosh.audio.clip = wooshSounds[randWoosh];
				woosh.audio.Play();
				//jetStream.renderer.material.color = new Color(1f,1f,1f,1f);
				jetStreamStart = true;
			}

			float normalizedStuff = (inputManager.GetFlyingControl() + 2) / 4;
			DragonSprite.gameObject.transform.localPosition = new Vector2(0, Mathf.Lerp(minHeight, maxHeight, normalizedStuff));

			if(thisFrameFlight > -1 && thisFrameFlight < 1)
			{
				// fly up and to the right
				this.rigidbody2D.velocity = new Vector2(flapForwardSpeed, flapUpSpeed);
				this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Flap);
				jetStream.rigidbody2D.velocity = new Vector2(0,0); // moves the jetstream along if you screw up
				if (flapSound.audio.isPlaying == false)
				{
					int randFlap = Random.Range(0,4);
					flapSound.audio.clip = flapSounds[randFlap];
					flapSound.audio.Play();
				}
			}
			else if(thisFrameFlight > 1) // stalling
			{
				this.rigidbody2D.velocity = new Vector2(dropSpeed, 0);
				this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Stall);
				jetStream.rigidbody2D.velocity = this.gameObject.rigidbody2D.velocity;
			}
			else if(thisFrameFlight < -1) // dropping
			{
				this.rigidbody2D.velocity = new Vector2(stallSpeed, 0);
				this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Drop);
				jetStream.rigidbody2D.velocity = this.gameObject.rigidbody2D.velocity;
			}
		}
	}



	void OldUpdate () 
	{
		// Update the animation

		// If you aren't breathing enough, drop,

		// If you are breathing too much, stall
//
//		float flightControl = inputManager.GetFlyingControl();
//
//		if(inputManager.BreathingStatus == enumStatus.Exhale)
//		{
//			if(flightControl > 1)
//			{
//				this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Stall);
//			}
//			else if(flightControl < -1)
//			{
//				this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Drop);
//			}
//			else if(flightControl > -1 && flightControl < 1)
//			{
//				this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Flap);
//			}
//		}
//		else if(inputManager.BreathingStatus == enumStatus.Inhale)
//		{
//			this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Glide);
//		}
//
//
//
//
//
//
//		//When exhaling, move in paths
//		if(inputManager.BreathingStatus == enumStatus.Exhale)
//		{
//			breath = inputManager.GetFlyingControl(); 
//			//Checks if you are in the path
//			if ((path)&&(go == false))
//			{
//				this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, (breath*10));
//				if ((breath > -1)&&(breath < 1))
//				{
//					if(go == false)
//						currentPath.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
//					this.rigidbody2D.velocity = new Vector2(moveSpeed, this.rigidbody2D.velocity.y + (flapSpeed*2f));
//					jetStream.renderer.material.color = Color.Lerp(jetStream.renderer.material.color,new Color(1f,1f,1f,1f),Time.deltaTime*3);
//					if (jetStreamStart == false)
//					{
//						jetStream.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
//						//jetStream.renderer.material.color = new Color(1f,1f,1f,1f);
//						jetStreamStart = true;
//					}
//
//				}
//				else
//				{
//					this.rigidbody2D.velocity = new Vector2(0, this.rigidbody2D.velocity.y*.7f);
//				}
//
//			}
//			//If not in path
//			else
//			{
//				go = true;
//				CheckToMove();
//
//				//If you breath properly
//				if ((breath > -1)&&(breath < 1))
//				{
//
//				}
//				//If you aren't breathing properly don't move higher or lower
//				else
//				{
//					this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, 0);
//				}
//			}
//		}
//
//		//When in Inhale, Glide
//		else if (inputManager.BreathingStatus == enumStatus.Inhale)
//		{
//			this.animator.SetInteger("FlightEnum", (int)FlightStatusEnum.Glide);
//			currentPath.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
//			this.rigidbody2D.velocity = new Vector2(moveSpeed, -flapSpeed);
//			jetStreamStart = false;
//			jetStream.renderer.material.color = Color.Lerp(jetStream.renderer.material.color,new Color(1f,1f,1f,0f),Time.deltaTime);
//
//		}
//


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
