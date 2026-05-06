using System;
using UnityEngine;
using Lean.Pool;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Collections;
using Cysharp.Threading.Tasks;
using Extensions;

namespace Services
{
    public class ParticleService : IService
    {
        private Dictionary<ParticleType, Transform> _particlePrefabs = new Dictionary<ParticleType, Transform>();
        private Dictionary<ParticleType, float> _particleDurationCache = new Dictionary<ParticleType, float>();
        private readonly HashSet<Transform> _activeParticles = new(1000);
        private CancellationTokenSource _cts;

        public async UniTask Initialize(List<ParticleCollection.Particle> particles)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            _particlePrefabs.Clear();
            foreach (var particle in particles)
            {
                if (particle.prefab != null)
                    _particlePrefabs[particle.particleType] = particle.prefab;
            }
            
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken: _cts.Token);
        }
        
        public Transform PlayParticle(
            ParticleType type,
            Vector3 position,
            Quaternion rotation,
            float? customDuration = null,
            Transform parent = null)
        {
            if (!_particlePrefabs.TryGetValue(type, out var prefab) || prefab == null)
            {
                DebugLogger.LogWarning($"No particle prefab found for type: {type}");
                return null;
            }

            if (parent != null && parent.Equals(null))
                parent = null;

            var effect = LeanPool.Spawn(prefab, position, rotation, parent);
            if (effect == null)
                return null;

            _activeParticles.Add(effect);

            float duration;
            if (customDuration.HasValue)
            {
                duration = customDuration.Value;
            }
            else if (!_particleDurationCache.TryGetValue(type, out duration))
            {
                var ps = prefab.GetComponent<ParticleSystem>();
                duration = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 0.5f;
                _particleDurationCache[type] = duration;
            }

            LeanPool.Despawn(effect, duration);
            _ = RemoveAfterDelay(effect, duration);

            return effect;
        }

        public async Task PlayParticleSequence(
            List<(ParticleType type, Vector3 pos, Quaternion rot, Transform parent, float? customDuration)> sequence,
            float delayBetween = 0f,
            Action onComplete = null)
        {
            foreach (var entry in sequence)
            {
                PlayParticle(entry.type, entry.pos, entry.rot, entry.customDuration, entry.parent);
                if (delayBetween > 0)
                    await Task.Delay(TimeSpan.FromSeconds(delayBetween));
            }

            onComplete?.Invoke();
        }

        public Transform PlayNumberParticle(Vector3 position, Quaternion rotation, Sprite sprite, string numberCount, float spriteScaleMultiplier = 1f, float textSize = 20f, Transform parent = null)
        {
            if (!_particlePrefabs.TryGetValue(ParticleType.NumberPopup, out var prefab) || prefab == null)
            {
                DebugLogger.LogWarning($"No prefab for {ParticleType.NumberPopup}");
                return null;
            }

            var effect = LeanPool.Spawn(prefab, position, rotation, parent);
            _activeParticles.Add(effect);

            var popup = effect.GetComponent<FloatingPopup>();
            if (popup != null)
            {
                float duration = 2f;
                popup.Setup(numberCount, sprite, duration, textSize);

                var scale = new Vector3(0.5f, 0.5f, 0.5f) * spriteScaleMultiplier;
                popup.IconTransform.localScale = scale;
                LeanPool.Despawn(effect, duration + 0.3f);
                _ = RemoveAfterDelay(effect, duration + 0.3f);
            }

            return effect;
        }

        private async UniTaskVoid RemoveAfterDelay(Transform effect, float delay)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(delay + 0.1f),
                    cancellationToken: _cts.Token
                );

                if (effect != null)
                    _activeParticles.Remove(effect);
            }
            catch (OperationCanceledException)
            {
                
            }
        }
        
        public void Cleanup()
        {
            foreach (var effect in _activeParticles)
            {
                if (effect != null)
                    LeanPool.Despawn(effect);
            }

            _activeParticles.Clear();
            _particlePrefabs.Clear();
            _particleDurationCache.Clear();
        }
    }
}