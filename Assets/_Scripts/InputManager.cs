using UnityEngine;
using System.Collections;
using BellaProject;
using Bindings;

public class InputManager : MonoBehaviour {

	private MasterGenerator masterGenerator;
	private float goodBreathRange;
	private float halfGoodBreathRange;

	private float overblownRange;

	private float underblownRange;

//	private float currentBreath;


	// For fetching the stuff
	Property<float> breathStrength;
	Property<enumStatus> status;
	Property<float> breathLength;
//	Property<float> minExhaleTime;

	private float currentBreath = 0;
	private enumStatus currentStatus = enumStatus.Initializing;
	private float thisBreathLength;

	float exhalePresMin = 0;
	float exhalePresMax = 0;
	float exhalePressMinFloor = 0;
	float exhalePressMaxRoof = 0;
	float exhaleTimeMin = 0;
	float exhaleTimeMax = 0;
	float exhaleTimeMedium = 0;

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

	public string GetSuccessBreath()
	{
		// They are ready

		// Then they exhale

		// Then once they pass a certain level, they are good

		// Need to get the next status

		// 

		return "";
	}

	public enum IdealStateEnum
	{
		ReadyToBreath = 0,
		KeepExhaling,
		Inhale,
		Rest
	}

	/// <summary>
	/// Returns the ideal of the user right at this moment. Out is -1 if the percentage hasn't started
	/// </summary>
	/// <returns>The state.</returns>
	public IdealStateEnum IdealState(out float breathCompletePercentage)
	{
		breathCompletePercentage = -1;
		if(this.BreathingStatus == enumStatus.Ready)
		{
			return IdealStateEnum.ReadyToBreath;
		}
		if(this.BreathingStatus == enumStatus.Exhale)
		{
			breathCompletePercentage = thisBreathLength / this.exhaleTimeMedium;
			// Check to see if the breath is good.
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


	
	// Update is called once per frame
	void Update () 
	{
		float percentage;
		Debug.Log(IdealState(out percentage));
		if(percentage > 0)
			Debug.Log(percentage * 100f);
	}

	// Grabs the info from the properties and updates the local private variables

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
}
