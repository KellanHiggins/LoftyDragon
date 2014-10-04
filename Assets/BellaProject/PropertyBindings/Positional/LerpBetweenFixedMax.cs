using UnityEngine;
using System.Collections;
namespace Bindings
{
		public class LerpBetweenFixedMax : LerpBetween
		{

				[SerializeField]
				private float
						maxValue = 0f;
				protected override float GetMaxValue ()
				{
						return maxValue;
				}
		}
}