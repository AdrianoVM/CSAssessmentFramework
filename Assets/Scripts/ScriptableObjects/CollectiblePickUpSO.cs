using Options.Gameplay.Activity;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
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