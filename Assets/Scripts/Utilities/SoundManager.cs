using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    
    //This class works in a pretty special way compared to normal sound workflow,
    // It comes from https://www.youtube.com/watch?v=QL29aTa7J5Q : Simple Sound Manager (Unity Tutorial) by Code Monkey
    // If using it becomes too cumbersome, I suggest changing the sound system.
    public static class SoundManager
    {
        public enum Sound
        {
            CoinPickup,
            FMS,
            GameEnd
            
        }

        private static Dictionary<Sound, (float lastPlayed,int randIdx)> _soundTimersAndId;
        private static Dictionary<Sound, List<float>> _soundMinDelays;

        private static GameObject _oneShotGameObject;
        private static AudioSource _oneShotAudioSource;
        
        
        public static void Initialize()
        {
            _soundTimersAndId = new Dictionary<Sound, (float,int)>();
            _soundMinDelays = new Dictionary<Sound, List<float>>();


            foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClips)
            {
                
                _soundTimersAndId[soundAudioClip.sound] = (0f,0);
                if (_soundMinDelays.ContainsKey(soundAudioClip.sound))
                {
                    _soundMinDelays[soundAudioClip.sound].Add(soundAudioClip.minRepetitionDelay);
                }
                else
                {
                    _soundMinDelays[soundAudioClip.sound] = new List<float>(){soundAudioClip.minRepetitionDelay};
                }
            }
            
        }

        /// <summary>
        /// Creates a new gameObject at specified position which plays a 3d sound.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        /// <param name="position">The position of the instantiated GameObject</param>
        public static void PlaySound(Sound sound, Vector3 position)
        {
            if (CanPlaySound(sound, out var idx))
            {
                var soundGameObject = new GameObject("Sound");
                soundGameObject.transform.position = position;
                var audioSource = soundGameObject.AddComponent<AudioSource>();
                GameAssets.SoundAudioClip soundAudioClip= GetSoundAudioClip(sound, idx);
                audioSource.clip = soundAudioClip.audioClip;
                audioSource.volume = soundAudioClip.volume;
                audioSource.pitch = soundAudioClip.pitch;
                audioSource.spatialize = true;
                audioSource.spatialBlend = 1;
                if (soundAudioClip.audioMixerGroup != null)
                {
                    audioSource.outputAudioMixerGroup = soundAudioClip.audioMixerGroup;
                }
                else
                {
                    Debug.LogWarning("Missing Mixer Group for "+soundAudioClip.sound);
                }
                audioSource.Play();
                
                Object.Destroy(soundGameObject, audioSource.clip.length);
            }
            
        }
        
        /// <summary>
        /// Plays a 2d sound
        /// </summary>
        /// <param name="sound">The sound to play</param>
        public static void PlaySound(Sound sound)
        {
            
            if (CanPlaySound(sound, out var idx))
            {
                if (_oneShotGameObject == null)
                {
                    _oneShotGameObject = new GameObject("Sound");
                    _oneShotAudioSource = _oneShotGameObject.AddComponent<AudioSource>();
                    _oneShotAudioSource.spatialize = false;
                }
                GameAssets.SoundAudioClip soundAudioClip = GetSoundAudioClip(sound, idx);
                if (soundAudioClip.audioMixerGroup != null)
                {
                    _oneShotAudioSource.outputAudioMixerGroup = soundAudioClip.audioMixerGroup;
                }
                else
                {
                    Debug.LogWarning("Missing Mixer Group for "+soundAudioClip.sound);
                }

                _oneShotAudioSource.pitch = soundAudioClip.pitch;
                _oneShotAudioSource.PlayOneShot(soundAudioClip.audioClip, soundAudioClip.volume);
            }
            
        }

        /// <summary>
        /// Checks whether it is allowed to play specified sound. 
        /// </summary>
        /// <param name="sound">The <see cref="Sound"/> to check.</param>
        /// <param name="idx">Index of sound if there are multiple audio clips for one sound</param>
        /// <returns>Can <paramref name="sound"/> be played or not.</returns>
        private static bool CanPlaySound(Sound sound, out int idx)
        {
            if (_soundTimersAndId.ContainsKey(sound))
            {

                idx = _soundTimersAndId[sound].randIdx;
                if (_soundMinDelays[sound][idx] == 0)
                {
                    return true;
                }
                else
                {
                    var lastTimePlayed = _soundTimersAndId[sound].lastPlayed;
                    if (lastTimePlayed + _soundMinDelays[sound][idx] < Time.time)
                    {
                        var randRange = Random.Range(0, _soundMinDelays[sound].Count);
                        _soundTimersAndId[sound] = (Time.time, randRange);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                
            }
            else
            {
                idx = 0;
                return false;
            }
        }

        /// <summary>
        /// Get a <see cref="GameAssets.SoundAudioClip"/> corresponding to given <see cref="Sound"/>.
        /// </summary>
        /// <param name="sound">The <see cref="Sound"/> for which we want a <see cref="GameAssets.SoundAudioClip"/>.</param>
        /// <param name="idx">the index of the clip we want, used for when there are multiple clips per <see cref="Sound"/></param>
        /// <returns>The selected <see cref="GameAssets.SoundAudioClip"/> if it exists, otherwise <c>null</c>.</returns>
        private static GameAssets.SoundAudioClip GetSoundAudioClip(Sound sound, int idx = 0)
        {
            var matchingSounds = new List<GameAssets.SoundAudioClip>();
            foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClips)
            {
                if (soundAudioClip.sound == sound)
                {
                    matchingSounds.Add(soundAudioClip);
                }
            }

            if (matchingSounds.Count > 0)
            {
                return matchingSounds[idx];
            }
            else
            {
                Debug.LogError("Sound "+sound+" not found");
                return null;
            }
        }
    }
}