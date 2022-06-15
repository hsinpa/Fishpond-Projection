using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.AI.Flocking
{
    public class FlockGameObject : MonoBehaviour
    {
        FlockAgent flockAgent;

        Animation movementAnim;

        [SerializeField]
        private float center_anim_time;

        public void SetUp(FlockAgent flockAgent) {
            this.flockAgent = flockAgent;
            this.movementAnim = this.GetComponentInChildren<Animation>();
        }

        public void UpdateTransform() {
            //Position
            this.transform.position = flockAgent.flockDataStruct.position;

            //Rotation
            float angle = (Mathf.Atan2(flockAgent.flockDataStruct.velocity.z, flockAgent.flockDataStruct.velocity.x) * Mathf.Rad2Deg);
            this.transform.rotation = Quaternion.LookRotation(flockAgent.flockDataStruct.velocity, Vector3.up);

            if (this.movementAnim != null) {
                foreach (AnimationState state in this.movementAnim) {
                    state.time = center_anim_time + (Mathf.Sin(Time.time * flockAgent.flockDataStruct.speedRatio));
                    //state.speed = flockAgent.flockDataStruct.speedRatio;

                }

            }

        }
    }
}