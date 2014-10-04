using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Bindings;
public class ColorSliderScript : MonoBehaviour
{
		[SerializeField]
		private Slider
				sliderR;
		private SliderProperty sliderRProperty;
		[SerializeField]
		private Slider
				sliderG;
		private SliderProperty sliderGProperty;
		[SerializeField]
		private Slider
				sliderB;
		private SliderProperty sliderBProperty;
		[SerializeField]
		private Slider
				sliderA;
		private SliderProperty sliderAProperty;
		public Property<Color> colorProperty;
		[SerializeField]
		private string
				nameOfBinding = "ColorBinding";
		private Binding<Color> colorBinding;
		public Color myColor;
		// Use this for initialization
		void Awake ()
		{
				
				colorBinding = new Binding<Color> (myColor, nameOfBinding);
				colorProperty = new Property<Color> (myColor);
				colorBinding.AddProperty (colorProperty, BindingDirection.BiDirectional);
				colorProperty.AddListener (UpdateColor);
				if (sliderR != null) {
						sliderRProperty = new SliderProperty (sliderR);
						sliderRProperty.AddListener (UpdateR);
						UpdateR (sliderR.value);
				}
				if (sliderG != null) {
						sliderGProperty = new SliderProperty (sliderG);
						sliderGProperty.AddListener (UpdateG);
						UpdateG (sliderG.value);
				}
				if (sliderB != null) {
						sliderBProperty = new SliderProperty (sliderB);
						sliderBProperty.AddListener (UpdateB);
						UpdateB (sliderB.value);
				}
				if (sliderA != null) {
						sliderAProperty = new SliderProperty (sliderA);
						sliderAProperty.AddListener (UpdateA);
						UpdateA (sliderA.value);
				}
				
		}
		void UpdateColor (Color color)
		{
				myColor = color;
		}
		void UpdateR (float value)
		{
				colorProperty.SetValue (new Color (value, colorProperty.value.g, colorProperty.value.b, colorProperty.value.a));
		}
		void UpdateG (float value)
		{
				colorProperty.SetValue (new Color (colorProperty.value.r, value, colorProperty.value.b, colorProperty.value.a));
		}
		void UpdateB (float value)
		{
				colorProperty.SetValue (new Color (colorProperty.value.r, colorProperty.value.g, value, colorProperty.value.a));
		}
		void UpdateA (float value)
		{
				colorProperty.SetValue (new Color (colorProperty.value.r, colorProperty.value.g, colorProperty.value.b, value));
		}
}
