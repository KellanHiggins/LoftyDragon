using System;
using UnityEngine;
using System.Collections;

public class PEPDataGenerator : IBADataInterface
{
	#region IBADataInterface implementation

	public float ExhalePressureThresholdMin2 {
		get {
			throw new NotImplementedException ();
		}
	}

	public int BreathsMin {
		get {
			throw new NotImplementedException ();
		}
	}

	public int BreathsTarget {
		get {
			throw new NotImplementedException ();
		}
	}

	public int BreathsMax {
		get {
			throw new NotImplementedException ();
		}
	}

	public int SetsMin {
		get {
			throw new NotImplementedException ();
		}
	}

	public int SetsTarget {
		get {
			throw new NotImplementedException ();
		}
	}

	public int SetsMax {
		get {
			throw new NotImplementedException ();
		}
	}

	public float InhalePresureThreshold {
		get {
			throw new NotImplementedException ();
		}
	}

	public float TimeSensitivity {
		get {
			throw new NotImplementedException ();
		}
	}

	public DateTime LastSessionDateTime {
		get {
			throw new NotImplementedException ();
		}
	}

	public float LastSessionSuccessLevel {
		get {
			throw new NotImplementedException ();
		}
	}

	public int SessionCount {
		get {
			throw new NotImplementedException ();
		}
	}

