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
    }

    public struct FlockColliderStruct
    {
        public int id;
        public Vector3 position;
        public float radius;
    }

    public struct FlockEnvStruct
    {
        //Center and radius
        public Vector3 centerWorldPos;
        public float centerRadius;

        //Movement
        public float speed;
        public float escape_multiplier;
        
        //Sensibility
        public float sense_range;
    }
}