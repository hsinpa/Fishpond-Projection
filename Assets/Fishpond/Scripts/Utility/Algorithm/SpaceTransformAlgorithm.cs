using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hsinpa.Utility.Algorithm
{
    public class SpaceTransformAlgorithm
    {
        float _worldScreenWidth, _worldScreenHeight, pjtXposRatio, pjtYPosRatio;
        int _areaWidth, _areaHeight;

        Vector3 worldCamPos;
        Vector4 worldSpaceCorner;
        float _worldWidth, _worldHeight;

        GeneralDataStructure.AreaStruct _targetArea;

        public SpaceTransformAlgorithm(Vector3 worldCamPos, float worldScreenWidth, float worldScreenHeight, int areaWidth, int areaHeight, GeneralDataStructure.AreaStruct targetArea) {
            this.worldCamPos = worldCamPos;
            this._worldScreenWidth = worldScreenWidth;
            this._worldScreenHeight = worldScreenHeight;
            this._areaWidth = areaWidth;
            this._areaHeight = areaHeight;

            Debug.Log($"_worldScreenWidth {_worldScreenWidth}, _worldScreenHeight {_worldScreenHeight}");
            Debug.Log($"_areaWidth {_areaWidth}, _areaHeight {_areaHeight}");

            this._targetArea = targetArea;

            CalculateRelativeRelationship();
        }

        public Vector4 ToGameWorldSpace(GeneralDataStructure.AreaStruct targetArea, int spaceHeight, int spaceWidth) {
            float xRatio = targetArea.x / (float)spaceWidth;
            float yRatio = targetArea.y / (float)spaceHeight;
            float heightRatio = targetArea.height / (float)spaceHeight;
            float widthRatio = targetArea.width / (float)spaceWidth;

            float height = _worldHeight * (heightRatio);
            float width = _worldWidth * (widthRatio);

            float y = Mathf.Lerp(worldSpaceCorner.y, worldSpaceCorner.x, yRatio);
            float x = Mathf.Lerp(worldSpaceCorner.w, worldSpaceCorner.z, xRatio);

            //Debug.Log($"ToGameWorldSpace x {x}, WorldFullHeight {y}, height {height}, width {width}");
            //Debug.Log($"heightTopRatio {heightTopRatio}, heightBottomRatio {heightBottomRatio}, widthLeftRatio {widthLeftRatio}, widthRightRatio {widthRightRatio}");
            //Debug.Log($"xRatio {xRatio}, yRatio {yRatio}, heightRatio {heightRatio}, widthRatio {widthRatio}");

            return new Vector4(x, y, width, height);
        }

        private void CalculateRelativeRelationship() {
            float widthRatio = ( this._targetArea.width) / (float)_areaWidth;
            float heightRatio = ( this._targetArea.height) / (float)_areaHeight;
            this.pjtXposRatio = (_areaWidth - this._targetArea.x) / (float)_areaWidth;
            this.pjtYPosRatio = this._targetArea.y / (float)_areaHeight;

            float verticalResidualRatio = (heightRatio * 0.5f);
            float horizontalResidualRatio = (widthRatio * 0.5f);

            this._worldWidth = this._worldScreenWidth / widthRatio;
            this._worldHeight = this._worldScreenHeight / heightRatio;

            float worldCenterXOffset = this.worldCamPos.x - (this._worldWidth * (pjtXposRatio));
            float worldCenterYOffset = this.worldCamPos.z - (this._worldHeight * (pjtYPosRatio));

            float greenscreenTop =  (this._worldHeight *  (pjtYPosRatio + verticalResidualRatio)) ;
            float greenscreanBottom = (this._worldHeight * (pjtYPosRatio - verticalResidualRatio)) ;
            float greenscreenRight =  (this._worldWidth * (pjtXposRatio + horizontalResidualRatio)) ;
            float greenscreenLeft =  (this._worldWidth * (pjtXposRatio - horizontalResidualRatio));

            float worldTop = greenscreenTop + (this._worldHeight * (1 - pjtYPosRatio + verticalResidualRatio)) + worldCenterYOffset;
            float worldBottom = worldCenterYOffset;
            float worldRight = greenscreenRight + (this._worldWidth * (1 - pjtXposRatio + horizontalResidualRatio)) + worldCenterXOffset;
            float worldLeft = worldCenterXOffset;

            Debug.Log($"widthRatio {widthRatio}, heightRatio {heightRatio}");
            Debug.Log($"pjtXposRatio {this.pjtXposRatio}, pjtYPosRatio {this.pjtYPosRatio}");
            Debug.Log($"WorldFullWidth {this._worldWidth}, WorldFullHeight {this._worldHeight}, World top {worldTop}, Bottom {worldBottom}, Right {worldRight}, left {worldLeft}");

            worldSpaceCorner = new Vector4(worldTop, worldBottom, worldLeft, worldRight);
        }


    }
}