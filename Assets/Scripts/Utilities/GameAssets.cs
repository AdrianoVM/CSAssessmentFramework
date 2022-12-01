using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Utilities
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets Instance
        {
            get
            {
                if (_i == null)
                {
                    _i = (Instantiate(Resources.Load("GameAssets")) as GameObject)?.GetComponent<GameAssets>();
                }
                return _i;
            }
        }

        public SoundAudioClip[] soundAudioClips;
        
        
        // When changing this class, also change SoundAudioClipConverter
        [Serializable]
        public class SoundAudioClip
        {
            public SoundManager.Sound sound;
            public AudioClip audioClip;
            [Range(0,1)]
            public float volume = 1;

            public float pitch = 1;
            public float minRepetitionDelay;
            public AudioMixerGroup audioMixerGroup;
        }
    }
}