	public float SessionSuccessLevelAvg {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult ExhaleStart {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult ExhaleEnd {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult GoodExhale {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult BreathEnd {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult SetEnd {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult SessionEnd {
		get {
			throw new NotImplementedException ();
		}
	}

	public BAResult StatusChanged {
		get {
			throw new NotImplementedException ();
		}
	}

	#endregion

	#region IBADataInterface implementation
	/// <summary>
	/// Gets the exhale Pressure threshold minimum.
	/// Pressure threshold above which the breath is considered to have started. 
	/// Pressure fluctuations below this level are ignored as noise and only when the pressure 
	/// passes this level is a breath considered to have started. 
	/// Pressure below this level for longer than the TimeSensitivity is the definition of the end of a breath. 
	/// When displaying a graph of the pressure, this is the minimum display value.
	/// </summary>
	/// <value>The exhale Pressure threshold minimum.</value>
	public float ExhalePressureThresholdMin {
		get {
			return 2F;
		}
	}
	/// <summary>
	/// Gets the exhale Pressure threshold max.
	/// Pressure threshold above which the pressure stops being measured. 
	/// This is a limit level above which the pressure stops being measured and instead the maximum value is shown. 
	/// When displaying a graph of the pressure, this is the maximum display value.
	/// </summary>
	/// <value>The exhale Pressure threshold max.</value>
	public float ExhalePressureThresholdMax {
		get {
			return 30F;
		}
	}
	/// <summary>
	/// Gets the exhale time threshold.
	/// The minimum time over which pressure above ExhalePressureThresholdMin must be held to 
	/// be considered the exhale part of a breath. 
	/// If the pressure drops below ExhalePressureThresholdMin in less time it is considered noise 
	/// and no counters are changed.
	/// </summary>
	/// <value>The exhale time threshold.</value>
	public float ExhaleTimeThreshold {
		get {
			return 1F;
		}
	}
	/// <summary>
	/// Gets the inhale time threshold.
	/// The minimum time over which pressure below ExhalePressureThresholdMin must be held to 
	/// be considered the inhale part of a breath. 
	/// If the pressure rises above ExhalePressureThresholdMin in less time it is considered part 
	/// of the prior exhale.
	/// </summary>
	/// <value>The inhale time threshold.</value>
	public float InhaleTimeThreshold {
		get {
			return 1F;
		}
	}

	#endregion

	#region IBADataInterface implementation

	public BAResult BreathIsGood {
		get {
			return (BAResult) mBreathSuccess;
		}
	}

	public BAResult SetIsGood {
		get {
			return (BAResult) (mBreathCountGood >= Breaths ? 1 : 2);
		}
	}

	public BAResult SessionIsGood {
		get {
			return (BAResult) (mSetCountGood >= SetCount ? 1 : 2);
		}
	}

	public int BreathCount {
		get {
			return mBreathCount;
		}
	}

	public int BreathCountGood {
		get {
			return mBreathCountGood;
		}
	}

	public int SetCount {
		get {
			return mSetCount;
		}
	}

	public int SetCountGood {
		get {
			return mSetCountGood;
		}
	}
	private float sumvalue = 0;
	private int valuecounter = 0;
	/// <summary>
	/// Reads the data.
	/// </summary>
	/// <returns>The data.</returns>
	public float ReadData ()
	{
		float pepData = 0;
		
		if (mStatus == 0)
		{
			long prepareSeconds = getTimeDiffSecond(prepareTime);
			if (prepareSeconds > MAXPREPARE)
			{
				Status = enumStatus.Inhale;
				mBreathCount = 0;
				mSetCount = 0;
				mBreathCountGood = 0;
				mSetCountGood = 0;
				inhaleTime = DateTime.Now;
			}
			else
			{
				pepData = 0; //indicate ready
			}
			
		}
		else
		{
			if (mSetCount < Breaths)
			{
				if (Status == enumStatus.Rest)
				{
					// ---- rest
					float restSeconds = getTimeDiffSecond(restTime);
					if (restSeconds > getRandom(InhaleTimeMin, InhaleTimeMax))
					{
						Status =  enumStatus.Inhale;
						mBreathCount = 0;
						mBreathCountGood = 0;
						inhaleTime = DateTime.Now;
					}
					else
					{
						pepData = getRandom(0, 2); // show little active to indicate
						// rest now
					}
					
				}
				else
				{
					if (mBreathCount < Breaths)
					{
						if (Status == enumStatus.Inhale)
						{
							// ---- inhale
							float inhaleSeconds = getTimeDiffMilliSecond(inhaleTime);
							if (inhaleSeconds > getRandom(InhaleTimeMin, InhaleTimeMax))
							{
								Status =  enumStatus.Exhale;
								exhaleTime = DateTime.Now;
							}
							else
							{
								pepData = 0;
							}
						}
						
						if (Status == enumStatus.Exhale)
						{
							// ---- exhale
							float holdSeconds = getTimeDiffMilliSecond(exhaleTime);
							
							if (holdSeconds > getRandom(ExhaleTimeMin, ExhaleTimeMax))
							{
								Status =  enumStatus.Inhale;
								mBreathCount ++;
								inhaleTime = DateTime.Now;
								pepData = 0;
								float av = sumvalue / valuecounter;
								if(av >= ExhalePressureMin && av <= ExhalePressureMax)
								{
									mBreathSuccess = 1;
									mBreathCountGood ++;
								}
								else
									mBreathSuccess = 2;
								
							}
							else
							{
								pepData = getBreathValue();
								sumvalue = sumvalue + pepData;
								valuecounter ++;
							}
						}
						
					}
					else
					{
						mStatus = 3;
						mSetCount = mSetCount + 1;
						if(mBreathCountGood >= Breaths)
							mSetCountGood ++;
						// take a 1 - 2 minutes rest
						mBreathCount = 0;
						mBreathCountGood = 0;
						restTime = DateTime.Now;
					}
				}
			}
			else
			{
				mStatus = 4;
				pepData = 0; // indicate off line 
				//initData();
			}
		}
		return pepData;
	}
	private float getTimeDiffMilliSecond(DateTime orgTime)
	{
		DateTime currTime = System.DateTime.Now;
		return (float)(currTime - orgTime).TotalMilliseconds / 1000;
	}
		/// <summary>
		/// Gets the therapy minimum time in minutes. 
		/// PEP therapy should take at least 20 minutes
		/// </summary>
		/// <value>The therapy time in minutes.</value>
		public float SessionTimeMin {
				get {
						return (20F * _deltaTime);
				}
		}
		/// <summary>
		/// Gets the break time minimum in minutes.
		/// Default is 1 minute
		/// </summary>
		/// <value>The break time minimum in minutes.</value>
		public float RestTimeMin {
				get {
						return 1F;
				}
		}
		/// <summary>
		/// Gets the break time max in minutes.
		/// Default is 2 minutes
		/// </summary>
		/// <value>The break time max in minutes.</value>
		public float RestTimeMax {
				get {
						return 2F;
				}
		}
		/// <summary>
		/// Gets the repeat breaths per step.
		/// Default: repeat for 15 breaths, breathing at a normal resporatory rate.
		/// </summary>
		/// <value>The repeat breaths per step.</value>
		public int Breaths {
				get {
						return 15;
				}
		}
		/// <summary>
		/// Gets the repeat steps per therapy.
		/// Repeat the step for 6 times per therapy
		/// </summary>
		/// <value>The repeat steps per therapy.</value>
		public int Sets {
				get {
						return 6;
				}
		}
		/// <summary>
		/// Gets the breath out minimum time in seconds.
		/// Default is 3 seconds
		/// </summary>
		/// <value>The breath out minimum time in seconds.</value>
		public float ExhaleTimeMin {
				get {
						return 3F;
				}
		}
		/// <summary>
		/// Gets the breath out max time in seconds.
		/// Default is 4 seconds
		/// </summary>
		/// <value>The breath out max time in seconds.</value>
		public float ExhaleTimeMax {
				get {
						return 4F;
				}
		}

		/// <summary>
		/// Gets the break time between breaths minimum in seconds.
		/// Default is 3 second
		/// </summary>
		/// <value>The break time between breaths minimum in seconds.</value>
		public float InhaleTimeMin {
				get {
						return (3F * _deltaTime);
				}
		}

		/// <summary>
		/// Gets the break time between breaths max in seconds.
		/// Default is 4 seconds
		/// </summary>
		/// <value>The break time between breaths max in seconds.</value>
		public float InhaleTimeMax {
				get {
						return (4F * _deltaTime);
				}
		}

	/// <summary>
	/// Gets the breath out pressure minimum.
	/// The back pressure mininum is 10 cm of water.
	/// </summary>
	/// <value>The breath out pressure minimum.</value>
	public float ExhalePressureMin {
			get {
					return 10F;
			}
	}
	/// <summary>
	/// Gets the breath out pressure max.
	/// The back pressure maximum is 20 cm of water
	/// </summary>
	/// <value>The breath out pressure max.</value>
	public float ExhalePressureMax {
			get {
					return 20F;
			}
	}

	#endregion

	private float _deltaTime { get; set; }
	// **** required parameters
	private int mStatus = (int)enumStatus.Exhale;
	public int mBreathCount = 0;
	public int mSetCount = 0;
	private int mBreathSuccess = 0;
	
	private int mBreathCountGood = 0;
	private int mSetCountGood = 0;
	public DataMode GDataMode { get; set; }

	// **** internal use
	private int MAXPREPARE = 2;
	private int FAILRATE = 5; // 5% Fail rate

	private DateTime prepareTime, inhaleTime, exhaleTime, restTime;
	
		public PEPDataGenerator (float deltatime)
		{
				//ummmmmm this doesn't seem right
				initData ();
				_deltaTime = deltatime;
		}

		public void initData ()
		{
			mStatus = (int)enumStatus.Inhale;
			mBreathCount = 0;
			mSetCount = 0;
			mBreathCountGood = 0;
			mSetCountGood = 0;
			prepareTime = DateTime.Now;

			GDataMode = DataMode.Automatic;
		}

		public enumStatus Status { 
				get {
						return (enumStatus)mStatus;
				}	
				set {
						mStatus = (int)value;
				}
		}

		private long getTimeDiffSecond (DateTime orgTime)
		{
				DateTime currTime = System.DateTime.Now;
				return (long)(currTime - orgTime).TotalSeconds;
		}
	
		public float getBreathValue ()
		{
				float breathValue = 0;
				if (getRandom (1, 100) > FAILRATE) { // 95% meet 10 - 20 cm required
						breathValue = getRandom (ExhalePressureMin, ExhalePressureMax) + getRandomFloat ();
				} else {
						if (getRandom (1, 100) > 5) { // 95% failed because lower than 10 cm
								breathValue = getRandom (0, ExhalePressureMin - 1);
						} else { // 5% failed because higher than 20 cm
								breathValue = getRandom (ExhalePressureMax + 1, 40);
						}
				}
				return breathValue;
		}

		private int getRandom (int Low, int High)
		{
				return UnityEngine.Random.Range (Low, High);
		}

		private float getRandom (float Low, float High)
		{
				return UnityEngine.Random.Range (Low, High);
		}

		private float getRandomFloat ()
		{
				return UnityEngine.Random.value;
		}
}

public enum DataMode
{
		Automatic = 0,
		Manual
}