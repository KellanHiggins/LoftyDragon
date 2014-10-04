using UnityEngine;
using System.Collections;
/// <summary>
/// Input modes.
/// </summary>
public enum InputModes
{
	Automatic,
	KeyInput,
	Slider	
}
//---- status enum
public enum enumStatus
{
	Initializing =-1,
	Ready = 0,
	Inhale = 1,
	Exhale = 2,
	Rest = 3,
	Finish = 4,
	Pause = 5
}
/// <summary>
/// Breath event.
/// </summary>
public enum BAEvent
{
	ExhaleStart = 0,
	ExhaleEnd = 1,
	GoodExhale,
	BreathEnd,
	SetEnd,
	SessionEnd
}
/// <summary>
/// Result.
/// </summary>
public enum BAResult
{
	None = 0,
	Yes = 1,
	No
}
//------------------------------------------------------------------------------
//     Data Interface
//------------------------------------------------------------------------------
public interface IBADataInterface
{
	/// <summary>
	/// Gets the breath out pressure minimum.
	/// Lower value of the pressure range within which the patient must hold an 
	/// exhalation breath for 3-4 seconds.  If the pressure drops below this value 
	/// for longer than the TimeSensitivity then the breath is not good and must be repeated.
	/// </summary>
	/// <value>The breath out pressure minimum.</value>
	float ExhalePressureMin { get; }
	/// <summary>
	/// Gets the breath out pressure max.
	/// Upper value of the pressure range within which the patient must hold an 
	/// exhalation breath for 3-4 seconds.  If the pressure rises above this value 
	/// for longer than the TimeSensitivity then the breath is not good and must be repeated.
	/// </summary>
	/// <value>The breath out pressure max.</value>
	float ExhalePressureMax { get; }
	/// <summary>
	/// Gets the exhale Pressure threshold minimum.
	/// Pressure threshold above which the breath is considered to have started.  
	/// Pressure fluctuations below this level are ignored as noise and only when the pressure 
	/// passes this level is a breath considered to have started.  
	/// Pressure below this level for longer than the TimeSensitivity is the definition of the end of a breath.  
	/// When displaying a graph of the pressure, this is the minimum display value.
	/// </summary>
	/// <value>The exhale Pressure threshold minimum.</value>
	float ExhalePressureThresholdMin { get;}
	/// <summary>
	/// Gets the exhale pressure threshold min2.
	/// Same as ExhalePresureThresholdMin but used for second and subsequent breaths of a set.  
	/// There is some thinking that if the mask is kept tight on the face and no pressure is lost 
	/// subsequent breaths start at a non-zero pressure level.  
	/// So this parameter allows the exhale pressure threshold to be set higher for later breaths if necessary.
	/// </summary>
	/// <value>The exhale pressure threshold min2.</value>
	float ExhalePressureThresholdMin2 { get;}
	/// <summary>
	/// Gets the exhale Pressure threshold max.
	/// Pressure threshold above which the pressure stops being measured.  
	/// This is a limit level above which the pressure stops being measured and instead the maximum value is shown.  
	/// When displaying a graph of the pressure, this is the maximum display value.
	/// </summary>
	/// <value>The exhale Pressure threshold max.</value>
	float ExhalePressureThresholdMax { get;}
	/// <summary>
	/// Gets the therapy minimum time in minutes. 
	/// PEP therapy should take at least 20 minutes
	/// </summary>
	/// <value>The therapy time in minutes.</value>
	float SessionTimeMin { get; }
	/// <summary>
	/// The minimum rest time between completed sets before the next 
	/// set can be started.  A breath made before this time counts as a breath 
	/// for the prior set.  A breath made after this time counts as a breath in the new set.
	/// Default is 1 minute
	/// </summary>
	/// <value>The break time minimum in minutes.</value>
	float RestTimeMin { get; }
	/// <summary>
	/// The recommended maximum rest time between completed sets before the next set can be started.  
	/// There is no consequence for taking longer than this time to start the next set, but the game 
	/// could prompt the user more emphatically at this time period to start their next set.
	/// Default is 2 minutes
	/// </summary>
	/// <value>The break time max in minutes.</value>
	float RestTimeMax { get; }
	/// <summary>
	/// The number of breaths in a set.  
	/// A set is considered complete when this number of valid breaths is completed.  
	/// There are no consequences for completing more breaths in a set than this value.
	/// Default: repeat for 15 breaths, breathing at a normal resporatory rate.
	/// </summary>
	/// <value>The repeat breaths per step.</value>
	int Breaths { get; }
	/// <summary>
	/// Gets the breaths minimum.
	/// The minimum number of breaths in a set.  
	/// A set is not considered complete until this number of breaths is completed.
	/// </summary>
	/// <value>The breaths minimum.</value>
	int BreathsMin { get;}
	/// <summary>
	/// Gets the breaths target.
	/// The target number of breaths in a set.  
	/// There are no consequences for completing more breaths in a set than this value.
	/// </summary>
	/// <value>The breaths target.</value>
	int BreathsTarget { get;}
	/// <summary>
	/// Gets the breaths max.
	/// The maximum number of breaths in a set.  If the user has completed this number of 
	/// breaths the set is considered complete no matter how many of the breaths were good.
	/// </summary>
	/// <value>The breaths max.</value>
	int BreathsMax { get;}
	/// <summary>
	/// The number of sets in a session.  
	/// A session is considered complete when this number of valid sets is completed.  T
	/// here game should indicate completion of the therapy session when this number of valid sets is complete.
	/// Repeat the set for 6 times per session
	/// </summary>
	/// <value>The repeat steps per therapy.</value>
	int Sets { get; }
	/// <summary>
	/// Gets the sets minimum.
	/// The minimum number of sets in a session.  
	/// A session is considered complete when this number of valid sets is completed.  
	/// The game should indicate completion of the therapy session when this number of valid sets is complete.
	/// </summary>
	/// <value>The sets minimum.</value>
	int SetsMin { get;}
	/// <summary>
	/// Gets the sets target.
	/// Same as SetsMin.  Parameter added for future considerations.
	/// </summary>
	/// <value>The sets target.</value>
	int SetsTarget {get;}
	/// <summary>
	/// Gets the sets max.
	/// Same as SetsMin.  Parameter added for future considerations.
	/// </summary>
	/// <value>The sets max.</value>
	int SetsMax { get;}
	/// <summary>
	/// The minimum time which an exhalation must be held to be a valid breath.  
	/// If the exhale pressure drops below the ExhalePressureMin pressure or above the ExhalePressureMax 
	/// pressure before this time then it is not a valid breath and must be repeated.
	/// Default is 3 seconds
	/// </summary>
	/// <value>The breath out minimum time in seconds.</value>
	float ExhaleTimeMin { get; }
	/// <summary>
	/// The recommended maximum time which an exhalation breath should be held.  
	/// However, a breath is still considered 
	/// good if it is held longer than this recommend time threshold.
	/// Default is 4 seconds
	/// </summary>
	/// <value>The breath out max time in seconds.</value>
	float ExhaleTimeMax { get; }
	/// <summary>
	/// Gets the exhale time threshold.
	/// The minimum time over which pressure above ExhalePressureThresholdMin must be held to 
	/// be considered the exhale part of a breath.  
	/// If the pressure drops below ExhalePressureThresholdMin in less time it is considered noise 
	/// and no counters are changed.
	/// </summary>
	/// <value>The exhale time threshold.</value>
	float ExhaleTimeThreshold { get; }
	/// <summary>
	/// The recommended minimum time of an inhalation.  
	/// There is no impact on the success of a breath for inhalation periods shorter than this level.
	/// Default is 3 second
	/// </summary>
	/// <value>The break time between breaths minimum in seconds.</value>
	float InhaleTimeMin { get; }
	/// <summary>
	/// The maximum time duration that is considered an inhalation.  
	/// If the pressure remains below ExhalePressureThresholdMin for longer than this duration 
	/// then the status changes from Inhale to Rest.  
	/// There is no impact on the success of a breath for inhalation periods longer than this level.
	/// Default is 4 seconds
	/// </summary>
	/// <value>The break time between breaths max in seconds.</value>
	float InhaleTimeMax { get; }
	/// <summary>
	/// Gets the inhale time threshold.
	/// The minimum time over which pressure below ExhalePressureThresholdMin must be held to 
	/// be considered the inhale part of a breath.  
	/// If the pressure rises above ExhalePressureThresholdMin in less time it is considered part 
	/// of the prior exhale.
	/// </summary>
	/// <value>The inhale time threshold.</value>
	float InhaleTimeThreshold {get;}
	/// <summary>
	/// Gets the inhale presure threshold.
	/// Pressure below this level for longer than the TimeSensitivity is the definition of the 
	/// end of an exhale and the start of an inhale.
	/// </summary>
	/// <value>The inhale presure threshold.</value>
	float InhalePresureThreshold { get;}
	/// <summary>
	/// This public variable contains the current status.  The enumerated values are:
	/// 1-inhale:  the inhalation phase of a breath
	///	2-exhale:  the exhalation phase of a breath
	///		3-rest:  the rest period between sets
	///		4-finished:  the session is complete
	///		5-pause :  the session is paused (the user can pause at any time)
	/// </summary>
	/// <value>The status.</value>
	enumStatus Status { get; }
	/// <summary>
	/// Gets or sets the breath result.
	/// </summary>
	/// <value>The breath result.</value>
	BAResult BreathIsGood { get; }
	/// <summary>
	/// Gets or sets the repeat result.
	/// </summary>
	/// <value>The repeat result.</value>
	BAResult SetIsGood {get;}
	/// <summary>
	/// Gets or sets the theraphy result.
	/// </summary>
	/// <value>The theraphy result.</value>
	BAResult SessionIsGood {get;}
	/// <summary>
	/// Gets or sets the breath count.
	/// Count of breaths in this set.  Counter is reset at the beginning of a set.
	/// </summary>
	/// <value>The breath count.</value>
	int BreathCount { get; }
	/// <summary>
	/// Gets or sets the good breath count.
	/// Count of good breaths in this set.  Counter is reset at the beginning of a set.
	/// </summary>
	/// <value>The good breath count.</value>
	int BreathCountGood { get;}
	/// <summary>
	/// Gets or sets the repeat count.
	/// Count of sets in this session.  Counter is reset at the beginning of a session.
	/// </summary>
	/// <value>The repeat count.</value>
	int SetCount { get; }
	/// <summary>
	/// Gets or sets the good set count.
	/// Count of good sets in this session.  Counter is reset at the beginning of a session.
	/// </summary>
	/// <value>The good repeat count.</value>
	int SetCountGood { get; }
	/// <summary>
	/// This call returns the current pressure value in cm H2O.  The value is between 0 and ExhalePressureThresholdMax.
	/// </summary>
	/// <returns>The data.</returns>
	float ReadData ();
	/// <summary>
	/// Gets the time sensitivity.
	/// The time sensitivity below which no attention is paid to swings in the exhalation pressure.  
	/// That is, pressure spikes up or down shorter than this duration are ignored.
	/// Default is 0.1 sec
	/// </summary>
	/// <value>The time sensitivity.</value>
	float TimeSensitivity { get;}
	/// <summary>
	/// Gets the last session date time.
	/// The date and time of completion of the last therapy session with a success level of at least 50%
	/// </summary>
	/// <value>The last session date time.</value>
	System.DateTime LastSessionDateTime { get;}
	/// <summary>
	/// Gets the last session success level.
	/// The success level in percent of the last session.
	/// </summary>
	/// <value>The last session success level.</value>
	float LastSessionSuccessLevel { get; }
	/// <summary>
	/// Gets the session count.
	/// A counter of sessions completed (since last reset) where the success level was at least 50%.
	/// </summary>
	/// <value>The session count.</value>
	int SessionCount { get;}
	/// <summary>
	/// Gets the session success level avg.
	/// The average success level of all sessions completed (since last reset).
	/// </summary>
	/// <value>The session success level avg.</value>
	float SessionSuccessLevelAvg { get;}
	/// <summary>
	/// Gets the exhale start.
	/// The exhale phase of a breath has started.  Note that it is possible to have several 
	/// false starts on the start of an exhale phase since the exhale starts when the 
	/// ExhalePressureThresholdMin pressure threshold is crossed, but could fail to stay 
	/// above that threshold for the ExhaleTimeThreshold time period required to complete 
	/// the exhale phase of a breath.
	/// </summary>
	/// <value>The exhale start.</value>
	BAResult ExhaleStart { get;}
	/// <summary>
	/// Gets the exhale end.
	/// </summary>
	/// <value>The exhale end.</value>
	BAResult ExhaleEnd { get; }
	/// <summary>
	/// Gets the good exhale.
	/// </summary>
	/// <value>The good exhale.</value>
	BAResult GoodExhale { get;}
	/// <summary>
	/// Gets the breath end.
	/// </summary>
	/// <value>The breath end.</value>
	BAResult BreathEnd { get;}
	/// <summary>
	/// Gets the set end.
	/// </summary>
	/// <value>The set end.</value>
	BAResult SetEnd { get;}
	/// <summary>
	/// Gets the session end.
	/// </summary>
	/// <value>The session end.</value>
	BAResult SessionEnd { get;}
	/// <summary>
	/// Gets the status changed.
	/// </summary>
	/// <value>The status changed.</value>
	BAResult StatusChanged { get;}
}
