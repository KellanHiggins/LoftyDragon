using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using BellaProject;

public enum BAServiceFunction
{
	Reset = 0,
	Pause = 5,
	Resume = 6
}

public class PEPDataPlugin : IBADataInterface
{
	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="name">Name.</param>
	/// <param name="def">Def.</param>
	private T GetValue<T>(string name, T def)
	{
		string tmp = string.Empty;
		try
		{
			tmp = _MyActivityObject.Get<string> (name);
		}
		catch{
				}
		if (string.IsNullOrEmpty (tmp))
				return def;
		else {
			MethodInfo m = typeof(T).GetMethod("Parse", new Type[] { typeof(string) } );
			if (m != null) { 
				return (T)m.Invoke(null, new object[] { tmp }); 
			}
			return def;
		}
	}
	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="name">Name.</param>
	/// <param name="def">Def.</param>
	private BAResult GetValue(string name, BAResult def)
	{
		string tmp = string.Empty;
		try
		{
			tmp = _MyActivityObject.Get<string> (name);
		}
		catch{
		}
		if (string.IsNullOrEmpty (tmp)) {
						return def;
		} else {
			return (BAResult)(int.Parse(tmp));
		}
	}
	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="name">Name.</param>
	/// <param name="def">Def.</param>
	private enumStatus GetValue(string name, enumStatus def)
	{
		string tmp = string.Empty;
		try
		{
			tmp = _MyActivityObject.Get<string> (name);
		}
		catch{
		}
		if (string.IsNullOrEmpty (tmp)) {
			return def;
		} else {
			return (enumStatus)(int.Parse(tmp));
		}
	}
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
			return GetValue<float>(ParameterBindingNames.ExhalePressureThresholdMin, 2F);
		}
	}
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
			return GetValue<float>(ParameterBindingNames.ExhalePressureThresholdMin2, 2F);
		}
	}	
	/// <summary>
	/// Gets the inhale presure threshold.
	/// Pressure below this level for longer than the TimeSensitivity is the definition of the 
	/// end of an exhale and the start of an inhale.
	/// </summary>
	/// <value>The inhale presure threshold.</value>
	public float InhalePresureThreshold {
		get {
			return GetValue<float>(ParameterBindingNames.InhalePressureThreshold, 2F);			
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
			return GetValue<float>(ParameterBindingNames.ExhalePressureThresholdMax, 30F);
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
			return GetValue<float>(ParameterBindingNames.ExhaleTimeThreshold, 1F);			
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
			return GetValue<float>(ParameterBindingNames.InhaleTimeThreshold, 0.5F);			
		}
	}
	/// <summary>
	/// Gets or sets the breath result.
	/// </summary>
	/// <value>The breath result.</value>
	public BAResult BreathIsGood {
		get {
			return GetValue(ParameterBindingNames.BreathIsGood, BAResult.None);
		}
	}
	/// <summary>
	/// Gets or sets the repeat result.
	/// </summary>
	/// <value>The repeat result.</value>
	public BAResult SetIsGood {
		get {
			return GetValue(ParameterBindingNames.SetIsGood, BAResult.None);
		}
	}
	/// <summary>
	/// Gets or sets the theraphy result.
	/// </summary>
	/// <value>The theraphy result.</value>
	public BAResult SessionIsGood {
		get {
			return GetValue(ParameterBindingNames.SessionIsGood, BAResult.None);
		}
	}
	/// <summary>
	/// Gets or sets the breath count.
	/// Breath count number of the repeat (set)
	/// </summary>
	/// <value>The breath count.</value>
	public int BreathCount {
		get {
			return GetValue<int>(ParameterBindingNames.BreathCount, 0);
		}
	}
	/// <summary>
	/// Gets or sets the good breath count.
	/// Good breath count number of the repeat (set)
	/// </summary>
	/// <value>The good breath count.</value>
	public int BreathCountGood {
		get {
			return GetValue<int>(ParameterBindingNames.BreathCountGood, 0);
		}
	}
	/// <summary>
	/// Gets or sets the repeat count.
	/// Repeat count number of the therapy
	/// </summary>
	/// <value>The repeat count.</value>
	public int SetCount {
		get {
			return GetValue<int>(ParameterBindingNames.SetCount, 0);
		}
	}
	/// <summary>
	/// Gets or sets the good repeat count.
	/// Good repeat count number of the theraphy
	/// </summary>
	/// <value>The good repeat count.</value>
	public int SetCountGood {
		get {
			return GetValue<int>(ParameterBindingNames.SetCountGood, 0);
		}
	}
	/// <summary>
	/// Reads the data.
	/// </summary>
	/// <returns>The data.</returns>
	public float ReadData ()
	{
		return GetValue<float> (ParameterBindingNames.BreathValue, 0f);
	}
	/// <summary>
	/// Gets the breath value.
	/// </summary>
	/// <returns>The breath value.</returns>
	public float GetBreathValue ()
	{
		return UnityEngine.Random.Range (ExhalePressureMin, ExhalePressureMax);
	}
	/// <summary>
	/// Gets the therapy minimum time in minutes. 
	/// PEP therapy should take at least 20 minutes
	/// </summary>
	/// <value>The therapy time in minutes.</value>
	public float SessionTimeMin {
		get {
			return 1200F;
		}
	}
	/// <summary>
	/// Gets the break time minimum in minutes.
	/// Default is 1 minute
	/// </summary>
	/// <value>The break time minimum in minutes.</value>
	public float RestTimeMin {
		get {
			return GetValue<float>(ParameterBindingNames.RestTimeMin, 60F);
		}
	}
	/// <summary>
	/// Gets the break time max in minutes.
	/// Default is 2 minutes
	/// </summary>
	/// <value>The break time max in minutes.</value>
	public float RestTimeMax {
		get {
			return GetValue<float>(ParameterBindingNames.RestTimeMax, 120F);
		}
	}
	/// <summary>
	/// Gets the break time between breaths minimum in seconds.
	/// Default is 3 second
	/// </summary>
	/// <value>The break time between breaths minimum in seconds.</value>
	public float InhaleTimeMin {
		get {
			return GetValue<float>(ParameterBindingNames.InhaleTimeMax, 2F);
		}
	}
	/// <summary>
	/// Gets the break time between breaths max in seconds.
	/// Default is 4 seconds
	/// </summary>
	/// <value>The break time between breaths max in seconds.</value>
	public float InhaleTimeMax {
		get {
			return GetValue<float>(ParameterBindingNames.InhaleTimeMax, 5F);
		}
	}
	/// <summary>
	/// Gets the repeat breaths per step.
	/// Default: repeat for 15 breaths, breathing at a normal resporatory rate.
	/// </summary>
	/// <value>The repeat breaths per step.</value>
	public int Breaths {
		get {
			return GetValue<int>(ParameterBindingNames.BreathsTarget, 15);
		}
	}
	/// <summary>
	/// Gets the breaths minimum.
	/// </summary>
	/// <value>The breaths minimum.</value>
	public int BreathsMin {
		get {
			return GetValue<int>(ParameterBindingNames.BreathsMin, 12);
		}	}
	/// <summary>
	/// Gets the breaths target.
	/// </summary>
	/// <value>The breaths target.</value>
	public int BreathsTarget {
		get {
			return GetValue<int>(ParameterBindingNames.BreathsTarget, 15);
		}	}
	/// <summary>
	/// Gets the breaths max.
	/// </summary>
	/// <value>The breaths max.</value>
	public int BreathsMax {
		get {
			return GetValue<int>(ParameterBindingNames.BreathsMax, 20);
		}	}
	/// <summary>
	/// Gets the repeat steps per therapy.
	/// Repeat the step for 6 times per therapy
	/// </summary>
	/// <value>The repeat steps per therapy.</value>
	public int Sets {
		get {
			return GetValue<int>(ParameterBindingNames.SetsTarget, 6);
		}
	}
	public int SetsMin {
		get {
			return GetValue<int>(ParameterBindingNames.SetsMin, 4);
		}
	}
	public int SetsTarget {
		get {
			return GetValue<int>(ParameterBindingNames.SetsTarget, 6);
		}
	}
	public int SetsMax {
		get {
			return GetValue<int>(ParameterBindingNames.SetsMax, 10);
		}
	}
	/// <summary>
	/// Gets the breath out minimum time in seconds.
	/// Default is 3 seconds
	/// </summary>
	/// <value>The breath out minimum time in seconds.</value>
	public float ExhaleTimeMin {
		get {
			return GetValue<float>(ParameterBindingNames.ExhaleTimeMin, 1F);
		}
	}
	/// <summary>
	/// Gets the breath out max time in seconds.
	/// Default is 4 seconds
	/// </summary>
	/// <value>The breath out max time in seconds.</value>
	public float ExhaleTimeMax {
		get {
			return GetValue<float>(ParameterBindingNames.ExhaleTimeMax, 4F);
		}
	}
	/// <summary>
	/// Gets the breath out pressure minimum.
	/// The back pressure mininum is 10 cm of water.
	/// </summary>
	/// <value>The breath out pressure minimum.</value>
	public float ExhalePressureMin {
		get {
			return GetValue<float>(ParameterBindingNames.ExhalePressureMin, 10F);
		}
	}
	/// <summary>
	/// Gets the breath out pressure max.
	/// The back pressure maximum is 20 cm of water
	/// </summary>
	/// <value>The breath out pressure max.</value>
	public float ExhalePressureMax {
		get {
			return GetValue<float>(ParameterBindingNames.ExhalePressureMax, 20F);
		}
	}
	/// <summary>
	/// This public variable contains the current status. The enumerated values are:
	/// 1-inhale: the inhalation phase of a breath
	///  2-exhale: the exhalation phase of a breath
	///  3-rest: the rest period between sets
	///  4-finished: the session is complete
	///  5-pause : the session is paused (the user can pause at any time)
	/// </summary>
	/// <value>The status.</value>
	public enumStatus Status { 
		get {
			return GetValue(ParameterBindingNames.Status, enumStatus.Ready);
		}	
	}
	/// <summary>
	/// Gets the time sensitivity.
	/// The time sensitivity below which no attention is paid to swings in the exhalation pressure. 
	/// That is, pressure spikes up or down shorter than this duration are ignored.
	/// Default is 0.1 sec
	/// </summary>
	/// <value>The time sensitivity.</value>
	public float TimeSensitivity {
		get {
			return GetValue<float>(ParameterBindingNames.TimeSensitivity, 0.1F);			
		}
	}
	/// <summary>
	/// Gets the last session date time.
	/// The date and time of completion of the last therapy session with a success level of at least 50%
	/// </summary>
	/// <value>The last session date time.</value>
	public DateTime LastSessionDateTime {
		get {
			return GetValue<DateTime>(ParameterBindingNames.LastSessionDateTime, DateTime.MinValue);			
		}
	}
	/// <summary>
	/// Gets the last session success level.
	/// The success level in percent of the last session.
	/// </summary>
	/// <value>The last session success level.</value>
	public float LastSessionSuccessLevel {
		get {
			return GetValue<float>(ParameterBindingNames.LastSessionSuccessLevel, 0F);
		}
	}
	/// <summary>
	/// Gets the session count.
	/// A counter of sessions completed (since last reset) where the success level was at least 50%.
	/// </summary>
	/// <value>The session count.</value>
	public int SessionCount {
		get {
			return GetValue<int>(ParameterBindingNames.ExhalePressureMax, 0);
		}
	}
	/// <summary>
	/// Gets the session success level avg.
	/// The average success level of all sessions completed (since last reset).
	/// </summary>
	/// <value>The session success level avg.</value>
	public float SessionSuccessLevelAvg {
		get {
			return GetValue<float>(ParameterBindingNames.SessionSuccessLevelAvg, 0F);
		}
	}
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
			return GetValue(ParameterBindingNames.ExhaleStart, BAResult.None);
		}
	}
	/// <summary>
	/// Gets the exhale end.
	/// </summary>
	/// <value>The exhale end.</value>
	public BAResult ExhaleEnd {
		get {
			return GetValue(ParameterBindingNames.ExhaleEnd, BAResult.None);
		}
	}
	/// <summary>
	/// Gets the good exhale.
	/// </summary>
	/// <value>The good exhale.</value>
	public BAResult GoodExhale {
		get {
			return GetValue(ParameterBindingNames.GoodExhale, BAResult.None);			
		}
	}
	/// <summary>
	/// Gets the breath end.
	/// </summary>
	/// <value>The breath end.</value>
	public BAResult BreathEnd {
		get {
			return GetValue(ParameterBindingNames.BreathEnd, BAResult.None);
		}
	}
	/// <summary>
	/// Gets the set end.
	/// </summary>
	/// <value>The set end.</value>
	public BAResult SetEnd {
		get {
			return GetValue(ParameterBindingNames.SetEnd, BAResult.None);
		}
	}
	/// <summary>
	/// Gets the session end.
	/// </summary>
	/// <value>The session end.</value>
	public BAResult SessionEnd {
		get {
			return GetValue(ParameterBindingNames.SessionEnd, BAResult.None);
		}
	}
	/// <summary>
	/// Gets the status changed.
	/// </summary>
	/// <value>The status changed.</value>
	public BAResult StatusChanged {
		get {
			return GetValue(ParameterBindingNames.StatusChanged, BAResult.None);
		}
	}
	/// <summary>
	/// Reset service application
	/// </summary>
	public void Reset()
	{
		_MyActivityObject.Call<string> ("sendMessage", ((int)BAServiceFunction.Reset).ToString());
	}
	/// <summary>
	/// Pause service application.
	/// </summary>
	public void Pause()
	{
		_MyActivityObject.Call<string> ("sendMessage", ((int)BAServiceFunction.Pause).ToString());
	}
	/// <summary>
	/// Resume service application.
	/// </summary>
	public void Resume()
	{
		_MyActivityObject.Call<string> ("sendMessage", ((int)BAServiceFunction.Resume).ToString());
	}
	#endregion
	private AndroidJavaClass _ActivityClass;
	private AndroidJavaObject _ActivityObject;
	private AndroidJavaClass _MyActivityClass;
	private AndroidJavaObject _MyActivityObject;
	/// <summary>
	/// Initializes a new instance of the <see cref="PEPDataPlugin"/> class.
	/// </summary>
	public PEPDataPlugin ()
	{
			initAndroidPlugin ();
	}
	/// <summary>
	/// Inits the android plugin.
	/// </summary>
	private void initAndroidPlugin ()
	{
		try{
			_ActivityClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			_ActivityObject = _ActivityClass.GetStatic<AndroidJavaObject> ("currentActivity");
			_MyActivityObject = new AndroidJavaObject ("com.spotsolutions.bella.plugin.BellaBroadcastReceiver", _ActivityObject);
			_MyActivityObject.Call ("registerService");
			///Reset service application
			Reset();
		}
		catch(Exception ex)
		{
			throw(ex);
		}
	}
}

