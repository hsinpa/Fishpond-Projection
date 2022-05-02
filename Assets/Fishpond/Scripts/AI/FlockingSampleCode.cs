using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Hsinpa.AI.Flocking
{
    public class FlockingSampleCode : MonoBehaviour
    {
        [SerializeField]
        private FlockManager flockManager;

        [SerializeField]
        private Transform debugColliderHolder;

        [SerializeField]
        private Vector2 PondSize;

        [SerializeField, Range(0, 200)]
        private int SpawnCount;

        [SerializeField, Range(0.1f, 20f)]
        private float Sensitivity;

        [SerializeField, Range(0f, 5f)]
        private float AgentMaxSpeed;

        [SerializeField, Range(0f, 5f)]
        private float AgentMinSpeed;

        [SerializeField, Range(0f, 10f)]
        private float AgendEscapeValue;

        void Start()
        {
            FlockEnvStruct flockEnvStruct = new FlockEnvStruct();
            flockEnvStruct.max_speed = AgentMaxSpeed;
            flockEnvStruct.min_speed = AgentMinSpeed;
            flockEnvStruct.waypoint_spawn_time = 0.5f;

            flockEnvStruct.escape_multiplier = AgendEscapeValue;
            flockEnvStruct.sense_range = Sensitivity;
            flockEnvStruct.centerWorldPos = this.transform.position;
            flockEnvStruct.centerRadius = PondSize.magnitude * 0.5f;

            flockManager.Init(PondSize, SpawnCount, flockEnvStruct);

            FlockDebugCollider[] debugColliders = debugColliderHolder.GetComponentsInChildren<FlockDebugCollider>();
            Debug.Log(debugColliders.Length);
            var colliders = debugColliders.Select(x => x.FlockColliderStruct).ToList();

            foreach (var c in colliders) {
                flockManager.RegisterCollider(c);
            }
        }

    }
}