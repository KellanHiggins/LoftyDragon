using UnityEngine;
using System.Collections;
namespace BellaProject
{
	/*
			This class stores all the binding names for all the parameters used in MasterGenerator.
			
			If you need to access a parameter only once then you can get in by going Binding<T>.GetBinding(stringName).value
			where T is the type of the variable shown in parantheses in the summary of each parameter
			and stringName is one of the strings below.
			
			If you would like an up to date reference to a value then you should create a property like below
				Ex: Property<T> property = new Property<T>(value);
			again substituing the T for for type, and value a default value, since the value will change when you bind the property, the value doesn't matter much, so just give it anything.
			After creating the property, you need to bind it so that it will get the value of the parameter to do this follow the example below
				Ex: property.AddToBinding(stringName,BindingDirection.BindingToProperty,AssignmentOnAdd.TakeBindingValue);
			Substitute stringName for one of the parameters strings below and leave the rest as is, Note that this will make the propert take the binding it is attaching itself to's value,
			and the property will only ever take new values from the binding. We want the property to take the parameter's value and never be able to change it.
			If we used BindingDirection.BiDirectional, everytime our created property value changed then it would change the binding's value aswell,
			and it's best to leave all the parameter value's alone.
			
			If you want to be notified whenever a parameter value changes, then first create a property as described above, and then add this line
			Ex: property.AddListener(method);
			Just substitute method with the void method that has a single parameter of the same type as the property, 
			
			Below is a fully working example, showing all of the ways explained above
			Ex: 
			Property<float> property;
			float maxPressureValue;
			void Awake()
			{
			maxPressureValue = Binding<float>.GetBinding(ParameterBindingNames.ExhalePressureMax).value;
			property = new Property<float>(0);
			property.AddToBinding(BreathStrength,BindingDirection.BindingToProperty,AssignmentOnAdd.TakeBindingValue);
			property.AddListener(OnNewValue);
			}
			
			void OnNewValue(float value)
			{
				//do something with value
			}
 		*/
		public static class ParameterBindingNames
		{ 		
		/// <summary>
		/// (float)The current breath strength in cm.
		/// </summary>
		public const string	BreathStrength = "BreathStrength";
		/// <summary>
		/// (float)The length of the currrent breath. Will be zero if there is no breath
		/// </summary>
		public const string BreathLength = "BreathLength";
		// static variable
		public const string ExhalePressureMin = "ExhalePressureMin";
		public const string ExhalePressureMax = "ExhalePressureMax";
		public const string ExhalePressureThresholdMin = "ExhalePressureThresholdMin";
		public const string ExhalePressureThresholdMin2 = "ExhalePressureThresholdMin2";
		public const string ExhalePressureThresholdMax = "ExhalePressureThresholdMax";
		public const string ExhaleTimeThreshold = "ExhaleTimeThreshold";
		public const string ExhaleTimeMin = "ExhaleTimeMin";
		public const string ExhaleTimeMax = "ExhaleTimeMax";
		public const string InhalePressureThreshold = "InhalePressureThreshold";
		public const string InhaleTimeThreshold = "InhaleTimeThreshold";
		public const string InhaleTimeMin = "InhaleTimeMin";
		public const string InhaleTimeMax = "InhaleTimeMax";
		public const string RestTimeMin = "RestTimeMin";
		public const string RestTimeMax = "RestTimeMax";
		public const string BreathsMin = "BreathsMin";
		public const string BreathsTarget = "BreathsTarget";
		public const string BreathsMax = "BreathsMax";
		public const string SetsMin = "SetsMin";
		public const string SetsTarget = "SetsTarget";
		public const string SetsMax = "SetsMax";
		public const string TimeSensitivity = "TimeSensitivity";
		public const string LastSessionDateTime = "LastSessionDateTime";
		public const string LastSessionSuccessLevel = "LastSessionSuccessLevel";
		public const string SessionCount = "SessionCount";
		public const string SessionSuccessLevelAvg = "SessionSuccessLevelAvg";
		// Dynamic variables
		public const string Status = "Status";
		public const string BreathCount = "BreathCount";
		public const string BreathCountGood = "BreathCountGood";
		public const string BreathIsGood = "BreathIsGood";
		public const string SetCount = "SetCount";
		public const string SetCountGood = "SetCountGood";
		public const string SetIsGood = "SetIsGood";
		public const string SessionIsGood = "SessionIsGood";
		public const string BreathValue = "PEPData";
		// Events
		public const string ExhaleStart = "ExhaleStart";
		public const string ExhaleEnd = "ExhaleEnd";
		public const string GoodExhale = "GoodExhale";
		public const string BreathEnd = "BreathEnd";
		public const string SetEnd = "SetEnd";
		public const string SessionEnd = "SessionEnd";
		public const string StatusChanged = "StatusChanged";

		public const string SessionTimeMin = "SessionTimeMin";
		public const string Breaths = "Breaths";
		public const string Sets = "Sets";
		public const string ExhalePressureMaxToTrack = "ExhalePressureMaxToTrack";
	}
}