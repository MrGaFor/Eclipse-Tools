using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EC.GPU
{
    public class GPUInstance
    {
        #region Init
        public static void Init()
        {
            blocks = new();
            ReInstance();
            SceneManager.sceneLoaded += ReInstance;
        }
        #endregion

        #region Interface & Process
        private static Dictionary<Material, MaterialPropertyBlock> blocks;

        public static MaterialPropertyBlock GetBlockForMaterial(Material material)
        {
            if (!blocks.ContainsKey(material))
                blocks.Add(material, new MaterialPropertyBlock());
            return blocks[material];
        }

        public static void ReInstance(Scene scene, LoadSceneMode mode = LoadSceneMode.Single) => ReInstance();
        public static void ReInstance()
        {
            Renderer[] renderers = GameObject.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Renderer renderer in renderers)
                if (renderer && renderer.sharedMaterial)
                    renderer.SetPropertyBlock(GetBlockForMaterial(renderer.sharedMaterial));
#if UNITY_EDITOR
            Debug.Log("Instanced renderers = " + renderers.Length.ToString());
#endif
        }
        #endregion
    }
}