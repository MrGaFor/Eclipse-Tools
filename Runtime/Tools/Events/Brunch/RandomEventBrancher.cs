using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Brancher
{
    [HideMonoScript]
    public class RandomEventBrancher : MonoBehaviour
    {
        [SerializeField, BoxGroup(), ListDrawerSettings(ShowFoldout = false, ShowPaging = false, DraggableItems = false)] private BranchVariant[] _variants;

        public void Branch()
        {
            int totalChance = 0;
            foreach (var variant in _variants)
                totalChance += variant.Chance;
            int randomValue = Random.Range(0, totalChance);
            foreach (var variant in _variants)
            {
                if (randomValue < variant.Chance)
                {
                    variant.Invoke();
                    return;
                }
                randomValue -= variant.Chance;
            }
        }
    }

    [System.Serializable]
    public struct BranchVariant
    {
        [SerializeField, LabelWidth(70), Range(1, 100)] private int _chance;
        [FoldoutGroup("Events"), SerializeField] private UnityEvent _isThis;

        public int Chance => _chance;
        public void Invoke() => _isThis?.Invoke();
    }
}
