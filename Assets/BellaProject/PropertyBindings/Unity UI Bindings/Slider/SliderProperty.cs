#if UNITY_4_6
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bindings
{
		public class SliderProperty : Property<float>
		{
				Slider _slider;
				public SliderProperty (Slider slider)
				{
						_slider = slider;
						_value = slider.value;
						slider.onValueChanged.AddListener (SetValue);
						_type = typeof(float);
						
				}


				protected override void Set (float value)
				{
						base.Set (value);
						_slider.value = value;
				}
				
		}
}
#endif