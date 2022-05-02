using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hsinpa.Utility.Algorithm {
    public class FloodfillAlgorithm
    {
        private float _threshold_area;

        public FloodfillAlgorithm(float threshold_area)
        {
            this._threshold_area = threshold_area;
        }

        public List<AreaStruct>  FindAreaStruct(Color[] colors, int width, int height) {
            List<AreaStruct> areas = new List<AreaStruct>();

            int colorLens = colors.Length;

            for (int i = 0; i < colorLens; i++) {
                int h = Mathf.FloorToInt(i / (float)width);

                int x = i % width;
                int y = h;

                //Debug.Log("Width " + x +", " + y);

                int colorIndex = GetPixelIndex(i, width, height);
                
            }

            return areas;
        }

        private int GetPixelIndex(int index, int width, int height) {
            return 0;
        }

        public struct AreaStruct {
            public float x;
            public float y;
            public float width;
            public float height;
            public float area => width * height;
        }

    }
}
