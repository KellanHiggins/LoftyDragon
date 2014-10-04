using UnityEngine;
using System.Collections;

public class CloudGenerator : MonoBehaviour {

	[SerializeField]
	private int cloudNumber = 20;

//	[SerializeField]
	private int cloudGenPercentage = 1;

	[SerializeField]
	private GameObject cloudNewLocation;

	[SerializeField]
	private GameObject cloudPrefab;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Random.Range(0, 100) < cloudGenPercentage)
		{
			CreateCloud();
		}
	}

	private void CreateCloud()
	{
		if(cloudNewLocation != null)
		{
			// find a random point along the cloud generator
			Debug.Log(cloudNewLocation.collider2D.bounds.min.y);

			float randomRange = Random.Range(cloudNewLocation.collider2D.bounds.min.y, cloudNewLocation.collider2D.bounds.max.y);

			Vector3 newCloudPos = new Vector3(cloudNewLocation.transform.position.x, cloudNewLocation.transform.position.y + randomRange, transform.position.z);
			GameObject newCloud = GameObject.Instantiate(cloudPrefab, newCloudPos, Quaternion.identity) as GameObject;
			newCloud.transform.parent = this.gameObject.transform;
			newCloud.transform.localScale = new Vector3(1,1,1);
			// generate a cloud into that item

			// DONE
		}
		else
		{
			Debug.Log("Cloud generator is not assigned");
		}
	}
}
