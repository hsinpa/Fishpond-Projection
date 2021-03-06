using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hsinpa.Utility.Algorithm;

namespace Hsinpa.AI.Flocking {
    public class FlockAgent
    {
        public FlockDataStruct flockDataStruct;
        private FlockEnvStruct _flockEnvStruct;

        private readonly static Vector3 ZeroVector = new Vector3(0, 0, 0);

        private const float ALIGN_WEIGHT = 1.5f;
        private const float SEPERATION_WEIGHT = 3.5f;
        private const float PULLBACK_WEIGHT = 0.2f;
        private const float COHESION_WEIGHT = 2.5f;
        private const float COLLISION_WEIGHT = 5;
        private const float RANDOM_WEIGHT = 0.5f;

        private const float COLLISION_RADIUS = 0.1f;
        private const float DELTA_TIME = 0.02f;

        private enum State { Normal, Flee }
        private State _state = State.Normal;
        private const float _fleeDuration = 2f;
        private float _fleeTimeRecord = 0;
        private Vector3 _lastFleeVelocity = new Vector3();

        public bool isCollideInThisFrame = false;
        private float _seed;
        System.Random _randomSystem;

        #region Public API
        public FlockAgent(int id, Vector3 position, Vector3 velocity, FlockEnvStruct flockEnvStruct)
        {
            flockDataStruct = new FlockDataStruct()
            {
                id = id,
                velocity = velocity,
                acceleration = new Vector3(),
                position = position,
            };

            this._flockEnvStruct = flockEnvStruct;

            this._randomSystem = new System.Random(id);

            ResetWayPoint();
        }
        
        public void OnUpdate(List<FlockAgent> flockAgents, List<FlockColliderStruct> colliders)
        {
            this.isCollideInThisFrame = false;
            int index_offset = 10;
            var filterFlocks = FilterSenseRange(flockAgents, this.flockDataStruct);
            this._seed = (index_offset * this.flockDataStruct.id + FlockManager.TIME) * DELTA_TIME;

            Vector3 steering_force = new Vector3(0, 0, 0);

            if (_state == State.Flee)
            {
                steering_force += _lastFleeVelocity;

                if (this._fleeTimeRecord + _fleeDuration < FlockManager.TIME)
                {
                    _state = State.Normal;
                }
            }

            if (_state == State.Normal)
            {
                if (filterFlocks.Count > 0)
                {
                    steering_force += GetAlignmentForce(filterFlocks) * ALIGN_WEIGHT;
                    steering_force += GetCohesiveForce(filterFlocks, flockDataStruct) * COHESION_WEIGHT;
                    steering_force += (GetSeperationForce(filterFlocks, flockDataStruct) * SEPERATION_WEIGHT);
                }

                steering_force += GetPullCenterForce() * PULLBACK_WEIGHT;
                steering_force += GetCollisionAvoidForce(colliders) * COLLISION_WEIGHT;
                steering_force += GetRandomForce() * RANDOM_WEIGHT;

                if (_state == State.Flee)
                    this.isCollideInThisFrame = true;
            }

            this.flockDataStruct.acceleration = steering_force;

            ProcessMovement();
        }
        #endregion

