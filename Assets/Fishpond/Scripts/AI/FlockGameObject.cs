using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.AI.Flocking
{
    public class FlockGameObject : MonoBehaviour
    {
        FlockAgent flockAgent;

        public void SetUp(FlockAgent flockAgent) {
            this.flockAgent = flockAgent;
        }

        public void UpdateTransform() {
            //Position
            this.transform.position = flockAgent.flockDataStruct.position;

            //Rotation
            float angle = (Mathf.Atan2(flockAgent.flockDataStruct.velocity.z, flockAgent.flockDataStruct.velocity.x) * Mathf.Rad2Deg);
            this.transform.rotation = Quaternion.LookRotation(flockAgent.flockDataStruct.velocity, Vector3.up);
        }
    }
}