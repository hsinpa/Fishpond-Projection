using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hsinpa.AI.Flocking
{
    public class FlockingSampleCode : MonoBehaviour
    {
        [SerializeField]
        private FlockManager flockManager;

        [SerializeField]
        private Vector2 PondSize;

        [SerializeField, Range(0, 100)]
        private int SpawnCount;

        [SerializeField, Range(0.1f, 20f)]
        private float Sensitivity;

        [SerializeField, Range(0f, 10f)]
        private float AgentSpeed;

        [SerializeField, Range(0f, 10f)]
        private float AgendEscapeValue;

        void Start()
        {
            FlockEnvStruct flockEnvStruct = new FlockEnvStruct();
            flockEnvStruct.speed = AgentSpeed;
            flockEnvStruct.escape_multiplier = AgendEscapeValue;
            flockEnvStruct.sense_range = Sensitivity;
            flockEnvStruct.centerWorldPos = this.transform.position;
            flockEnvStruct.centerRadius = PondSize.magnitude * 0.5f;

            flockManager.Init(PondSize, SpawnCount, flockEnvStruct);
        }

    }
}