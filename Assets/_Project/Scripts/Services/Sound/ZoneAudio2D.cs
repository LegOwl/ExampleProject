using UnityEngine;

namespace Sound
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SphereCollider))]
    public class ZoneAudio2D : MonoBehaviour
    {
        [Header("Volume Settings")]
        [SerializeField] private float maxVolume = 1f;
        [SerializeField] private AnimationCurve volumeByDistance =
            AnimationCurve.EaseInOut(0, 1, 1, 0);

        private AudioSource _audioSource;
        private SphereCollider _trigger;
        private Transform _player;

        private bool _playerInside;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _trigger = GetComponent<SphereCollider>();

            _audioSource.volume = 0f;
            _trigger.isTrigger = true;
        }

        private void Update()
        {
            if (!_playerInside || _player == null)
                return;

            float distance = Vector3.Distance(transform.position, _player.position);
            float normalizedDistance = distance / _trigger.radius;
            normalizedDistance = Mathf.Clamp01(normalizedDistance);

            float volume = volumeByDistance.Evaluate(normalizedDistance) * maxVolume;
            _audioSource.volume = volume;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out CharacterController characterController))
                return;

            _player = characterController.transform;
            _playerInside = true;

            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out CharacterController characterController))
                return;

            _playerInside = false;
            _player = null;

            _audioSource.Stop();
            _audioSource.volume = 0f;
        }
    }
}