        #region Private API
        private List<FlockDataStruct> FilterSenseRange(List<FlockAgent> flockAgents, FlockDataStruct selfDataStruct) {
            List<FlockDataStruct> filterAgents = new List<FlockDataStruct>();

            flockAgents.ForEach(x =>
            {
                FlockDataStruct f = x.flockDataStruct;
                if (
                f.id != selfDataStruct.id &&
                Vector3.Distance(f.position, selfDataStruct.position) < this._flockEnvStruct.sense_range) {


                    filterAgents.Add(f);
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
            if (t < 0.8f)
            {
                return new Vector3(0, 0, 0);
            }

            return centerOffset * (t * t);
        }

        private Vector3 GetRandomForce()
        {
            Vector3 centerOffset = (this._flockEnvStruct.waypoint - this.flockDataStruct.position);
            float centerDist = Vector3.Distance(this._flockEnvStruct.waypoint, this.flockDataStruct.position);

            float sampleX = (Mathf.PerlinNoise(_seed, 0)) * 2 - 1;
            float sampleY = (Mathf.PerlinNoise(0, _seed)) * 2 - 1;
            //Debug.Log($"SampleX {sampleX}, SampleY {sampleY}");

            //Object are more likely to gather at center
            Vector3 randomForce = this.flockDataStruct.velocity;
            //randomForce.x += sampleX;
            //randomForce.z += sampleY;

            randomForce += (centerOffset.normalized * 0.1f);
            //Debug.Log(centerDist);
            if (centerDist < 3f) {
                ResetWayPoint();
            }

            randomForce.Normalize();
            return randomForce;
        }

        private Vector3 GetCollisionAvoidForce(List<FlockColliderStruct> colliders) {

            var area = COLLISION_RADIUS * 2;

            var averageForce = AverageOperation(colliders, collider =>
            {
                Vector3 force = new Vector3(0,0,0);
                Vector3 expectedPosition = this.flockDataStruct.position + (Vector3.Normalize(this.flockDataStruct.velocity) * area);

                var distance = Vector3.Distance(collider.position, expectedPosition);

                Vector3 expectColCircle = new Vector3(expectedPosition.x, expectedPosition.z, COLLISION_RADIUS);
                Vector4 tCollideVec = new Vector4(collider.position.x, collider.position.z, collider.width, collider.height);

                if ( CollisionAlgorithm.BoxCircleIntersect(tCollideVec, expectColCircle) ) {
                    force = this.flockDataStruct.position - collider.position;
                    //force /= distance;
                }

                return force;
            });

            if (averageForce.magnitude > 0.1f) {
                ResetWayPoint();
                this._lastFleeVelocity = averageForce.normalized;
                this._fleeTimeRecord = FlockManager.TIME;
                this._state = State.Flee;
            }

            return averageForce;
        }

        private Vector3 AverageOperation<T>(List<T> flockAgents, System.Func<T, Vector3> ops) {
            Vector3 average = new Vector3(0, 0, 0);

            if (flockAgents.Count <= 0) return average;

            flockAgents.ForEach(x =>
            {
                average += ops(x);
            });

            average /= flockAgents.Count;

            return average;
        }

        private Vector3 ResetWayPoint() {

            if (FlockManager.TIME - this._flockEnvStruct.waypoint_last_spawn_time > this._flockEnvStruct.waypoint_spawn_time) 
                return this._flockEnvStruct.waypoint;

            float size = (this._flockEnvStruct.centerRadius * 0.8f);
            float random_offset_x = (_randomSystem.Next(-100, 100) * 0.01f) * size;
            float random_offset_y = (_randomSystem.Next(-100, 100) * 0.01f) * size;

            //float random_y = Random.Range(this._flockEnvStruct.centerWorldPos.z - size, this._flockEnvStruct.centerWorldPos.z + size);

            this._flockEnvStruct.waypoint = new Vector3(this._flockEnvStruct.centerWorldPos.x + random_offset_x, 0, this._flockEnvStruct.centerWorldPos.z + random_offset_y);

            this._flockEnvStruct.waypoint_last_spawn_time = FlockManager.TIME;

            return this._flockEnvStruct.waypoint;
        }

        private void ProcessMovement() {
            float decay = 0.98f;
            flockDataStruct.velocity *= decay; // Decay

            Vector3 previousVelocity = flockDataStruct.velocity.normalized;
            Vector3 velocity = flockDataStruct.velocity + (flockDataStruct.acceleration * DELTA_TIME);
            var speedTuple = GetSpeed();
            float velocityChange = (1 - Vector3.Dot(previousVelocity, velocity.normalized)) * 5;
            
            flockDataStruct.velocity = Vector3.ClampMagnitude(velocity, speedTuple.Item1);
            flockDataStruct.speedRatio = velocityChange + (speedTuple.Item2 * 0.6f);

            flockDataStruct.position += flockDataStruct.velocity * DELTA_TIME;
        }

        /// <summary>
        /// Speed, SpeedRatio
        /// </summary>
        /// <returns></returns>
        private (float, float) GetSpeed() {
            float t = (Mathf.PerlinNoise(_seed * 10, _seed));
            float speed = Mathf.Lerp(this._flockEnvStruct.min_speed, this._flockEnvStruct.max_speed, t);

            if (_state == State.Flee)
               return (this._flockEnvStruct.max_speed * 2, 1);

            return (speed, t);
        }

        #endregion

    }
}
