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

	private MasterGenerator masterGenerator;
	private float goodBreathRange;
	private float halfGoodBreathRange;

	private float overblownRange;

	private float underblownRange;

	// For fetching the stuff
	Property<float> breathStrength;
	Property<enumStatus> status;
	Property<float> breathLength;
	Property<bool> goodSet;
	Property<int> breathCountGood;
	Property<bool> breathGood;
	Property<int> setCount;
//	Property<float> minExhaleTime;

	private float currentBreath = 0;
	private enumStatus currentStatus = enumStatus.Initializing;
	private float thisBreathLength;
	private int goodBreathCount;
	private bool setIsGood;

	float exhalePresMin = 0;
	float exhalePresMax = 0;
	float exhalePressMinFloor = 0;
	float exhalePressMaxRoof = 0;
	float exhaleTimeMin = 0;
	float exhaleTimeMax = 0;
	float exhaleTimeMedium = 0;

	public float ExhaleTimeMax{
		get{return exhaleTimeMax;}}
	public float ExhaleTimeMin{
		get{return exhaleTimeMin;}}
	public float ExhaleCurrent{
		get{return breathLength.value;}}
	// Use this for initialization
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

		masterGenerator = GameObject.FindObjectOfType<MasterGenerator>();

		// Figure out the ranges
		goodBreathRange = exhalePresMax - exhalePresMin;
		overblownRange = exhalePressMaxRoof - exhalePresMax;
		underblownRange = exhalePresMin - exhalePressMinFloor;

		// Binding the status
		status = new Property<enumStatus>(enumStatus.Initializing);
		status.AddToBinding("Status", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		status.AddListener(StatusUpdate);

		// Binding the length of the current breath
		breathLength = new Property<float>(0);
		breathLength.AddToBinding("BreathLength", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathLength.AddListener(BreathLengthUpdate);

		// Binding to the Breath
		breathStrength = new Property<float>(0);
		breathStrength.AddToBinding("BreathStrength", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathStrength.AddListener(UpdateBreath);

		// Binding to the currentSet
		goodSet = new Property<bool>(false);
		goodSet.AddToBinding("SetIsGood", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		goodSet.AddListener(SetGoodUpdate);

		breathCountGood = new Property<int>(0);
		breathCountGood.AddToBinding("BreathCountGood", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathCountGood.AddListener(GoodBreathCountUpdate);

		breathGood = new Property<bool>(false);
		breathGood.AddToBinding("BreathIsGood", BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
		breathGood.AddListener(BreathGood);

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
		get { return status.value; }
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

//			if()

			return IdealStateEnum.ReadyToBreath;
		}
	}
	
	// Grabs the info from the properties and updates the local private variables

	void Update()
	{
//		Debug.Log("Set is " + setIsGood + " GoodbreathCount " + goodBreathCount);
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

	private void BreathGood(bool goodBreath)
	{
		Debug.Log("breath is " + goodBreath);
	}
}
