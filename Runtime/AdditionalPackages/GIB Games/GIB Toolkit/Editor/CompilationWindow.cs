using UnityEditor;
using UnityEditor.Compilation;

namespace GIB.EditorUtilities
{
    /// <summary>
    /// Performs script compilation requests.
    /// </summary>
    public class CompilationWindow : EditorWindow
    {
        [MenuItem("Tools/GIB Toolkit/Request Script Compilation")]
        private static void DoRequest()
        {
            GIBUtils.Log("Requesting script compilation...");
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}