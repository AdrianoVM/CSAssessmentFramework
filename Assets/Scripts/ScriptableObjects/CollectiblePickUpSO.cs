using Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
    /// <summary>
    /// Channel used to communicate that a <see cref="Collectible"/> has been picked up.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Collectible Event Channel", order = 0)]
    public class CollectiblePickUpSO : ScriptableObject
    {
        public UnityAction<Collectible> OnCollectiblePickup;

        public void RaiseEvent(Collectible collectible)
        {
            if (OnCollectiblePickup != null)
            {
                OnCollectiblePickup.Invoke(collectible);
            }
            else
            {
                Debug.Log("Collectible Pickup event Raised but nobody picked up");
            }
        }
    }
}