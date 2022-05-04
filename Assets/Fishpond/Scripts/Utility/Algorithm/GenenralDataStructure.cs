
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Utility.Algorithm {
    public class GeneralDataStructure {


        public struct AreaStruct {
            public float x;
            public float y;
            public float width;
            public float height;
            public int id;
            public float area => width * height;
        }
    }
}