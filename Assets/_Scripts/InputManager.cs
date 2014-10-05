using UnityEngine;
using System.Collections;
using BellaProject;
using Bindings;

public class InputManager : MonoBehaviour {

	public enum IdealStateEnum
	{
		ReadyToBreath = 0,
		KeepExhaling,
		Inhale,
		Rest
	}

	// reference scripts
	private MasterGenerator masterGenerator;

	[SerializeField]
	private GameState gameState;


	private float goodBreathRange;
	private float halfGoodBreathRange;
	private float overblownRange;
	private float underblownRange;

	// For fetching the stuff
	Property<float> breathStrengthProp;
	Property<enumStatus> statusProp;
	Property<float> breathLengthProp;
	Property<bool> goodSetProp;
	Property<int> breathCountGoodProp;
	Property<bool> breathGoodProp;
	Property<int> setCountProp;
	Property<int> targetBreathNumberProp;
	Property<int> maxBreathNumberProp;
	Property<int> breathCountProp;

	Property<int> breathMinProp;

	private float currentBreath = 0;
	private enumStatus currentStatus = enumStatus.Initializing;
	private float thisBreathLength;
	private int goodBreathCount;
	private bool setIsGood;
	private int setCount;

	private int hackSetCount = 0;

	float exhalePresMin = 0;
	float exhalePresMax = 0;
	float exhalePressMinFloor = 0;
	float exhalePressMaxRoof = 0;
	float exhaleTimeMin = 0;
	float exhaleTimeMax = 0;
	float exhaleTimeMedium = 0;
	int breathCountMin = 12;
	int breathCountTarget = 15;
	int maxBreathNumber = 20;

	public float ExhaleTimeMax{
		get{return exhaleTimeMax;}}
	public float ExhaleTimeMin{
		get{return exhaleTimeMin;}}
	public float ExhaleCurrent{
		get{return breathLengthProp.value;}}


	// Game changing bools
	private bool playMiniGame = false;
	public bool PlayMiniGame { get { return playMiniGame; } }

	private bool hackFinish = false;
	public bool HackFinish { get { return hackFinish; } }


	void Start () 
	{
		// Initial status for the max and mins
		exhalePresMin = Binding<float>.GetBinding("ExhalePressureMin").value();
		exhalePresMax = Binding<float>.GetBinding("ExhalePressureMax").value();
		exhalePressMinFloor = Binding<float>.GetBinding("ExhalePressureThresholdMin").value();
		exhalePressMaxRoof = Binding<float>.GetBinding("ExhalePressureThresholdMax").value();
		exhaleTimeMin = Binding<float>.GetBinding("ExhaleTimeThreshold").value();
		exhaleTimeMax = Binding<float>.GetBinding("ExhaleTimeMax").value();
		exhaleTimeMedium = (exhaleTimeMax - exhaleTimeMin) / 2 + exhaleTimeMin;

		breathCountMin = Binding<int>.GetBinding("BreathsMin").value();
//		breathCountTarget = Binding<int>.GetBinding("BreathsTarget").value();
		maxBreathNumber = Binding<int>.GetBinding("BreathsMax").value();


		masterGenerator = GameObject.FindObjectOfType<MasterGenerator>();

		// Figure out the ranges
		goodBreathRange = exhalePresMax - exhalePresMin;
		overblownRange = exhalePressMaxRoof - exhalePresMax;
		underblownRange = exhalePresMin - exhalePressMinFloor;

		// Binding the status
		statusProp = new Property<enumStatus>(enumStatus.Initializing);
		statusProp.AddToBinding("Status", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		statusProp.AddListener(StatusUpdate);

		// Binding the length of the current breath
		breathLengthProp = new Property<float>(0);
		breathLengthProp.AddToBinding("BreathLength", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathLengthProp.AddListener(BreathLengthUpdate);

		// Binding to the Breath
		breathStrengthProp = new Property<float>(0);
		breathStrengthProp.AddToBinding("BreathStrength", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathStrengthProp.AddListener(UpdateBreath);

		// Binding to the currentSet
		goodSetProp = new Property<bool>(false);
		goodSetProp.AddToBinding("SetIsGood", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		goodSetProp.AddListener(SetGoodUpdate);

		breathCountGoodProp = new Property<int>(0);
		breathCountGoodProp.AddToBinding("BreathCountGood", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathCountGoodProp.AddListener(GoodBreathCountUpdate);

		breathGoodProp = new Property<bool>(false);
		breathGoodProp.AddToBinding("BreathIsGood", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathGoodProp.AddListener(BreathGood);

		breathCountProp = new Property<int>(0);
		breathCountProp.AddToBinding("BreathCount", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);

		setCountProp = new Property<int>(0);
		setCountProp.AddToBinding("SetCount", BindingDirection.BiDirectional, AssignmentOnAdd.TakeBindingValue);
		setCountProp.AddListener(SetCountUpdate);


	}

	public float GetFlyingControl()
	{

		// Returns the percentage in the sweet spot
		if(currentBreath < exhalePresMax && currentBreath > exhalePresMin)
		{
			float breathAboveMin = currentBreath - exhalePresMin;
		 	return breathAboveMin / (goodBreathRange / 2) - 1;
		}

		// Returns something above 1 if the person is blowing over
		else if(currentBreath > exhalePresMax)
		{
			float overblownAmount = currentBreath - masterGenerator.ExhalePressureMax;
			return (overblownAmount / overblownRange) + 1;
		}

		// Returns something below zero if the person is not blowing
		else if(currentBreath < exhalePresMin)
		{
			float underBlownAmount = exhalePresMax - currentBreath;
			return (1 - (underBlownAmount / underblownRange)) - 1;
		}

		return 0;
	}

	public enumStatus BreathingStatus
	{
		get { return statusProp.value; }
	}

	/// <summary>
	/// Returns the ideal of the user right at this moment. Out is -1 if the percentage hasn't started
	/// </summary>
	/// <returns>The state.</returns>
	public float IdealStatePercentage()
	{
		if(this.BreathingStatus == enumStatus.Exhale && IdealState == IdealStateEnum.KeepExhaling)
		{
			return thisBreathLength / this.exhaleTimeMedium;
		}

		return -1;
	}

	public IdealStateEnum IdealState
	{
		get 
		{
			if(this.BreathingStatus == enumStatus.Ready)
			{
				return IdealStateEnum.ReadyToBreath;
			}
			if(this.BreathingStatus == enumStatus.Exhale)
			{
				if(thisBreathLength < this.exhaleTimeMedium)
				{
					return IdealStateEnum.KeepExhaling;
				}
				else
				{
					return IdealStateEnum.Inhale;
				}
			}

			return IdealStateEnum.ReadyToBreath;
		}
	}

	private void UpdateBreath(float strength)
	{
		currentBreath = strength;
	}

	private void StatusUpdate(enumStatus status)
	{
		currentStatus = status;
	}

	private void BreathLengthUpdate(float length)
	{
		thisBreathLength = length;
	}

	private void SetGoodUpdate(bool setGood)
	{
		setIsGood = setGood;	
	}

	private void GoodBreathCountUpdate(int num)
	{
		goodBreathCount = num;
	}

	private void SetCountUpdate(int num)
	{
		setCount = num;
		Debug.Log("Number of Set Counts" + num);
	}

	private void BreathGood(bool goodBreath)
	{
		Debug.Log("breath is " + goodBreath);
	}
}
