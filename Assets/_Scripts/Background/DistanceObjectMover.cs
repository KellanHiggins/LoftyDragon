using UnityEngine;
using System.Collections;

public class DistanceObjectMover : MonoBehaviour {

	[SerializeField]
	private GameObject player;

	[SerializeField]
	[Range(-100,100)]
	private int movementXModPercentage = 50;

	[SerializeField]
	[Range(-100,100)]
	private int movementYModPercentage = 50;

	[SerializeField]
	private int renderLayer;

	private float movementXMod = 0.5f;
	private float movementYMod = 0f;

	// Use this for initialization
	void Start () 
	{
		if(player == null)
		{
			Debug.LogError("Player can't be null, drag Dragon into this space");
		}
		movementXMod = movementXModPercentage / 100f;
		movementYMod = movementYModPercentage / 100f;

		SpriteRenderer[] allSprites = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
		for(int i = 0; i < allSprites.Length; i++)
		{
			allSprites[i].sortingOrder = renderLayer;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		this.gameObject.rigidbody2D.velocity = new Vector2(player.rigidbody2D.velocity.x * -1 * movementXMod,
		                                                   player.rigidbody2D.velocity.y * movementYMod);
		movementXMod = movementXModPercentage / 100f;
		movementYMod = movementYModPercentage / 100f;
	}
}
