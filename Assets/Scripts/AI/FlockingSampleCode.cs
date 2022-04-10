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

        void Start()
        {
            flockManager.Init(PondSize, SpawnCount, Sensitivity);
        }

    }
}