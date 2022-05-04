
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Utility.Algorithm {
    public class GeneralDataStructure {

        public struct AreaStruct {
            public int x;
            public int y;
            public int width;
            public int height;
            public int id;
            public int area => width * height;
        }
    }
}