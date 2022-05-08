using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.AI.Flocking
{
    public class FlockDebugCollider : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 3f)]
        private float Radius;

        public FlockColliderStruct FlockColliderStruct => new FlockColliderStruct() {position = this.transform.position};

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
