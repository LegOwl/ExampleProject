using UI;
using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "PrefabStaticData", menuName = "ScriptableObjects/StaticData/Prefabs")]
    public class PrefabStaticData : ScriptableObject
    {
        [field: SerializeField] public UIView guiViewPrefab;
        [field: SerializeField] public GameObject soundHolder;
    }

}
