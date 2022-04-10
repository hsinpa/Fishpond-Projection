using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.AI.Flocking
{
    public struct FlockDataStruct
    {
        public int id;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public float sense_range;
    }
}