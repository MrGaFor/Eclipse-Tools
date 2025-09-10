using UnityEditor;
using System;

namespace GIB.EditorUtilities
{
	/// <summary>
	/// Launches IMGUI Debugger.
	/// </summary>
	public static class IMGUIDebugger
	{
		static Type type = Type.GetType("UnityEditor.GUIViewDebuggerWindow,UnityEditor");

		[MenuItem("Tools/GIB Toolkit/IMGUI Debugger", false)]
		public static void Open() => EditorWindow.GetWindow(type).Show();

	}
}