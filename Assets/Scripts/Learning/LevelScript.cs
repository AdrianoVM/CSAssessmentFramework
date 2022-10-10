using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Learning
{
    [RequireComponent(typeof(Rigidbody))]
    public class LevelScript : MonoBehaviour
    {
        public int experience;
        
        public XROrigin xrOrigin;

        public Rigidbody projectile;
        public Vector3 offset = Vector3.forward;
        [Range(0,100)]
        public float velocity = 10f;

        public float damageRadius = 1f;
        [HideInInspector]
        public Rigidbody levelRigidbody;

        public int Level
        {
            get { return experience / 750; }
        }

        public void BuildCube()
        {
            Instantiate(this.gameObject, Vector3.one, Quaternion.identity);
        }

        private void Reset()
        {
            levelRigidbody = GetComponent<Rigidbody>();
        }
        [ContextMenu("Fire")]
        public void Fire()
        {
            Rigidbody body = Instantiate(projectile, transform.TransformPoint(offset), transform.rotation);
            body.velocity = Vector3.forward * velocity;
        }
    }
}
