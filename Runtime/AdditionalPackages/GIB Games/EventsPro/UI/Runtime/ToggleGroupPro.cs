using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Toggle Group Pro", 32)]
	[DisallowMultipleComponent]
	public class ToggleGroupPro : UIBehaviour
	{
		[SerializeField] private bool m_AllowSwitchOff = false;
		public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

		private List<TogglePro> m_Toggles = new List<TogglePro>();

		protected ToggleGroupPro() { }

		private void ValidateToggleIsInGroup(TogglePro toggle)
		{
			if (toggle == null || !m_Toggles.Contains(toggle))
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
		}

		public void NotifyToggleOn(TogglePro toggle)
		{
			ValidateToggleIsInGroup(toggle);

			// disable all toggles in the group
			for (var i = 0; i < m_Toggles.Count; i++)
			{
				if (m_Toggles[i] == toggle)
					continue;

				m_Toggles[i].isOn = false;
			}
		}

		public void UnregisterToggle(TogglePro toggle)
		{
			if (m_Toggles.Contains(toggle))
				m_Toggles.Remove(toggle);
		}

		public void RegisterToggle(TogglePro toggle)
		{
			if (!m_Toggles.Contains(toggle))
				m_Toggles.Add(toggle);
		}

		public bool AnyTogglesOn()
		{
			return m_Toggles.Find(x => x.isOn) != null;
		}

		public IEnumerable<TogglePro> ActiveToggles()
		{
			return m_Toggles.Where(x => x.isOn);
		}

		public void SetAllTogglesOff()
		{
			bool oldAllowSwitchOff = m_AllowSwitchOff;
			m_AllowSwitchOff = true;

			for (var i = 0; i < m_Toggles.Count; i++)
				m_Toggles[i].isOn = false;

			m_AllowSwitchOff = oldAllowSwitchOff;
		}
	}
}