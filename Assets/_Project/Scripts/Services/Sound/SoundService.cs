using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading;
using Core;
using Saves;
using Services;

namespace Sound
{
    public class SoundService : BaseDisposable, IService
    {
        private AudioSource _audioSource;
        private AudioSource _loopedAudioSource;
        private AudioSource _backgroundSource;

        private CancellationTokenSource _bgmFadeCts;
        private CancellationTokenSource _loopFadeCts;
        private CancellationTokenSource _playlistCts;

        private float _savedBackgroundMusicVolume;
        private float _savedLoopSoundVolume;

        private string[] _playlist;
        private int _currentTrackIndex;

        public void Initialize(GlobalUserData globalUserData, GameObject soundHandler)
        {
            _audioSource = soundHandler.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.volume = 1f;
            
            _loopedAudioSource = soundHandler.AddComponent<AudioSource>();
            _loopedAudioSource.playOnAwake = false;
            _loopedAudioSource.loop = true;
            _loopedAudioSource.spatialBlend = 0f;
            _loopedAudioSource.volume = 1f;
            
            _backgroundSource = soundHandler.AddComponent<AudioSource>();
            _backgroundSource.playOnAwake = false;
            _backgroundSource.loop = true;
            _backgroundSource.spatialBlend = 0f;
            _backgroundSource.volume = 0.05f;

            _savedBackgroundMusicVolume = _backgroundSource.volume;
        }
        
        public void Play(string soundName, float volume = 1)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{soundName}");
            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: {soundName}");
                return;
            }

            _audioSource.PlayOneShot(clip, volume);
        }

        public void PlayFootstep()
        {
            int step = Random.Range(1, 5);
            Play($"Footstep_{step}");
        }

        public void PlaySuccessful()
        {
            PauseBackgroundMusic();
            Play("Successful", 0.35f);

            UniTask.Delay(TimeSpan.FromSeconds(7))
                .ContinueWith(() => ContinueBackgroundMusic())
                .Forget();
        }
        
        public void PlayBackgroundMusic(bool withFade = true)
        {
            var clip = Resources.Load<AudioClip>("Sounds/BackgroundMusic");
            if (clip == null)
            {
                Debug.LogWarning("Sound not found: BackgroundMusic");
                return;
            }

            _backgroundSource.loop = true;
            _backgroundSource.clip = clip;

            _savedBackgroundMusicVolume = 0.05f;
            _backgroundSource.volume = withFade ? 0f : _savedBackgroundMusicVolume;

            _backgroundSource.Play();

            if (withFade)
                FadeBGM(_savedBackgroundMusicVolume, 1f, false).Forget();
        }

        public void PauseBackgroundMusic(float duration = 1f)
        {
            FadeBGM(0f, duration, true).Forget();
        }

        public void ContinueBackgroundMusic(float duration = 1f)
        {
            if (!_backgroundSource.isPlaying)
                _backgroundSource.Play();

            FadeBGM(_savedBackgroundMusicVolume, duration, false).Forget();
        }

        public void StopBackgroundMusic()
        {
            _backgroundSource.Stop();
        }

        private async UniTask FadeBGM(float target, float duration, bool stopAfter)
        {
            _bgmFadeCts?.Cancel();
            _bgmFadeCts = new CancellationTokenSource();

            var token = _bgmFadeCts.Token;

            float start = _backgroundSource.volume;
            float time = 0f;

            if (target == 0f)
                _savedBackgroundMusicVolume = start;

            try
            {
                while (time < duration)
                {
                    token.ThrowIfCancellationRequested();

                    time += Time.deltaTime;
                    float t = time / duration;

                    _backgroundSource.volume = Mathf.Lerp(start, target, t);

                    await UniTask.Yield();
                }

                _backgroundSource.volume = target;

                if (stopAfter && Mathf.Approximately(target, 0f))
                    _backgroundSource.Stop();
            }
            catch (OperationCanceledException) { }
        }

        public void PlayLoop(string soundName, float volume = 1f, bool withFade = true)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{soundName}");
            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: {soundName}");
                return;
            }

            _loopedAudioSource.clip = clip;
            _savedLoopSoundVolume = volume;

            _loopedAudioSource.volume = withFade ? 0f : volume;
            _loopedAudioSource.Play();

            if (withFade)
                FadeLoop(volume, 1f, false).Forget();
        }

        public void StopLoopedSound()
        {
            FadeLoop(0f, 1f, true).Forget();
        }

        private async UniTask FadeLoop(float target, float duration, bool stopAfter)
        {
            _loopFadeCts?.Cancel();
            _loopFadeCts = new CancellationTokenSource();

            var token = _loopFadeCts.Token;

            float start = _loopedAudioSource.volume;
            float time = 0f;

            if (target == 0f)
                _savedLoopSoundVolume = start;

            try
            {
                while (time < duration)
                {
                    token.ThrowIfCancellationRequested();

                    time += Time.deltaTime;
                    float t = time / duration;

                    _loopedAudioSource.volume = Mathf.Lerp(start, target, t);

                    await UniTask.Yield();
                }

                _loopedAudioSource.volume = target;

                if (stopAfter && Mathf.Approximately(target, 0f))
                {
                    _loopedAudioSource.Stop();
                    _loopedAudioSource.clip = null;
                }
            }
            catch (OperationCanceledException) { }
        }

        public void PlayBackgroundPlaylist(string[] trackNames, bool withFade = false)
        {
            if (trackNames == null || trackNames.Length == 0)
            {
                Debug.LogWarning("Playlist is empty");
                return;
            }

            _playlist = trackNames;
            _currentTrackIndex = 0;

            _playlistCts?.Cancel();
            _playlistCts = new CancellationTokenSource();

            RunPlaylist(withFade, _playlistCts.Token).Forget();
        }

        private async UniTask RunPlaylist(bool withFade, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await PlayTrack(_playlist[_currentTrackIndex], withFade, token);

                _currentTrackIndex = (_currentTrackIndex + 1) % _playlist.Length;
            }
        }

        private async UniTask PlayTrack(string trackName, bool withFade, CancellationToken token)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{trackName}");
            if (clip == null)
            {
                Debug.LogWarning($"Track not found: {trackName}");
                return;
            }

            _backgroundSource.clip = clip;
            _backgroundSource.loop = false;

            if (withFade)
            {
                _backgroundSource.volume = 0f;
                _backgroundSource.Play();
                await FadeBGM(_savedBackgroundMusicVolume, 1f, false);
            }
            else
            {
                _backgroundSource.volume = _savedBackgroundMusicVolume;
                _backgroundSource.Play();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(clip.length), cancellationToken: token);
        }

        public void StopPlaylist()
        {
            _playlistCts?.Cancel();
            _playlistCts = null;

            StopBackgroundMusic();
        }
    }
}