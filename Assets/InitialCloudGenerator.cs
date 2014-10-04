using UnityEngine;
using System.Collections;

/// <summary>
/// Generates all starting clouds
/// </summary>
public class InitialCloudGenerator : MonoBehaviour 
{
	public GameObject CloudDestroyer;
	public GameObject CloudGenerator;

	public GameObject cloudPrefab;

	public GameObject[] CloudLayerHolders;

	private int initialCloudMin = 5;
	private int initialCloudMax = 15;

	// Use this for initialization
	void Start () 
	{
		GenerateRandomClouds();
	}

	void GenerateRandomClouds ()
	{
		foreach(var holder in CloudLayerHolders)
		{
			int numClouds = Random.Range(initialCloudMin, initialCloudMax);

			for(int i = 0; i < numClouds; i ++)
			{
				GenerateClouds(holder);
			}
		}
	}

	private void GenerateClouds(GameObject holder)
	{

		float randomX = Random.Range(CloudDestroyer.collider2D.bounds.min.x, CloudGenerator.collider2D.bounds.max.x);
		float randomY = Random.Range(CloudGenerator.collider2D.bounds.min.y, CloudGenerator.collider2D.bounds.max.y);

		Vector2 randomLoc = new Vector2(randomX, randomY);
	
		GameObject newCloud = GameObject.Instantiate(cloudPrefab, randomLoc, Quaternion.identity) as GameObject;
		newCloud.transform.parent = holder.gameObject.transform;
		newCloud.transform.localScale = new Vector3(1,1,1);
	}
}
















