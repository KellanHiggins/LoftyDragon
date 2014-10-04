using UnityEngine;
using System.Collections;
using Bindings;
namespace BellaProject
{
	public class MasterGenerator :GenericManager<MasterGenerator>
	{
		#region Dynamic Variables
		/// <summary>
		/// The status of current breath
		/// </summary>
		private readonly Property<enumStatus>StatusProperty = new Property<enumStatus> (enumStatus.Initializing);
		/// <summary>
		/// The current pressure value in cm H2O. 
		/// The value is between 0 and ExhalePressureThresholdMax
		/// </summary>
		public readonly Property<float> BreathStrength = new Property<float> (0);
		/// <summary>
		/// The length of the current exhal.
		/// </summary>
		public readonly Property<float> BreathLength = new Property<float> (0);
		/// <summary>
		/// Count of breaths in this set. Counter is reset at the beginning of a set.
		/// </summary>
		public readonly Property<int> BreathCount = new Property<int> (0);
		/// <summary>
		/// Count of good breaths in this set.  Counter is reset at the beginning of a set.
		/// </summary>
		public readonly Property<int> BreathCountGood = new Property<int> (0);
		/// <summary>
		/// Set to True as soon as an exhale is good.  Reset to False at the start of an exhale.
		/// </summary>
		public readonly Property<BAResult> BreathIsGood = new Property<BAResult> (BAResult.None);
		/// <summary>
		/// Count of sets in this session.  Counter is reset at the beginning of a session.
		/// </summary>
		public readonly Property<int> SetCount = new Property<int> (0);
		/// <summary>
		/// Count of good sets in this session.  Counter is reset at the beginning of a session.
		/// </summary>
		public readonly Property<int> SetCountGood = new Property<int> (0);
		/// <summary>
		/// Set to True as soon as a set is good.  Reset to False at the start of a set.
		/// </summary>
		public readonly Property<BAResult> SetIsGood = new Property<BAResult> (BAResult.None);
		/// <summary>
		/// Set to True as soon as a session is good.  Reset to False at the start of a session.
		/// </summary>
		public readonly Property<BAResult> SessionIsGood = new Property<BAResult> (BAResult.None);
		#endregion
		float timeAt0 = 0f;
	
		public ParameterGenerator Parameters{ get { return generator; } }
		private ParameterGenerator generator;

		#region Static Variables
		public enum InputTypes
		{
				Keys,
				MouseScroll,
				Automatic
		}
		public InputTypes inputType = InputTypes.Keys;
		/// <summary>
		/// The minimum time of each session
		/// </summary>
		public float SessionTimeMin = 1200;
		public float RestTimeMin = 60;
		public float RestTimeMax = 120;
		public int Breaths = 15;
		public int BreathsMin = 12;
		public int BreathsTarget = 15;
		public int BreathsMax = 20;
		public int Sets = 6;
		public int SetsMin = 6;
		public int SetsTarget = 6;
		public int SetsMax = 6;
		public float ExhaleTimeMin = 1F;
		public float ExhaleTimeMax = 4F;
		public float ExhalePressureMin = 10F;
		public float ExhalePressureMax = 20F;
		public float ExhalePressureThresholdMax = 30F;
		public float ExhalePressureThresholdMin = 2F;
		public float ExhalePressureThresholdMin2 = 2F;
		public float ExhaleTimeThreshold = 1F;
		public float InhaleTimeMin = 2F;
		public float InhaleTimeMax = 5F;
		public float InhaleTimeThreshold = 0.5F;
		public float InhalePressureThreshold = 2F;
		#endregion
	
		#if UNITY_EDITOR
				//this should not be used in code only for debuging purposes in editor
				[SerializeField]
				private enumStatus
						debugShowCurrentState = enumStatus.Initializing;
		#endif
		/// <summary>
		/// Breaths the length changed.
		/// </summary>
		/// <param name="value">Value.</param>
		void BreathLengthChanged (float value)
		{
			if (value >= generator.ExhaleTimeMin) {
				BreathIsGood.value = BAResult.Yes;
			} else {
				BreathIsGood.value = BAResult.No;
			}
		}
		/// <summary>
		/// Breath Count Number changed.
		/// </summary>
		/// <param name="value">Value.</param>
		void BreathCountChanged (int value)
		{
			if (value > BreathsMax) {
				Manager.messenger.Publish (this, BellaMessages.SetEnd);				
				++ SetCount.value;
			}
		}
		/// <summary>
		/// Breaths Count Good changed.
		/// </summary>
		/// <param name="value">Value.</param>
		void BreathCountGoodChanged (int value)
		{
			if (value >= generator.BreathsMin) {
				SetIsGood.value = BAResult.Yes;
				if (value == generator.BreathsMin)
					SetCountGood.value ++;

				if (value == generator.BreathsMax)
					SetCount.value = generator.SetsMax;
			}
			else
				SetIsGood.value = BAResult.None;
		}
		/// <summary>
		/// Set Counter Number changed
		/// </summary>
		/// <param name="value">Value.</param>
		void SetCountChanged (int value)
		{	
			BreathCount.value = 0;
			BreathCountGood.value = 0;
			if (value > SetsMax) {
				StatusProperty.value = enumStatus.Finish;
			}
			else {
				if (value > 0)
					StatusProperty.value = enumStatus.Rest;
			}
		}
		/// <summary>
		/// Set Count Good Changed
		/// </summary>
		/// <param name="value">Value.</param>
		void SetCountGoodChanged (int value)
		{
			if (value >= generator.SetsMin)
					SessionIsGood.value = BAResult.Yes;
			else
					SessionIsGood.value = BAResult.No;

			if (value == generator.SetsMax)
				SetCount.value = generator.SetsMax;
		}
		/// <summary>
		/// Breath is good value changed
		/// </summary>
		/// <param name="value">Value.</param>
		void BreathIsGoodChanged (BAResult value)
		{
			if (value == BAResult.Yes)
				Manager.messenger.Publish (this, BellaMessages.GoodExhale);
		}
		/// <summary>
		/// Set is good value changed
		/// </summary>
		/// <param name="value">Value.</param>
		void SetIsGoodChanged (BAResult value)
		{
			if (value == BAResult.Yes)
				Manager.messenger.Publish (this, BellaMessages.GoodSet);
		}
		/// <summary>
		/// Session is good value changed
		/// </summary>
		/// <param name="value">Value.</param>
		void SessionIsGoodChanged (BAResult value)
		{
			if (value == BAResult.Yes)
				Manager.messenger.Publish (this, BellaMessages.SessionEnd);								
		}

