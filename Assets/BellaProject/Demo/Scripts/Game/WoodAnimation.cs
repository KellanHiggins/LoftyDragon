using UnityEngine;
using System.Collections;
using BellaProject;
public class WoodAnimation : MonoBehaviour
{

		public Animator animator;
		
		void Start ()
		{
				Manager.messenger.Subscribe (BellaMessages.GoodBreath, OnMessage);
				Manager.messenger.Subscribe (BellaMessages.WeakBreath, OnMessage);
				Manager.messenger.Subscribe (BellaMessages.StrongBreath, OnMessage);
				Manager.messenger.Subscribe (BellaMessages.ReadyForInput, OnMessage);
				Manager.messenger.Subscribe (BellaMessages.BreakTimeStarted, OnMessage);
				Manager.messenger.Subscribe (BellaMessages.BreakTimeMinReached, OnMessage);	
		}
		void OnDestroy ()
		{
				Manager.messenger.Unsubscribe (BellaMessages.GoodBreath, OnMessage);
				Manager.messenger.Unsubscribe (BellaMessages.WeakBreath, OnMessage);
				Manager.messenger.Unsubscribe (BellaMessages.StrongBreath, OnMessage);
				Manager.messenger.Unsubscribe (BellaMessages.ReadyForInput, OnMessage);
				Manager.messenger.Unsubscribe (BellaMessages.BreakTimeStarted, OnMessage);
				Manager.messenger.Unsubscribe (BellaMessages.BreakTimeMinReached, OnMessage);
		}
		void OnMessage (Object sender, string msgID, float num1 = 0f, float num2 = 0f, float num3 = 0f, float num4 = 0f)
		{
				animator.ResetTrigger ("Return");
				
				switch (msgID) {
					case BellaMessages.WeakBreath:
						animator.ResetTrigger ("Too Weak");
						animator.SetTrigger ("Too Weak");
						break;
					case BellaMessages.GoodBreath:
						animator.ResetTrigger ("Good");
						animator.SetTrigger ("Good");
						break;
					case BellaMessages.StrongBreath:
						animator.ResetTrigger ("Too Strong");
						animator.SetTrigger ("Too Strong");
						break;
					case BellaMessages.ReadyForInput:
						animator.SetTrigger ("Return");
						break;
					case BellaMessages.BreakTimeStarted:
						break;
					case BellaMessages.BreakTimeMinReached:
						break;
				}
		}
}
