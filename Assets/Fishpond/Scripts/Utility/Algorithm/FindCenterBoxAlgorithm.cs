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
        private const float ErrorRange = 0.1f;
        Color[] _colors;
        int _width, _height;

        public async Task<GeneralDataStructure.AreaStruct> FindSize(Color[] colors, int width, int height) {
            _colors = colors;
            _width = width;
            _height = height;

            Task<Vector2Int>[] tasks = new Task<Vector2Int>[4];
            int centerX = Mathf.RoundToInt(width * 0.5f);
            int centerY = Mathf.RoundToInt(height * 0.5f);
            Vector3 refCol = ColorToVector(colors[ GetPixelIndex(centerX, centerY) ]);

            Debug.Log("RefCol x " + refCol.x + ", y " + refCol.y +", z" + refCol.z);

            tasks[0] = Task.Run(() =>  FindEdge(centerX, centerY, new Vector2Int(0, 1), refCol)); //Top
            tasks[1] = Task.Run(() => FindEdge(centerX, centerY, new Vector2Int(0, -1), refCol)); //Down
            tasks[2] = Task.Run(() => FindEdge(centerX, centerY, new Vector2Int(-1, 0), refCol)); // Left
            tasks[3] = Task.Run(() => FindEdge(centerX, centerY, new Vector2Int(1, 0), refCol)); // Right

            await Task.WhenAll(tasks);

            GeneralDataStructure.AreaStruct areaStruct = new GeneralDataStructure.AreaStruct();

            Vector2Int top = await tasks[0], down = await tasks[1], left = await tasks[2], right = await tasks[3];
            areaStruct.width = right.x - left.x;
            areaStruct.height = top.y - down.y;
            areaStruct.y = Mathf.FloorToInt((areaStruct.height * 0.5f) + top.y);
            areaStruct.x = Mathf.FloorToInt((areaStruct.width * 0.5f) + left.x);

            Debug.Log("Color right.x " + right.x + ", left.x " + left.x);
            Debug.Log("Color top.y " + top.y + ", down.y " + down.y);

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
                    //Debug.Log("RefCol x " + cacheColor.x + ", y " + cacheColor.y + ", z" + cacheColor.z);

                    if (dist > ErrorRange)
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

        private bool IsWithinRange(int x , int y) {
            return x >= 0 && x < this._width && y >= 0 && y < this._height;
        }

    }
}