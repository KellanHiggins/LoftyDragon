using UnityEngine;
using System.Collections;
using Bindings;
using UnityEngine.UI;
public class ImageFillProperty : Property<float>
{
		Image _image;
		float _maxValue;
		public ImageFillProperty (Image image, float maxValue =1f)
		{
				
				_image = image;
				_value = image.fillAmount;
				_type = typeof(float);
				_maxValue = maxValue;
		
		}
	
	
		protected override void Set (float value)
		{
				base.Set (value);
				_image.fillAmount = value / _maxValue;
		}

}
