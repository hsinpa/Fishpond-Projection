using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Hsinpa.AI.Flocking {
    public class FlockAgent : MonoBehaviour
    {
        public FlockDataStruct flockDataStruct;
        private FlockEnvStruct _flockEnvStruct;

        private const float SEPERATION_WEIGHT = 3.5f;
        private const float PULLBACK_WEIGHT = 0.2f;
        private const float COHESION_WEIGHT = 2;

        #region Public API
        public void SetUp(int id, Vector3 velocity, FlockEnvStruct flockEnvStruct)
        {
            flockDataStruct = new FlockDataStruct()
            {
                id = id,
                velocity = velocity,
                acceleration = new Vector3(),
                position = transform.position
            };

            this._flockEnvStruct = flockEnvStruct;
        }
        
        public void OnUpdate(List<FlockDataStruct> flockAgents)
        {
            var filterFlocks = FilterSenseRange(flockAgents, this.flockDataStruct);

            Vector3 steering_force = new Vector3(0, 0, 0);

            if (filterFlocks.Count > 0) {
                steering_force += GetAlignmentForce(filterFlocks);
                steering_force += GetCohesiveForce(filterFlocks, flockDataStruct) * COHESION_WEIGHT;
                steering_force += (GetSeperationForce(filterFlocks, flockDataStruct) * SEPERATION_WEIGHT);
                steering_force += GetPullCenterForce() * PULLBACK_WEIGHT;
            }

            this.flockDataStruct.acceleration = steering_force;

            ProcessMovement();
        }
        #endregion

        #region Private API
        private List<FlockDataStruct> FilterSenseRange(List<FlockDataStruct> flockAgents, FlockDataStruct selfDataStruct) {
            List<FlockDataStruct> filterAgents = new List<FlockDataStruct>();

            flockAgents.ForEach(x =>
            {
                if (
                x.id != selfDataStruct.id &&
                Vector3.Distance(x.position, selfDataStruct.position) < this._flockEnvStruct.sense_range) {


                    filterAgents.Add(x);
                }
            });

            return filterAgents;
        }

        private Vector3 GetAlignmentForce(List<FlockDataStruct> flockAgents) {
            return AverageOperation(flockAgents, x => x.velocity);
        }

        private Vector3 GetCohesiveForce(List<FlockDataStruct> flockAgents, FlockDataStruct selfDataStruct)
        {
            Vector3 average = AverageOperation(flockAgents, x => x.position);

            average -= selfDataStruct.position;

            return average;
        }

        private Vector3 GetSeperationForce(List<FlockDataStruct> flockAgents, FlockDataStruct selfDataStruct) {

            Vector3 emptyVector = new Vector3(0,0,0);

            Vector3 average = AverageOperation(flockAgents, x => {

                float dist = Vector3.Distance(x.position, selfDataStruct.position);

                //Push away from target agent
                Vector3 sepForce = (selfDataStruct.position - x.position);

                //Scale by distance, the near, the stronger
                sepForce /= dist;

                return sepForce;
            });

            return average;
        }

        private Vector3 GetPullCenterForce() {
            Vector3 centerOffset = this._flockEnvStruct.centerWorldPos - this.flockDataStruct.position;
            float t = centerOffset.magnitude / this._flockEnvStruct.centerRadius;

            //If we are within specfic range, then no effect at all
            if (t < 0.9f)
            {
                return new Vector3(0, 0, 0);
            }

            return centerOffset * (t * t);
        }

        private Vector3 AverageOperation(List<FlockDataStruct> flockAgents, System.Func<FlockDataStruct, Vector3> ops) {
            Vector3 average = new Vector3(0, 0, 0);

            flockAgents.ForEach(x =>
            {
                average += ops(x);
            });

            average /= flockAgents.Count;

            return average;
        }

        private void ProcessMovement() {
            flockDataStruct.velocity += flockDataStruct.acceleration * Time.deltaTime;
            flockDataStruct.velocity = Vector3.Normalize(flockDataStruct.velocity) * this._flockEnvStruct.speed;

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
