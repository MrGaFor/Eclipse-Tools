using Sirenix.OdinInspector;
using UnityEngine;

namespace GIB.Toolkit
{
	/// <summary>
	/// Objects with this component will not be destroyed when new scenes are loaded.
	/// </summary>
	
	public class DontDestroyOnLoad : MonoBehaviour
	{
		[InfoBox("Objects with this component will not be destroyed when new scenes are loaded.")]
		public bool enable = true;
		void Awake()
		{
			if(enable)
				DontDestroyOnLoad(this);
		}
	}
}
