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
#if UNITY_6000_4_OR_NEWER
            MeshRenderer[] renderers = GameObject.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude);
#else
#pragma warning disable CS0618 // Type or member is obsolete
            MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>(false);
#pragma warning restore CS0618 // Type or member is obsolete
#endif

            foreach (var renderer in renderers)
            {
                if (!renderer)
                    continue;

                var mat = renderer.sharedMaterial;
                if (!mat || !mat.enableInstancing)
                    continue;

                if (renderer.GetComponent<GPUInstanceLocalDisable>())
                    continue;

                renderer.SetPropertyBlock(GetBlockForMaterial(mat));
            }

#if UNITY_EDITOR
            Debug.Log("Instanced renderers = " + renderers.Length);
#endif
        }

#endregion
    }
}