		protected override void AwakeNew ()
		{
			BreathStrength.AddToBinding (ParameterBindingNames.BreathStrength, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathLength.AddToBinding (ParameterBindingNames.BreathLength, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathCount.AddToBinding (ParameterBindingNames.BreathCount, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SetCount.AddToBinding (ParameterBindingNames.SetCount, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			
			BreathCountGood.AddToBinding (ParameterBindingNames.BreathCountGood, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SetCountGood.AddToBinding (ParameterBindingNames.SetCountGood, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathIsGood.AddToBinding (ParameterBindingNames.BreathIsGood, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SetIsGood.AddToBinding (ParameterBindingNames.SetIsGood, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SessionIsGood.AddToBinding (ParameterBindingNames.SessionIsGood, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			StatusProperty.AddToBinding (ParameterBindingNames.Status, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);

			///Status Changed Listener
			StatusProperty.AddListener (NewState);							
			BreathLength.AddListener (BreathLengthChanged);
			BreathCount.AddListener (BreathCountChanged);
			BreathCountGood.AddListener (BreathCountGoodChanged);
			SetCount.AddListener (SetCountChanged);
			SetCountGood.AddListener (SetCountGoodChanged);
			
			BreathIsGood.AddListener (BreathIsGoodChanged);
			SetIsGood.AddListener (SetIsGoodChanged);
			SessionIsGood.AddListener (SessionIsGoodChanged);

			generator = new ParameterGenerator (this, InitializationComplete);

			StatusProperty.value = generator.Status;				
			BreathCount.value = generator.BreathCount;
			BreathCountGood.value = generator.BreathCountGood;
			SetCount.value = generator.SetCount;
			SetCountGood.value = generator.SetCountGood;

		}
		/// <summary>
		/// Initializations the complete.
		/// </summary>
		private void InitializationComplete ()
		{				
			StatusProperty.value = enumStatus.Ready;
		}
		//this method is used in conjunction with the property listener to create a coroutine state machine 
		private void NewState (enumStatus status)
		{
			#if UNITY_EDITOR
			debugShowCurrentState = status;
			#endif
			Manager.messenger.Publish (this, BellaMessages.StatusChanged);				
			switch (status) {
				case enumStatus.Ready:
					StartCoroutine (Ready ());
					break;
				case enumStatus.Exhale:
					if (BreathCount.value > 0 || SetCount.value > 0)
						Manager.messenger.Publish (this, BellaMessages.BreathEnd);						
					StartCoroutine (Exhale ());
					break;
				case enumStatus.Finish:
					StartCoroutine (Finish ());
					break;
				case enumStatus.Inhale:
					StartCoroutine (Inhale ());
					break;
				case enumStatus.Rest:
					StartCoroutine (Rest ());
					break;
				case enumStatus.Initializing:
					break;
				default:
					Debug.LogWarning ("Enum " + status + " is not set in NewState for MasterGenerator");
					break;
			}
		}
		private float idleTime = 0f;
		/// <summary>
		/// Ready this instance.
		/// </summary>
		IEnumerator Ready ()
		{
			//somewhere in here we should account the maxTimes for waiting for next breath and rest times, but how much does this matter?
			//we can send some messages when the max wait times are reached without breathing or should we enfore it further some how?
			yield return null;
			idleTime = 0;
			Manager.messenger.Publish (this, BellaMessages.ReadyForInput);
			while (StatusProperty.value == enumStatus.Ready) {

				if (generator.ReadData () >= generator.ExhalePressureThresholdMin) {
					Manager.messenger.Publish (this, BellaMessages.ExhaleStart);	
					StatusProperty.value = enumStatus.Exhale;
				}
				/*
				else
				{
					idleTime += Time.deltaTime;
					if (idleTime > generator.InhaleTimeMax)
					{
						if (BreathCountGood.value >= generator.BreathsMin)
							BreathCount.value = generator.BreathsMax;
						if (SetCountGood.value >= generator.SetsMin)
						{
							SetCount.value = generator.SetsMax;
							BreathCount.value = generator.BreathsMax;
						}

						idleTime = 0;
						BreathCount.value ++;
					}
				}
				*/
				yield return null;
			}		
		}
		/// <summary>
		/// Exhale this instance.
		/// </summary>
		IEnumerator Exhale ()
		{
			while (StatusProperty.value == enumStatus.Exhale) {						
				float value = generator.ReadData ();
				generator.ErrorMessageProperty.value = value.ToString();
				BreathStrength.value = Mathf.Lerp (BreathStrength.value, generator.ReadData (), 0.1f);
				if (value >= generator.ExhalePressureMin && value <= generator.ExhalePressureMax) {
					BreathLength.value += Time.deltaTime;
				}

				//should be changed to be under a threshold? does the device have a degree of error that we should account for?
				if (value <= generator.ExhalePressureThresholdMin) {
					timeAt0 += Time.deltaTime;
					//should give this a parameter value to account for how long the device might take to respond
					// and how long we want to wait until confirming the breath is finished
					if (timeAt0 > generator.InhaleTimeThreshold) {
						Manager.messenger.Publish (this, BellaMessages.ExhaleEnd);						
						BreathFinished ();
					}
				} else {
					timeAt0 = 0;
				}
				yield return null;
			}
		}
		/// <summary>
		/// Finish this instance.
		/// </summary>
		IEnumerator Finish ()
		{
			while (StatusProperty.value == enumStatus.Finish) {
				Manager.messenger.Publish (this, BellaMessages.SessionFinished);
				yield return null;
			}
		}
		/// <summary>
		/// Inhale this instance.
		/// </summary>
		IEnumerator Inhale ()
		{
			yield return new WaitForSeconds (generator.InhaleTimeThreshold);
			while (StatusProperty.value == enumStatus.Inhale) {
				float value = generator.ReadData ();
				generator.ErrorMessageProperty.value = value.ToString();
				BreathStrength.value = Mathf.Lerp (BreathStrength.value, generator.ReadData (), 0.1f);
				if (value >= generator.ExhalePressureThresholdMin) {
					StatusProperty.value = enumStatus.Ready;					
				}
				yield return null;
			}
		}
		/// <summary>
		/// Rest this instance.
		/// </summary>
		IEnumerator Rest ()
		{
			Manager.messenger.Publish (this, BellaMessages.BreakTimeStarted);
			yield return new WaitForSeconds (generator.InhaleTimeMax);
			while (StatusProperty.value == enumStatus.Rest) {
				float value = generator.ReadData ();
				generator.ErrorMessageProperty.value = value.ToString();
				BreathStrength.value = Mathf.Lerp (BreathStrength.value, generator.ReadData (), 0.1f);
				if (value >= generator.ExhalePressureThresholdMin) {
					Manager.messenger.Publish (this, BellaMessages.BreakTimeMinReached);
					StatusProperty.value = enumStatus.Ready;					
				}
				yield return null;
			}
		}
		/// <summary>
		/// Breath finished
		/// </summary>
		void BreathFinished ()
		{		
			if (BreathLength.value > generator.ExhaleTimeThreshold) {
				Manager.messenger.Publish (this, BellaMessages.ExhaleEnd);
				BreathCount.value++;
				if (BreathLength.value > generator.ExhaleTimeMin) {
					Manager.messenger.Publish (this, BellaMessages.GoodBreath);
					Manager.messenger.Publish (this, BellaMessages.GoodExhale);		
					BreathCountGood.value++;
				} else {
					Manager.messenger.Publish (this, BellaMessages.WeakBreath);
				}
			} 

			BreathStrength.value = 0;
			BreathLength.value = 0;
			StatusProperty.value = enumStatus.Inhale;
		}
		/// <summary>
		/// Validates the data.
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="methodOnValidation">Method on validation.</param>
		public IEnumerator ValidateData (OnInitilization methodOnValidation)
		{
			while (generator.InhaleTimeMaxProperty.value==0) {
				generator.UpdateAndroidProperties ();					
				yield return null;
			}
			methodOnValidation ();
		}
	}
	/// <summary>
	/// On initilization.
	/// </summary>
	public delegate void OnInitilization ();
	/// <summary>
	/// Parameter generator.
	/// </summary>
	public class ParameterGenerator: IBADataInterface
	{
		private IBADataInterface _generator ;
		#if UNITY_EDITOR
		private float scrollValue = 0f;
		#endif
		private MasterGenerator master;
		/// <summary>
		/// Initializes a new instance of the <see cref="BellaProject.ParameterGenerator"/> class.
		/// </summary>
		/// <param name="master">Master.</param>
		/// <param name="method">Method.</param>
		public ParameterGenerator (MasterGenerator master, OnInitilization method)
		{
			this.method = method;
			this.master = master;
			ErrorMessageProperty = new Property<string> (string.Empty);
			ErrorMessageProperty.AddToBinding ("ErrorMessage", BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);			
		#if UNITY_EDITOR						
			_generator = new PEPDataGenerator(Time.deltaTime);
			EditorPropertyInitialization (master);
		#elif UNITY_ANDROID
			if (GameMode.gameMode == GameMode.GameModes.DebugMode) {
				_generator = new PEPDataGenerator(Time.deltaTime);
				EditorPropertyInitialization (master);
			}else{
				initAndroidPlugin ();
			}
		#endif
		}
		/// <summary>
		/// Initializations the complete.
		/// </summary>
		void InitializationComplete ()
		{
			method ();			
		}
		/// <summary>
		/// The method.
		/// </summary>
		OnInitilization method;
		/// <summary>
		/// Adds the bindings.
		/// </summary>
		private void AddBindings ()
		{
			SessionTimeMinProperty.AddToBinding (ParameterBindingNames.SessionTimeMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			RestTimeMinProperty .AddToBinding (ParameterBindingNames.RestTimeMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			RestTimeMaxProperty.AddToBinding (ParameterBindingNames.RestTimeMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathsProperty .AddToBinding (ParameterBindingNames.Breaths, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathsMinProperty .AddToBinding (ParameterBindingNames.BreathsMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathsTargetProperty .AddToBinding (ParameterBindingNames.SetsTarget, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			BreathsMaxProperty .AddToBinding (ParameterBindingNames.BreathsMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);			
			SetsProperty .AddToBinding (ParameterBindingNames.Sets, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SetsMinProperty .AddToBinding (ParameterBindingNames.SetsMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SetsTargetProperty .AddToBinding (ParameterBindingNames.SetsTarget, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			SetsMaxProperty .AddToBinding (ParameterBindingNames.SetsMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);			
			ExhaleTimeMinProperty .AddToBinding (ParameterBindingNames.ExhaleTimeMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			ExhaleTimeMaxProperty .AddToBinding (ParameterBindingNames.ExhaleTimeMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			ExhaleTimeThresholdProperty .AddToBinding (ParameterBindingNames.ExhaleTimeThreshold, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);			
			ExhalePressureMinProperty .AddToBinding (ParameterBindingNames.ExhalePressureMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			ExhalePressureMaxProperty .AddToBinding (ParameterBindingNames.ExhalePressureMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			ExhalePressureThresholdMaxProperty.AddToBinding (ParameterBindingNames.ExhalePressureThresholdMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			ExhalePressureThresholdMinProperty.AddToBinding (ParameterBindingNames.ExhalePressureThresholdMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			ExhalePressureThresholdMin2Property.AddToBinding (ParameterBindingNames.ExhalePressureThresholdMin2, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);			
			InhaleTimeMinProperty .AddToBinding (ParameterBindingNames.InhaleTimeMin, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			InhaleTimeMaxProperty.AddToBinding (ParameterBindingNames.InhaleTimeMax, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			InhaleTimeThresholdProperty.AddToBinding (ParameterBindingNames.InhaleTimeThreshold, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);
			InhalePresureThresholdProperty.AddToBinding (ParameterBindingNames.InhalePressureThreshold, BindingDirection.PropertyToBinding, AssignmentOnAdd.TakePropertyValue);

			InitializationComplete ();
		}
		/// <summary>
		/// Editors the property initialization.
		/// </summary>
		/// <param name="master">Master.</param>
		private void EditorPropertyInitialization (MasterGenerator master)
		{		
			SessionTimeMinProperty = new Property<float> (master.SessionTimeMin);
			RestTimeMinProperty = new Property<float> (master.RestTimeMin);
			RestTimeMaxProperty = new Property<float> (master.RestTimeMax);
			BreathsProperty = new Property<int> (master.Breaths);
			BreathsMinProperty = new Property<int> (master.BreathsMin);
			BreathsTargetProperty = new Property<int> (master.BreathsTarget);
			BreathsMaxProperty = new Property<int> (master.BreathsMax);			
			SetsProperty = new Property<int> (master.Sets);
			SetsMinProperty = new Property<int> (master.SetsMin);
			SetsTargetProperty = new Property<int> (master.SetsTarget);
			SetsMaxProperty = new Property<int> (master.SetsMax);			
			ExhaleTimeMinProperty = new Property<float> (master.ExhaleTimeMin);
			ExhaleTimeMaxProperty = new Property<float> (master.ExhaleTimeMax);
			ExhalePressureMinProperty = new Property<float> (master.ExhalePressureMin);
			ExhalePressureMaxProperty = new Property<float> (master.ExhalePressureMax);
			ExhalePressureThresholdMaxProperty = new Property<float> (master.ExhalePressureThresholdMax);
			ExhalePressureThresholdMinProperty = new Property<float> (master.ExhalePressureThresholdMin);
			ExhalePressureThresholdMin2Property = new Property<float> (master.ExhalePressureThresholdMin2);
			ExhaleTimeThresholdProperty = new Property<float> (master.ExhaleTimeThreshold);			
			InhaleTimeMinProperty = new Property<float> (master.InhaleTimeMin);
			InhaleTimeMaxProperty = new Property<float> (master.InhaleTimeMax);
			InhaleTimeThresholdProperty = new Property<float> (master.InhaleTimeThreshold);
			InhalePresureThresholdProperty = new Property<float> (master.InhalePressureThreshold);

			///Init dynamic values
			StatusProperty = new Property<enumStatus> (enumStatus.Ready);
			BreathCountProperty = new Property<int> (0);
			BreathCountGoodProperty = new Property<int> (0);
			SetCountProperty = new Property<int> (0);
			SetCountGoodProperty = new Property<int> (0);
			AddBindings ();
		}
		/// <summary>
		/// Updates the android properties.
		/// </summary>
		public void UpdateAndroidProperties ()
		{
			SessionTimeMinProperty .SetValue (master.SessionTimeMin);
			RestTimeMinProperty .SetValue (_generator.RestTimeMin);
			RestTimeMaxProperty .SetValue (_generator.RestTimeMax);
			BreathsProperty .SetValue (_generator.Breaths);
			BreathsMinProperty .SetValue (_generator.BreathsMin);
			BreathsTargetProperty .SetValue (_generator.BreathsTarget);
			BreathsMaxProperty .SetValue (_generator.BreathsMax);			
			SetsProperty .SetValue (_generator.Sets);
			SetsMinProperty .SetValue (_generator.SetsMin);
			SetsTargetProperty .SetValue (_generator.SetsTarget);
			SetsMaxProperty .SetValue (_generator.SetsMax);			
			ExhaleTimeMinProperty .SetValue (_generator.ExhaleTimeMin);
			ExhaleTimeMaxProperty .SetValue (_generator.ExhaleTimeMax);
			ExhalePressureMinProperty .SetValue (_generator.ExhalePressureMin);
			ExhalePressureMaxProperty .SetValue (_generator.ExhalePressureMax);
			ExhalePressureThresholdMaxProperty .SetValue (_generator.ExhalePressureThresholdMax);
			ExhalePressureThresholdMinProperty .SetValue (_generator.ExhalePressureThresholdMin);
			ExhalePressureThresholdMin2Property .SetValue (_generator.ExhalePressureThresholdMin2);
			ExhaleTimeThresholdProperty .SetValue (_generator.ExhaleTimeThreshold);			
			InhaleTimeMinProperty .SetValue (_generator.InhaleTimeMin);
			InhaleTimeMaxProperty .SetValue (_generator.InhaleTimeMax);
			InhaleTimeThresholdProperty .SetValue (_generator.InhaleTimeThreshold);
			InhalePresureThresholdProperty .SetValue (_generator.InhalePresureThreshold);

			///Update dynamic values, read value from data service
			StatusProperty.SetValue (_generator.Status);
			BreathCountProperty.SetValue (_generator.BreathCount);
			BreathCountGoodProperty.SetValue (_generator.BreathCountGood);
			SetCountProperty.SetValue (_generator.SetCount);
			SetCountGoodProperty.SetValue (_generator.SetCountGood);
		}
		/// <summary>
		/// Androids the initialization.
		/// </summary>
		private void AndroidInitialization ()
		{	
			try {
				_generator = new PEPDataPlugin();			

				SessionTimeMinProperty = new Property<float> (master.SessionTimeMin);
				RestTimeMinProperty = new Property<float> (_generator.RestTimeMin);
				RestTimeMaxProperty = new Property<float> (_generator.RestTimeMax);
				BreathsProperty = new Property<int> (_generator.Breaths);
				BreathsMinProperty = new Property<int> (_generator.BreathsMin);
				BreathsTargetProperty = new Property<int> (_generator.BreathsTarget);
				BreathsMaxProperty = new Property<int> (_generator.BreathsMax);			
				SetsProperty = new Property<int> (_generator.Sets);
				SetsMinProperty = new Property<int> (_generator.SetsMin);
				SetsTargetProperty = new Property<int> (_generator.SetsTarget);
				SetsMaxProperty = new Property<int> (_generator.SetsMax);			
				ExhaleTimeMinProperty = new Property<float> (_generator.ExhaleTimeMin);
				ExhaleTimeMaxProperty = new Property<float> (_generator.ExhaleTimeMax);
				ExhalePressureMinProperty = new Property<float> (_generator.ExhalePressureMin);
				ExhalePressureMaxProperty = new Property<float> (_generator.ExhalePressureMax);
				ExhalePressureThresholdMaxProperty = new Property<float> (_generator.ExhalePressureThresholdMax);
				ExhalePressureThresholdMinProperty = new Property<float> (_generator.ExhalePressureThresholdMin);
				ExhalePressureThresholdMin2Property = new Property<float> (_generator.ExhalePressureThresholdMin2);
				ExhaleTimeThresholdProperty = new Property<float> (_generator.ExhaleTimeThreshold);			
				InhaleTimeMinProperty = new Property<float> (_generator.InhaleTimeMin);
				InhaleTimeMaxProperty = new Property<float> (_generator.InhaleTimeMax);
				InhaleTimeThresholdProperty = new Property<float> (_generator.InhaleTimeThreshold);
				InhalePresureThresholdProperty = new Property<float> (_generator.InhalePresureThreshold);

				///Init dynamic values, read value from data service
				StatusProperty = new Property<enumStatus> (_generator.Status);
				BreathCountProperty = new Property<int> (_generator.BreathCount);
				BreathCountGoodProperty = new Property<int> (_generator.BreathCountGood);
				SetCountProperty = new Property<int> (_generator.SetCount);
				SetCountGoodProperty = new Property<int> (_generator.SetCountGood);

				AddBindings ();
				ErrorMessageProperty.value = "Ready for Input";				
			}catch(System.Exception ex)
			{
				ErrorMessageProperty.value = ex.Message;
			}
		}
		/// <summary>
		/// Inits the android plugin.
		/// </summary>
		private void initAndroidPlugin ()
		{
			AndroidInitialization ();
			master.StartCoroutine (master.ValidateData (AndroidInitialization));						
		}

		#region IBADataInterface implementation
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="property">Property.</param>
		/// <param name="def">Def.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private T GetValue<T>(Property<T> property, T def)
		{
			if (property == null)
				property = new Property<T> (def);
			return property.value;
		}
		public Property<float> ExhalePressureThresholdMin2Property;
		/// <summary>
		/// Gets the exhale pressure threshold min2.
		/// Same as ExhalePresureThresholdMin but used for second and subsequent breaths of a set. 
		/// There is some thinking that if the mask is kept tight on the face and no pressure is lost 
		/// subsequent breaths start at a non-zero pressure level. 
		/// So this parameter allows the exhale pressure threshold to be set higher for later breaths if necessary.
		/// </summary>
		/// <value>The exhale pressure threshold min2.</value>
		public float ExhalePressureThresholdMin2 {
			get {
				return ExhalePressureThresholdMin2Property.value;
			}
		}
		public Property<int> BreathsMinProperty;
		/// <summary>
		/// Gets the breaths minimum.
		/// The minimum number of breaths in a set. 
		/// A set is not considered complete until this number of breaths is completed.
		/// </summary>
		/// <value>The breaths minimum.</value>
		public int BreathsMin {
			get {
				return BreathsMinProperty.value;
			}
		}
		public Property<int> BreathsTargetProperty;
		/// <summary>
		/// Gets the breaths target.
		/// The target number of breaths in a set. 
		/// There are no consequences for completing more breaths in a set than this value.
		/// </summary>
		/// <value>The breaths target.</value>
		public int BreathsTarget {
			get {
				return BreathsTargetProperty.value;
			}
		}
		public Property<int> BreathsMaxProperty;
		/// <summary>
		/// Gets the breaths max.
		/// The maximum number of breaths in a set. If the user has completed this number of 
		/// breaths the set is considered complete no matter how many of the breaths were good.
		/// </summary>
		/// <value>The breaths max.</value>
		public int BreathsMax {
			get {
				return BreathsMaxProperty.value;
			}
		}
		public Property<int> SetsMinProperty;
		/// <summary>
		/// Gets the sets minimum.
		/// The minimum number of sets in a session. 
		/// A session is considered complete when this number of valid sets is completed. 
		/// The game should indicate completion of the therapy session when this number of valid sets is complete.
		/// </summary>
		/// <value>The sets minimum.</value>
		public int SetsMin {
			get {
				return SetsMinProperty.value;
			}
		}
		public Property<int> SetsTargetProperty;
		/// <summary>
		/// Gets the sets target.
		/// Same as SetsMin. Parameter added for future considerations.
		/// </summary>
		/// <value>The sets target.</value>
		public int SetsTarget {
			get {
				return SetsTargetProperty.value;
			}
		}
		public Property<int> SetsMaxProperty;
		/// <summary>
		/// Gets the sets max.
		/// Same as SetsMin. Parameter added for future considerations.
		/// </summary>
		/// <value>The sets max.</value>
		public int SetsMax {
			get {
				return SetsMaxProperty.value;
			}
		}
		public Property<float> InhalePresureThresholdProperty;
		/// <summary>
		/// Gets the inhale presure threshold.
		/// Pressure below this level for longer than the TimeSensitivity is the definition of the 
		/// end of an exhale and the start of an inhale.
		/// </summary>
		/// <value>The inhale presure threshold.</value>
		public float InhalePresureThreshold {
			get {
				return InhalePresureThresholdProperty.value;
			}
		}
		public Property<float> TimeSensitivityProperty;
		/// <summary>
		/// Gets the time sensitivity.
		/// The time sensitivity below which no attention is paid to swings in the exhalation pressure. 
		/// That is, pressure spikes up or down shorter than this duration are ignored.
		/// Default is 0.1 sec
		/// </summary>
		/// <value>The time sensitivity.</value>
		public float TimeSensitivity {
			get {
				return TimeSensitivityProperty.value;
			}
		}
		public Property<System.DateTime> LastSessionDateTimeProperty;
		/// <summary>
		/// Gets the last session date time.
		/// The date and time of completion of the last therapy session with a success level of at least 50%
		/// </summary>
		/// <value>The last session date time.</value>
		public System.DateTime LastSessionDateTime {
			get {
				return LastSessionDateTimeProperty.value;
			}
		}
		public Property<float> LastSessionSuccessLevelProperty;
		/// <summary>
		/// Gets the last session success level.
		/// The success level in percent of the last session.
		/// </summary>
		/// <value>The last session success level.</value>
		public float LastSessionSuccessLevel {
			get {
				return LastSessionSuccessLevelProperty.value;
			}
		}
		public Property<int> SessionCountProperty;
		/// <summary>
		/// Gets the session count.
		/// A counter of sessions completed (since last reset) where the success level was at least 50%.
		/// </summary>
		/// <value>The session count.</value>
		public int SessionCount {
			get {
				return SessionCountProperty.value;
			}
		}
		public Property<float> SessionSuccessLevelAvgProperty;
		/// <summary>
		/// Gets the session success level avg.
		/// The average success level of all sessions completed (since last reset).
		/// </summary>
		/// <value>The session success level avg.</value>
		public float SessionSuccessLevelAvg {
			get {
				return SessionSuccessLevelAvgProperty.value;
			}
		}
		public Property<BAResult> ExhaleStartProperty;
		/// <summary>
		/// Gets the exhale start.
		/// The exhale phase of a breath has started. Note that it is possible to have several 
		/// false starts on the start of an exhale phase since the exhale starts when the 
		/// ExhalePressureThresholdMin pressure threshold is crossed, but could fail to stay 
		/// above that threshold for the ExhaleTimeThreshold time period required to complete 
		/// the exhale phase of a breath.
		/// </summary>
		/// <value>The exhale start.</value>
		public BAResult ExhaleStart {
			get {
				return ExhaleStartProperty.value;
			}
		}
		public Property<BAResult> ExhaleEndProperty;
		/// <summary>
		/// Gets the exhale end.
		/// </summary>
		/// <value>The exhale end.</value>
		public BAResult ExhaleEnd {
			get {
				return ExhaleEndProperty.value;
			}
		}
		public Property<BAResult> GoodExhaleProperty;
		/// <summary>
		/// Gets the good exhale.
		/// </summary>
		/// <value>The good exhale.</value>
		public BAResult GoodExhale {
			get {
				return GoodExhaleProperty.value;
			}
		}
		public Property<BAResult> BreathEndProperty;
		/// <summary>
		/// Gets the breath end.
		/// </summary>
		/// <value>The breath end.</value>
		public BAResult BreathEnd {
			get {
				return BreathEndProperty.value;
			}
		}
		public Property<BAResult> SetEndProperty;
		/// <summary>
		/// Gets the set end.
		/// </summary>
		/// <value>The set end.</value>
		public BAResult SetEnd {
			get {
				return SetEndProperty.value;
			}
		}
		public Property<BAResult> SessionEndProperty;
		/// <summary>
		/// Gets the session end.
		/// </summary>
		/// <value>The session end.</value>
		public BAResult SessionEnd {
			get {
				return SessionEndProperty.value;
			}
		}
		public Property<BAResult> StatusChangedProperty;
		/// <summary>
		/// Gets the status changed.
		/// </summary>
		/// <value>The status changed.</value>
		public BAResult StatusChanged {
			get {
				return StatusChangedProperty.value;
			}
		}

		#endregion

		public Property<string> ErrorMessageProperty;
		public string ErrorMessage{
			get {return ErrorMessageProperty.value;}
		}
		/// <summary>
		/// Gets the therapy minimum time in minutes. 
		/// PEP therapy should take at least 20 minutes
		/// </summary>
		/// <value>The therapy time in minutes.</value>
		public float SessionTimeMin { get { return SessionTimeMinProperty.value; } }
		public Property<float> SessionTimeMinProperty;
		/// <summary>
		/// Gets the break time minimum in minutes.
		/// Default is 1 minute
		/// </summary>
		/// <value>The break time minimum in minutes.</value>
		public float RestTimeMin { get { return RestTimeMinProperty.value; } }
		public Property<float> RestTimeMinProperty;
		/// <summary>
		/// Gets the break time max in minutes.
		/// Default is 2 minutes
		/// </summary>
		/// <value>The break time max in minutes.</value>
		public float RestTimeMax { get { return RestTimeMaxProperty.value; } }
		public Property<float> RestTimeMaxProperty;
		/// <summary>
		/// Gets the repeat breaths per step.
		/// Default: repeat for 15 breaths, breathing at a normal resporatory rate.
		/// </summary>
		/// <value>The repeat breaths per step.</value>
		public int Breaths { get { return BreathsProperty.value; } }
		public Property<int> BreathsProperty;
		/// <summary>
		/// Gets the repeat steps per therapy.
		/// Repeat the step for 6 times per therapy
		/// </summary>
		/// <value>The repeat steps per therapy.</value>
		public int Sets { get { return SetsProperty.value; } }
		public Property<int> SetsProperty;
		/// <summary>
		/// Gets the breath out minimum time in seconds.
		/// Default is 3 seconds
		/// </summary>
		/// <value>The breath out minimum time in seconds.</value>
		public float ExhaleTimeMin { get { return ExhaleTimeMinProperty.value; } }
		public Property<float> ExhaleTimeMinProperty;
		/// <summary>
		/// Gets the breath out max time in seconds.
		/// Default is 4 seconds
		/// </summary>
		/// <value>The breath out max time in seconds.</value>
		public float ExhaleTimeMax { get { return ExhaleTimeMaxProperty.value; } }
		public Property<float> ExhaleTimeMaxProperty;
		/// <summary>
		/// Gets the breath out pressure minimum.
		/// The back pressure mininum is 10 cm of water.
		/// </summary>
		/// <value>The breath out pressure minimum.</value>
		public float ExhalePressureMin { get { return ExhalePressureMinProperty.value; } }
		public Property<float> ExhalePressureMinProperty;
		/// <summary>
		/// Gets the breath out pressure max.
		/// The back pressure maximum is 20 cm of water
		/// </summary>
		/// <value>The breath out pressure max.</value>
		public float ExhalePressureMax { get { return ExhalePressureMaxProperty.value; } }
		public Property<float> ExhalePressureMaxProperty;
		/// <summary>
		/// (float)Gets the maximum breath out pressure to track.
		/// The default is 30 cm of water
		/// </summary>
		public float ExhalePressureThresholdMax { get { return ExhalePressureThresholdMaxProperty.value; } }
		public Property<float> ExhalePressureThresholdMaxProperty;
		/// <summary>
		/// Gets the break time between breaths minimum in seconds.
		/// Default is 3 second
		/// </summary>
		/// <value>The break time between breaths minimum in seconds.</value>
		public float InhaleTimeMin { get { return InhaleTimeMinProperty.value; } }
		public Property<float> InhaleTimeMinProperty;
		/// <summary>
		/// Gets the break time between breaths max in seconds.
		/// Default is 4 seconds
		/// </summary>
		/// <value>The break time between breaths max in seconds.</value>
		public float InhaleTimeMax { get { return InhaleTimeMaxProperty.value; } }
		public Property<float> InhaleTimeMaxProperty;

		#region IBADataInterface implementation
		/// <summary>
		/// The exhale Pressure threshold minimum property.
		/// </summary>
		public Property<float> ExhalePressureThresholdMinProperty;
		public float ExhalePressureThresholdMin {
			get {
				return ExhalePressureThresholdMinProperty.value;
			}
		}
		/// <summary>
		/// The exhale time threshold property.
		/// </summary>
		public Property<float> ExhaleTimeThresholdProperty;
		public float ExhaleTimeThreshold {
			get {
				return ExhaleTimeThresholdProperty.value;
			}
		}
		/// <summary>
		/// The inhale time threshold property.
		/// </summary>
		public Property<float> InhaleTimeThresholdProperty;
		public float InhaleTimeThreshold {
			get {
				return InhaleTimeThresholdProperty.value;
			}
		}
		public Property<enumStatus> StatusProperty;
		/// <summary>
		/// Gets or sets the breath status.
		/// </summary>
		/// <value>The status.</value>
		public enumStatus Status {
			get {
				return StatusProperty.value;
			}
		}
		/// <summary>
		/// The breath result property.
		/// </summary>
		public Property<BAResult> BreathIsGoodProperty;
		public BAResult BreathIsGood {
			get {
				return BreathIsGoodProperty.value;
			}
		}
		/// <summary>
		/// The repeat result property.
		/// </summary>
		public Property<BAResult> SetIsGoodProperty;
		public BAResult SetIsGood {
			get {
				return SetIsGoodProperty.value;
			}
		}
		/// <summary>
		/// The theraphy result property.
		/// </summary>
		public Property<BAResult> SessionIsGoodProperty;
		public BAResult SessionIsGood {
			get {
				return SessionIsGoodProperty.value;
			}
		}
		/// <summary>
		/// The breath count property.
		/// </summary>
		public Property<int> BreathCountProperty;
		public int BreathCount {
			get {
				return BreathCountProperty.value;
			}
		}
		/// <summary>
		/// The good breath count property.
		/// </summary>
		public Property<int> BreathCountGoodProperty;
		public int BreathCountGood {
			get {
				return BreathCountGoodProperty.value;
			}
		}
		/// <summary>
		/// The repeat count property.
		/// </summary>
		public Property<int> SetCountProperty;
		public int SetCount {
			get {
				return SetCountProperty.value;
			}
		}
		public Property<int> SetCountGoodProperty;
		public int SetCountGood {
			get {
				return BreathCountGoodProperty.value;
			}
		}

		#endregion
		private float timeAt0 = 0F, timeAt1 = 0F, rTime = 0;
		/// <summary>
		/// Reads the data.
		/// </summary>
		/// <returns>The data.</returns>
		public float ReadData ()
		{
			#if UNITY_EDITOR
			if (master.inputType == MasterGenerator.InputTypes.Keys) {
				if (Input.GetKey (KeyCode.Alpha3)) {
						return Random.Range (ExhalePressureMax + 0.5f, ExhalePressureMax + (ExhalePressureMax - ExhalePressureMin));
				} else if (Input.GetKey (KeyCode.Alpha2)) {
						return Random.Range (ExhalePressureMin, ExhalePressureMax);
				} else if (Input.GetKey (KeyCode.Alpha1)) {
						return Random.Range (1f, ExhalePressureMin - 0.5f);
				}
			} else if (master.inputType == MasterGenerator.InputTypes.MouseScroll) {
				float inputValue = Input.GetAxis ("Mouse ScrollWheel");
				if (inputValue > 0) {
						scrollValue = Mathf.Clamp (scrollValue + ((ExhalePressureMax - ExhalePressureMin) * 0.25f), 0f, ExhalePressureThresholdMax);
				} else if (inputValue < 0) {
					scrollValue = Mathf.Clamp (scrollValue - ((ExhalePressureMax - ExhalePressureMin) * 0.25f), 0f, ExhalePressureThresholdMax);
				}

				return scrollValue;
			} else if (master.inputType == MasterGenerator.InputTypes.Automatic){
				if (rTime == 0)
					rTime = UnityEngine.Random.Range(5, 12);
				if (timeAt0 <= rTime) {
					timeAt0 += Time.deltaTime;
					timeAt1 = 0;
					return ((PEPDataGenerator)_generator).getBreathValue();
				}
				else
				{
					if (timeAt1 < InhaleTimeThreshold + 0.5f){
						timeAt1 += Time.deltaTime;
						return 0;
					}
					else
					{
						timeAt0 = 0;
						rTime = 0;
					}
				}
			}
			#elif UNITY_ANDROID
			if (GameMode.gameMode == GameMode.GameModes.DebugMode) {	
					if (Input.touchCount > 2) {
							return Random.Range (ExhalePressureMax + 0.5f, ExhalePressureMax + (ExhalePressureMax - ExhalePressureMin));
					} 
					if (Input.touchCount > 1) {
							return Random.Range (ExhalePressureMin, ExhalePressureMax);
					} 
					if (Input.touchCount > 0) {
							return Random.Range (1f, ExhalePressureMin - 0.5f);
					}
			
			} else {
				return _generator.ReadData();;
			}
			#endif
			return 0;
		}
	}
}