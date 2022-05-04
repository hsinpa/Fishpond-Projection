using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Hsinpa.Utility.Algorithm;
using Hsinpa.Utility;

namespace Hsinpa.Realsense {
    public class RealsenseCtrl : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.RawImage debugTexture_a;

        [SerializeField]
        private UnityEngine.UI.RawImage debugTexture_b;

        [SerializeField]
        private UnityEngine.UI.RawImage colorTexture_canvas;

        [SerializeField]
        private MultiThreadProcess multiThreadProcess;

        [SerializeField, Range(0, 1)]
        private float depthmapThreshold;

        [SerializeField]
        private int outputTexSize;

        [SerializeField]
        private Material realsenseMat;

        [SerializeField]
        private Material customizeMat;

        [SerializeField]
        private Material areaDebugMat;

        [SerializeField]
        private bool debugAreaFlag;

        private Texture rawDepthMapTexture;
        private Texture rawColorTexture;

        private RenderTexture imageProcessA;
        private RenderTexture imageProcessB;
        private RenderTexture grayDepthMapTexture;
        private RenderTexture filterTexture;

        private Texture2D _imgProcessingTex;
        private SegmentationAlgorithm _segmentationAlgorithm;

        private float aspectRatio = 0;
        private bool textureCopyFlag = true;

        public Texture depth_texture
        {
            get
            {
                return rawDepthMapTexture;
            }
            set
            {
                if (rawDepthMapTexture == value)
                    return;

                rawDepthMapTexture = value;
                OnTexture(value);
            }
        }

        public Texture color_texture
        {
            get
            {
                return rawColorTexture;
            }
            set
            {
                if (rawColorTexture == value)
                    return;

                rawColorTexture = value;
                colorTexture_canvas.texture = rawColorTexture;
            }
        }

        private void OnTexture(Texture p_texture) {
            aspectRatio = p_texture.height / (float)p_texture.width;
            int outputWidth = outputTexSize;
            int outputHeight = Mathf.FloorToInt(aspectRatio * outputTexSize);

            Debug.Log("OnTexture width " + outputWidth + ", " + outputHeight);

            if (_segmentationAlgorithm == null)
                _segmentationAlgorithm = new SegmentationAlgorithm(threshold_area: 100, width: outputWidth, height: outputHeight);

            if (grayDepthMapTexture == null)
                grayDepthMapTexture = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 16, RenderTextureFormat.R16);

            if (imageProcessA == null)
                imageProcessA = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            if (imageProcessB == null)
                imageProcessB = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            if (filterTexture == null)
                filterTexture = TextureUtility.GetRenderTexture(outputWidth, outputHeight, 8, RenderTextureFormat.ARGB32);

            if (_imgProcessingTex == null)
                _imgProcessingTex = new Texture2D(outputWidth, outputHeight, TextureFormat.RGB24, false);

            if (debugTexture_a != null)
                debugTexture_a.texture = filterTexture;

            if (debugTexture_b != null)
                debugTexture_b.texture = _imgProcessingTex;
        }

        private void Update()
        {
            if (filterTexture == null || grayDepthMapTexture == null) return;

            Graphics.Blit(rawDepthMapTexture, grayDepthMapTexture, realsenseMat);

            Graphics.Blit(grayDepthMapTexture, imageProcessA, customizeMat, 0); // Guassian
            Graphics.Blit(imageProcessA, imageProcessB, customizeMat, 2); // Filter
            Graphics.Blit(imageProcessB, filterTexture, customizeMat, 1); // Erode

            //Graphics.Blit(imageProcessA, filterTexture, customizeMat, 1); // Erode

            ExecEdgeProcessing();
        }


        public void ExecEdgeProcessing()
        {
            if (textureCopyFlag && filterTexture != null)
            {
                textureCopyFlag = false;
                AsyncGPUReadback.Request(filterTexture, 0, TextureFormat.RGB24, OnTexCompleteReadback);
            }
        }

        private void OnTexCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.Log("GPU readback error detected.");
                return;
            }

            if (_imgProcessingTex == null) return;

            _imgProcessingTex.LoadRawTextureData(request.GetData<uint>());
            _imgProcessingTex.Apply();

            if (_segmentationAlgorithm != null) {
                //var areas = _segmentationAlgorithm.FindAreaStruct(_imgProcessingTex.GetPixels());
                
                //if (debugAreaFlag)
                //    DrawAreaHint(areas);
            }

            textureCopyFlag = true;
        }

        private void DrawAreaHint(List<GeneralDataStructure.AreaStruct> areaStructs) {
            //Debug.Log("Segmentation " + areaStructs.Count);

            foreach (GeneralDataStructure.AreaStruct areaStruct in areaStructs) {

                int radiusX = Mathf.RoundToInt(areaStruct.width * 0.5f);
                int radiusY = Mathf.RoundToInt(areaStruct.height * 0.5f);

                for (int x = -radiusX; x < radiusX; x++) {
                    //Up
                    _imgProcessingTex.SetPixel(areaStruct.x + x, areaStruct.y + radiusY, Color.blue);

                    //Down
                    _imgProcessingTex.SetPixel(areaStruct.x + x, areaStruct.y - radiusY, Color.blue);
                }

                for (int y = -radiusY; y < radiusY; y++)
                {
                    //Left
                    _imgProcessingTex.SetPixel(areaStruct.x - radiusX, areaStruct.y + y, Color.blue);

                    //Right
                    _imgProcessingTex.SetPixel(areaStruct.x + radiusX, areaStruct.y + y, Color.blue);
                }
            }

            _imgProcessingTex.Apply();
        }
    }
}
