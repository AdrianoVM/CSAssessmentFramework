using System;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Utilities.Json.Converters
{
    public class SoundAudioClipConverter : PartialConverter<GameAssets.SoundAudioClip>
    {
        protected override void ReadValue(ref GameAssets.SoundAudioClip value, string name, JsonReader reader, JsonSerializer serializer)
        {
            switch (name)
            {
                case nameof(value.sound):
                    SoundManager.Sound t;
                    if (Enum.TryParse(reader.ReadAsString() ?? "", out t))
                    {
                        value.sound = t;
                    }
                    break;
                
                case nameof(value.volume):
                    value.volume = reader.ReadAsFloat() ?? 0f;
                    break;
                
                case nameof(value.audioClip):
                {
                    var id = reader.ReadAsString();
                    JsonHelperExtensions.ReplaceValueWithID(ref value.audioClip, id, nameof(GameAssets));
                    break;
                }
                
                case nameof(value.audioMixerGroup):
                {
                    var id = reader.ReadAsString();
                    JsonHelperExtensions.ReplaceValueWithID(ref value.audioMixerGroup, id, nameof(GameAssets));
                    break;
                }

                case nameof(value.audioMixerGroup) + "Name":
                {
                    value.audioMixerGroup =
                        value.audioMixerGroup.audioMixer.FindMatchingGroups(reader.ReadAsString())[0];
                    break;
                }
                
                case nameof(value.minRepetitionDelay):
                    value.minRepetitionDelay = reader.ReadAsFloat() ?? 0f;
                    break;
                    
            }
        }

        

        protected override void WriteJsonProperties(JsonWriter writer, GameAssets.SoundAudioClip value, JsonSerializer serializer)
        {
            writer.WritePropertyName(nameof(value.sound));
            writer.WriteValue(value.sound);
            writer.WritePropertyName(nameof(value.volume));
            writer.WriteValue(value.volume);
            writer.WritePropertyName(nameof(value.audioClip));
            writer.WriteValue(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value.audioClip)));
            writer.WritePropertyName(nameof(value.audioMixerGroup));
            writer.WriteValue(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value.audioMixerGroup)));
            //AudioMixerGroups are stored in a single asset.
            writer.WritePropertyName(nameof(value.audioMixerGroup)+"Name");
            writer.WriteValue(value.audioMixerGroup.name);
            writer.WritePropertyName(nameof(value.minRepetitionDelay));
            writer.WriteValue(value.minRepetitionDelay);
        }
    }
}