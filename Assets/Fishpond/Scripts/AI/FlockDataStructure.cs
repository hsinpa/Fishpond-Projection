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
        public float speedRatio; // between 0 and 1
    }

    public struct FlockColliderStruct
    {
        public Vector3 position;
        public float height;
        public float width;
    }

    public struct FlockEnvStruct
    {
        //Center and radius
        public Vector3 centerWorldPos;
        public float centerRadius;

        //Movement
        public float max_speed;
        public float min_speed;

        public float escape_multiplier;
        
        //Sensibility
        public float sense_range;

        //Waypoint, Unique for individual object
        public Vector3 waypoint;
        public float waypoint_spawn_time; // second
        public float waypoint_last_spawn_time; //second

    }
}