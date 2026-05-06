using Collections;
using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "PresetStaticData", menuName = "ScriptableObjects/StaticData/PresetStaticData")]
    public class PresetStaticData : ScriptableObject
    {
        [field: SerializeField] public ParticleCollection particleCollection;
    }

}
