using System;
using Player;
using UnityEngine;

namespace Options.Gameplay
{
    public class RespawnZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out GameCharacter character))
            {
                character.Respawn();
            }
        }
    }
}
