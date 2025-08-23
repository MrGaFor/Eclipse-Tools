using System;
using Conversa.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
	public class CustomPort : Port
	{
		private static readonly Color FlowColor = GetColor(0, 235, 0);
        private static readonly Color SimpleMessageColor = GetColor(70, 130, 180);
        private static readonly Color CustomMessageColor = GetColor(90, 150, 200);
        private static readonly Color SimpleMassiveColor = GetColor(60, 115, 160);
        private static readonly Color SimpleChoiceColor = GetColor(40, 95, 140);
        private static readonly Color SimpleEventColor = GetColor(100, 160, 210);
        private static readonly Color IntColor = GetColor(200, 80, 80);
        private static readonly Color FloatColor = GetColor(230, 140, 50);
        private static readonly Color BoolColor = GetColor(70, 160, 70);
        private static readonly Color StringColor = GetColor(170, 90, 200);
        private static readonly Color IntPortColor = GetColor(220, 100, 100);
        private static readonly Color FloatPortColor = GetColor(250, 170, 80);
        private static readonly Color BoolPortColor = GetColor(100, 190, 100);
        private static readonly Color StringPortColor = GetColor(200, 120, 230);
        private static readonly Color LocalizationPortColor = GetColor(240, 210, 80);

        public CustomPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) :
			base(portOrientation, portDirection, portCapacity, type)
		{
			m_EdgeConnector = new EdgeConnector<Edge>(new FooConnector());

			this.AddManipulator(m_EdgeConnector);

			if (type == typeof(BaseNode))
				portColor = FlowColor;
			else if (type == typeof(bool))
				portColor = BoolPortColor;
			else if (type == typeof(int))
				portColor = IntPortColor;
			else if (type == typeof(float))
				portColor = FloatPortColor;
			else if (type == typeof(string))
				portColor = StringPortColor;
			else if (type == typeof(EC.Localization.LocalizationElement<string>))
				portColor = LocalizationPortColor;
		}
		
		static Color GetColor(int r, int g, int b) => new Color(r/255f, g/255f, b/255f);
	}
}