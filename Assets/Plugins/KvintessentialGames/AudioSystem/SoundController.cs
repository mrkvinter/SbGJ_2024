using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KvinterGames
{
    public class SoundController : MonoSingleton<SoundController>
    {
        private const int MAX_SOUNDS = 20;

        [SerializeField] private Sound[] sounds;

        private readonly List<AudioSource> soundAudioSources = new();
        private int currentSourceIndex;
        private readonly Dictionary<string, Sound> soundDictionary = new();

        protected override void OnInitialize()
        {
            foreach (var sound in sounds)
            {
                soundDictionary[sound.Name] = sound;
            }

            for (int i = 0; i < MAX_SOUNDS; i++)
            {
                var go = new GameObject($"AudioSource{i}");
                go.transform.SetParent(transform);
                var audioSource = go.AddComponent<AudioSource>();
                soundAudioSources.Add(audioSource);
            }
        }

        public AudioClip PlaySound(AudioClip clip, float pitchRandomness = 0, float volume = 1,
            bool fadeOut = false, float pitch = 1)
        {
            var audioSource = GetFreeAudioSource();
            audioSource.volume = volume;
            audioSource.pitch = pitch + Random.Range(-pitchRandomness, pitchRandomness);
            audioSource.clip = clip;
            audioSource.PlayOneShot(clip);
            if (fadeOut)
            {
                audioSource.DOFade(0, 1)
                    .SetDelay(clip.length - 1);
            }
            return clip;
        }

        public AudioClip PlaySound(string soundName, float pitchRandomness = 0, float volume = 1,
            bool fadeOut = false, float pitch = 1)
        {
            if (soundDictionary.TryGetValue(soundName, out var soundInfo))
            {
                var audioSource = GetFreeAudioSource();
                audioSource.volume = volume;
                audioSource.pitch = pitch + Random.Range(-pitchRandomness, pitchRandomness);
                var clipIndex = Random.Range(0, soundInfo.Clips.Length);
                var clip = soundInfo.Clips[clipIndex];
                audioSource.clip = clip;
                audioSource.PlayOneShot(clip);
                if (fadeOut)
                {
                    audioSource.DOFade(0, 1)
                        .SetDelay(clip.length - 1);
                }
                return clip;
            }
            else
            {
                Debug.LogError($"Sound with name {soundName} not found");
                return null;
            }
        }

        private AudioSource GetFreeAudioSource()
        {
            var audioSource = soundAudioSources[currentSourceIndex];
            currentSourceIndex = (currentSourceIndex + 1) % soundAudioSources.Count;
            return audioSource;
        }

        public LoopSound PlayLoop(AudioClip clip)
        {
            var audioSource = new GameObject("LoopAudioSource").AddComponent<AudioSource>();
            audioSource.transform.SetParent(transform);
            return new LoopSound(audioSource, clip);
        }
        
        public LoopSound PlayLoop(string soundName)
        {
            if (soundDictionary.TryGetValue(soundName, out var soundInfo))
            {
                var audioSource = new GameObject("LoopAudioSource").AddComponent<AudioSource>();
                audioSource.transform.SetParent(transform);
                var clipIndex = Random.Range(0, soundInfo.Clips.Length);
                var clip = soundInfo.Clips[clipIndex];
                return new LoopSound(audioSource, clip);
            }
            else
            {
                Debug.LogError($"Sound with name {soundName} not found");
                return null;
            }
        }
        
        public class LoopSound : IDisposable
        {
            private AudioSource audioSource;

            public AudioSource AudioSource => audioSource;

            public LoopSound(AudioSource audioSource, AudioClip clip)
            {
                this.audioSource = audioSource;
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }

            public void Stop(float fadeOutTime = 0)
            {
                if (fadeOutTime > 0)
                {
                    audioSource.DOFade(0, fadeOutTime)
                        .OnComplete(Dispose);
                }
                else
                {
                    Dispose();
                }
            }
            
            public void Pause()
            {
                audioSource.Pause();
            }
            
            public void Resume()
            {
                audioSource.UnPause();
            }

            public void Dispose()
            {
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
            }
        }
    }

    [Serializable]
    public struct Sound
    {
        public string Name;
        public AudioClip[] Clips;
    }
}