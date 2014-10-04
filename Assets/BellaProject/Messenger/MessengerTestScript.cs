using UnityEngine;
using System.Collections;

public class MessengerTestScript : MonoBehaviour
{
		bool canSend = true;
		public MessengerPrefab message;
		// Use this for initialization
		void Start ()
		{
				message.Subscribe (OnMessage);
		}
		void OnDestroy ()
		{
				message.Unubscribe (OnMessage);
		}
		// Update is called once per frame
		void Update ()
		{
				if (canSend) {
						message.Publish (null);
				}
		}
		void OnMessage (Object sender, string msgID, float num1 = 0f, float num2 = 0f, float num3 = 0f, float num4 = 0f)
		{
				Debug.Log ("Hey It works");
				canSend = false;
		}
}
