using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hsinpa.Utility.Algorithm
{
    /// <summary>
    /// Find the size of center box
    /// </summary>
    public class FindCenterBoxAlgorithm
    {
        private const float ErrorRange = 0.4f;
        Color[] _colors;
        int _width, _height;

        public async Task<GeneralDataStructure.AreaStruct> FindSize(Color[] colors, Color refCol, int width, int height) {
            GeneralDataStructure.AreaStruct areaStruct = new GeneralDataStructure.AreaStruct();

            _colors = colors;
            _width = width;
            _height = height;
            
            Task<Vector2Int>[] tasks = new Task<Vector2Int>[4];
            int centerX = Mathf.RoundToInt(width * 0.5f);
            int centerY = Mathf.RoundToInt(height * 0.5f);
            Vector3 cenerColVec = ColorToVector(colors[GetPixelIndex(centerX, centerY)]);
            Vector3 refColVec = ColorToVector(refCol);

            if (Vector3.Distance(cenerColVec, refColVec) > ErrorRange) return areaStruct;

            //Debug.Log("width " + width + ", height " + height + ", centerX " + centerX + ", centerY " + centerY);

            //Debug.Log("RefCol x " + refCol.x + ", y " + refCol.y +", z" + refCol.z);

            tasks[0] = Task.Run(() =>  FindEdge(centerX, centerY, new Vector2Int(0, -1), cenerColVec)); //Top
            tasks[1] = Task.Run(() => FindEdge(centerX, centerY, new Vector2Int(0, 1), cenerColVec)); //Down
            tasks[2] = Task.Run(() => FindEdge(centerX, centerY, new Vector2Int(-1, 0), cenerColVec)); // Left
            tasks[3] = Task.Run(() => FindEdge(centerX, centerY, new Vector2Int(1, 0), cenerColVec)); // Right

            await Task.WhenAll(tasks);
            Vector2Int top = await tasks[0], down = await tasks[1], left = await tasks[2], right = await tasks[3];
            areaStruct.width = right.x - left.x;
            areaStruct.height = down.y - top.y;
            areaStruct.y = Mathf.FloorToInt((areaStruct.height * 0.5f) + top.y);
            areaStruct.x = Mathf.FloorToInt((areaStruct.width * 0.5f) + left.x);

            //Debug.Log("Color right.x " + right.x + ", left.x " + left.x);
            //Debug.Log("Color top.y " + top.y + ", down.y " + down.y);

            return areaStruct;
        }

        private Vector2Int FindEdge(int x, int y, Vector2Int direction, Vector3 referenceCol) {
            bool validIndex = IsWithinRange(x, y);
            Vector2Int currentPoint = new Vector2Int(x, y);
            Vector3 cacheColor = new Vector3();

            while (validIndex) {
                currentPoint.x += direction.x;
                currentPoint.y += direction.y;

                if (IsWithinRange(currentPoint.x, currentPoint.y)) {
                    int index = GetPixelIndex(currentPoint.x, currentPoint.y);
                    Color currentColor = _colors[index];
                    cacheColor.x = currentColor.r;
                    cacheColor.y = currentColor.g;
                    cacheColor.z = currentColor.b;

                    float dist = Vector3.Distance(referenceCol, cacheColor);

                    //if (direction.x == 1) {
                    //    Debug.Log("RefCol x " + referenceCol.x + ", y " + referenceCol.y + ", z" + referenceCol.z);
                    //    Debug.Log("cacheColor x " + cacheColor.x + ", y " + cacheColor.y + ", z" + cacheColor.z);
                    //    Debug.Log("Dist x " + dist);
                    //}

                    if (IsBlackNoise(currentColor)) {
                        validIndex = false;
                        continue;
                    }

                    if (dist > ErrorRange && !IsBlackNoise(currentColor))
                        validIndex = false;
                } else
                {
                    validIndex = false;
                }
            }

            return currentPoint;
        }

        private int GetPixelIndex(int width, int height)
        {
            return width + (this._width * height);
        }

        private Vector3 ColorToVector(Color col) {
            return new Vector3(col.r, col.g, col.b);
        }

        private bool IsBlackNoise(Color currentColor) {
            return currentColor.r <= 0.05f && currentColor.g <= 0.05f && currentColor.b <= 0.05f;
        }

        private bool IsWithinRange(int x , int y) {
            return x >= 0 && x < this._width && y >= 0 && y < this._height;
        }

    }
}