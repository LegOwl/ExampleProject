using System;
using System.Collections.Generic;
using UnityEngine;

namespace Collections
{
    public enum ParticleType
    {
        Example = 0,
        NumberPopup = 1,
    }
    [CreateAssetMenu(fileName = "ParticleCollection", menuName = "ParticleCollection")]
    public class ParticleCollection : ScriptableObject
    {
        [SerializeField] public List<Particle> particles = new List<Particle>();

        [Serializable]
        public class Particle
        {
            public ParticleType particleType;
            public Transform prefab;
        }
    }

}
