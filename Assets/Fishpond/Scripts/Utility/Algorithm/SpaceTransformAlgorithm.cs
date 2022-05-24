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

        public Vector4 ToGameWorldSpace(GeneralDataStructure.AreaStruct _targetArea, int spaceHeight, int spaceWidth) {
            float xRatio = _targetArea.x / (float)spaceWidth;
            float yRatio = _targetArea.y / (float)spaceHeight;
            float heightRatio = _targetArea.height / (float)spaceHeight;
            float widthRatio = _targetArea.width / (float)spaceWidth;

            float heightTopRatio = yRatio + (heightRatio * 0.5f);
            float heightBottomRatio = yRatio - (heightRatio * 0.5f);
            float widthLeftRatio = xRatio - (widthRatio * 0.5f);
            float widthRightRatio = xRatio + (widthRatio * 0.5f);

            float y = Mathf.Lerp(worldSpaceCorner.y, worldSpaceCorner.x, yRatio);
            float x = Mathf.Lerp(worldSpaceCorner.w, worldSpaceCorner.z, xRatio);
            float height = _worldHeight * heightRatio;
            float width = _worldWidth * widthRatio;

            //Debug.Log($"ToGameWorldSpace x {x}, WorldFullHeight {y}, height {height}, width {width}");
            //Debug.Log($"heightTopRatio {heightTopRatio}, heightBottomRatio {heightBottomRatio}, widthLeftRatio {widthLeftRatio}, widthRightRatio {widthRightRatio}");
            //Debug.Log($"xRatio {xRatio}, yRatio {yRatio}, heightRatio {heightRatio}, widthRatio {widthRatio}");

            return new Vector4(x, y, width, height);
        }

        private void CalculateRelativeRelationship() {
            float widthRatio = ( this._targetArea.width) / (float)_areaWidth;
            float heightRatio = ( this._targetArea.height) / (float)_areaHeight;
            this.pjtXposRatio = this._targetArea.x / (float)_areaWidth;
            this.pjtYPosRatio = this._targetArea.y / (float)_areaHeight;

            float pjtTopResidualRatio = this.pjtYPosRatio - (this.pjtYPosRatio - (heightRatio * 0.5f));
            float pjtBtnResidualRatio = (this.pjtYPosRatio + (heightRatio * 0.5f)) - this.pjtYPosRatio;

            float pjtRightResidualRatio = (pjtXposRatio + (widthRatio * 0.5f)) - pjtXposRatio;
            float pjtLeftResidualRatio = this.pjtXposRatio - (this.pjtXposRatio - (widthRatio * 0.5f));

            this._worldWidth = this._worldScreenWidth / widthRatio;
            this._worldHeight = this._worldScreenHeight / heightRatio;

            float worldTop = (this.worldCamPos.z + (this._worldScreenHeight*0.5f)) + (this._worldHeight * pjtTopResidualRatio);
            float worldBottom = (this.worldCamPos.z - (this._worldScreenHeight * 0.5f)) - (this._worldHeight * pjtBtnResidualRatio);
            float worldRight = (this.worldCamPos.x + (this._worldScreenWidth * 0.5f)) + (this._worldWidth * pjtRightResidualRatio);
            float worldLeft = (this.worldCamPos.x - (this._worldScreenWidth * 0.5f)) - (this._worldWidth * pjtLeftResidualRatio);

            Debug.Log($"widthRatio {widthRatio}, heightRatio {heightRatio}");
            Debug.Log($"pjtTopResidualRatio {pjtTopResidualRatio}, pjtBtnResidualRatio {pjtBtnResidualRatio}, pjtRightResidualRatio {pjtRightResidualRatio}, pjtLeftResidualRatio {pjtLeftResidualRatio}");
            Debug.Log($"WorldFullWidth {this._worldWidth}, WorldFullHeight {this._worldHeight}, World top {worldTop}, Bottom {worldBottom}, Right {worldRight}, left {worldLeft}");

            worldSpaceCorner = new Vector4(worldTop, worldBottom, worldLeft, worldRight);
        }


    }
}