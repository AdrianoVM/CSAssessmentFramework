using UnityEngine;

namespace Player
{
    public class RespawnZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out GameCharacter character))
            {
                character.Respawn(GameCharacter.SpawnPoint.AtPath);
            }
        }
    }
}
