#if UNITY_4_6
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Bindings;
[RequireComponent(typeof(Image))]
public class FillPropertyAttachmentFixedEndPoint : FillPropertyBaseClass
{


		[SerializeField]
		private  float
				maxValue = 1f;
		protected override float GetMaxValue ()
		{
				return maxValue;
		}
		

}
#endif