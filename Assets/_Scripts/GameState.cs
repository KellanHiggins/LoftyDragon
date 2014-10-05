using UnityEngine;
using System.Collections;
using Bindings;


/// <summary>
/// This script figures out what level the game is on
/// </summary>
public class GameState : MonoBehaviour {

	public enum GameStatesEnum
	{
		Ready,
		BreathingSet,
		RestBetweenSets,
		FinishSets
	}

	// Public getters

	public GameStatesEnum CurrentGameState
	{
		get { return gameState; } 
	}

	public int CurrentSet { get { return setCount; } }
	public int CurrentBreath { get { return breathCount; } }
	public float RestTimer { get { return (float)System.Math.Round((double)restTimer, 1); } }
	public float RestTimerCountdown
	{
		get
		{
			if(restTimer > 0)
			{
				return minRestTime - RestTimer;
			}
			else
			{
				return 0;
			}
		}
	}

	Property<float> breathLength;
	Property<enumStatus> breathingStatus;

	[SerializeField]
	private GameStatesEnum gameState;
	
	private int breathCount; // needs to get to at least 15 before working
	private int setCount; // needs to get to at least 6 before completing the game
	private int breathGoodCount; // the number of good breath counts

	// Manually hacked in systems numbers because the API wasn't working
	private float minGoodTime;
	private int minSetNum;
	private int maxBreathCount;
	private int breathsMin = 15;
	private float restTimer = 0;
	private float minRestTime = 60f;

	// Last frame statuses
	private enumStatus lastBreathingStatus;
	private float lastBreathLength;


	// Use this for initialization
	void Start () 
	{
//		DontDestroyOnLoad(this);

		breathLength = new Property<float>(0);
		breathLength.AddToBinding("BreathLength", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);

		breathingStatus = new Property<enumStatus>(enumStatus.Initializing);
		breathingStatus.AddToBinding("Status", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);

		minGoodTime = Binding<float>.GetBinding("ExhaleTimeMin").value();
		maxBreathCount = Binding<int>.GetBinding("BreathsMax").value();
		breathsMin = Binding<int>.GetBinding("BreathsMin").value();
		minRestTime = Binding<float>.GetBinding("RestTimeMin").value();
		minSetNum = Binding<int>.GetBinding("SetsMin").value();

		// Delete before publishing TODO
//		maxBreathCount = 5;
//		breathsMin = 2;
//		minRestTime = 5;
//		minSetNum = 2;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameState == GameStatesEnum.Ready && breathingStatus.value == enumStatus.Exhale)
		{
			gameState = GameStatesEnum.BreathingSet;
		}


		// Check for a good breath ending and add to the count
		if(lastBreathingStatus == enumStatus.Exhale && breathingStatus.value == enumStatus.Inhale)
		{
			breathCount += 1;
			if(lastBreathLength > minGoodTime)
			{
				breathGoodCount += 1;
			}
		}

		if(breathCount > maxBreathCount || breathGoodCount > breathsMin)
		{
			gameState = GameStatesEnum.RestBetweenSets;

			// resets the counts for breaths
			breathCount = 0;
			breathGoodCount = 0;

			// increases the set counter
			setCount += 1;
			restTimer = 0f;

			if(setCount > minSetNum)
			{
				gameState = GameStatesEnum.FinishSets;
			}
		}
	
		// If the player should be resting, add a timer onto the system.
		if(gameState == GameStatesEnum.RestBetweenSets)
		{
			restTimer += Time.deltaTime;
		}

		// If the resting time is over, ready to play again
		if(restTimer > minRestTime && gameState == GameStatesEnum.RestBetweenSets)
		{
			gameState = GameStatesEnum.Ready;

		}

		// Update the last frame
		
		lastBreathingStatus = breathingStatus.value;
		lastBreathLength = breathLength.value;
	}
}
