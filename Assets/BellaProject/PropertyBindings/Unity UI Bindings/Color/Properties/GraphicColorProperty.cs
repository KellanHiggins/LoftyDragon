using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace Bindings
{
		public class GraphicColorProperty : Property<Color>
		{
				Graphic _graphic;
				public GraphicColorProperty (Graphic graphic)
				{
						_graphic = graphic;
						_value = graphic.color;
						_type = typeof(Color);
			
				}
		
		
				protected override void Set (Color value)
				{
						base.Set (value);
						_graphic.color = value;
				}
		}
}