using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Hsinpa.AI.Flocking {
    public class FlockAgent : MonoBehaviour
    {
        public FlockDataStruct flockDataStruct;

        #region Public API
        public void SetUp(int id, float sense_range, Vector3 velocity)
        {
            flockDataStruct = new FlockDataStruct()
            {
                id = id,
                velocity = velocity,
                acceleration = new Vector3(),
                sense_range = sense_range,
                position = transform.position
            };
        }
        
        public void OnUpdate(List<FlockDataStruct> flockAgents)
        {
            var filterFlocks = FilterSenseRange(flockAgents, this.flockDataStruct);

            Vector3 steering_force = new Vector3();
                    steering_force += GetAlignmentForce(filterFlocks);

            ProcessMovement();
        }
        #endregion

        #region Private API
        private List<FlockDataStruct> FilterSenseRange(List<FlockDataStruct> flockAgents, FlockDataStruct selfDataStruct) {
            List<FlockDataStruct> filterAgents = new List<FlockDataStruct>();

            flockAgents.ForEach(x =>
            {
                if (Vector3.Distance(x.position, selfDataStruct.position) < selfDataStruct.sense_range) { 
                    filterAgents.Add(x);
                }
            });

            return filterAgents;
        }

        private Vector3 GetAlignmentForce(List<FlockDataStruct> flockAgents) {
            Vector3 average = new Vector3();

            flockAgents.ForEach(x =>
            {
                average += x.velocity;
            });

            average /= flockAgents.Count;

            return average;
        }

        private void ProcessMovement() {
            flockDataStruct.velocity += flockDataStruct.acceleration * Time.deltaTime;
            flockDataStruct.position += flockDataStruct.velocity * Time.deltaTime;

            //Position
            this.transform.position = flockDataStruct.position;

            //Rotation
            float angle = (Mathf.Atan2(flockDataStruct.velocity.z, flockDataStruct.velocity.x) * Mathf.Rad2Deg) - 90 ;
            this.transform.rotation = Quaternion.Euler(90, 0, angle);
        }
        #endregion

    }
}
