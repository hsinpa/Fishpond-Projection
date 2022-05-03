using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


namespace Hsinpa.Utility.Algorithm {
    public class SegmentationAlgorithm
    {
        private float _threshold_area;
        private int _width, _height;
        private PixelStruct[] pixelStructArray;

        private int incrementalNewIndex = 1;

        public SegmentationAlgorithm(float threshold_area, int width, int height)
        {
            this._threshold_area = threshold_area;
            this.SetSize(width, height);
        }

        public void SetSize(int width, int height) {
            this._width = width;
            this._height = height;


        }

        public List<AreaStruct>  FindAreaStruct(Color[] colors) {
            Dispose();

            List<AreaStruct> areas = new List<AreaStruct>();
            int colorLens = colors.Length;

            for (int x = 0; x < this._width; x++) {
                for (int y = 0; y < this._height; y++) {
                    int index = x + (x * y);



                }
            }

            return areas;
        }

        public void Dispose() {
            int totalLength = this._width * this._height;
            var parallelResult = Parallel.For(0, totalLength, (i) => {
                pixelStructArray[i].state = PixelStruct.State.Unknown;
            });

            incrementalNewIndex = 1;
        }

        private int FindPixelByPos(int pos_x, int pos_y) {
            int index = pos_x + (pos_x * pos_y);

            bool insideBoundary = InsideBoundary(pos_x, pos_y);
            
            return incrementalNewIndex;
        }

        private bool InsideBoundary(int pos_x, int pos_y) {
            return pos_x >= 0 && pos_x < this._width && pos_y >= 0 && pos_y < this._height;
        }

        private int GetPixelIndex(int index, int width, int height) {
            return 0;
        }

        private struct PixelStruct {
            public int x;
            public int y;
            public int index;
            public int unique_id;
            public State state;
        
            public enum State {Unknown, Empty, Object};
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
