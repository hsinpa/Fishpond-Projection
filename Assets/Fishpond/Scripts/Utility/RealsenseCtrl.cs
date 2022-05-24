using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Hsinpa.Utility.Algorithm;
using Hsinpa.Utility;
using Intel.RealSense;

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
        private ProjectorSizeCorrector projectorSizeCorrector;

        [SerializeField]
        private bool debugAreaFlag;

        private Texture rawDepthMapTexture;
        private Texture rawColorTexture;

        private RenderTexture imageProcessA;
        private RenderTexture imageProcessB;
        private RenderTexture grayDepthMapTexture;
        private RenderTexture colorMapTexture;
        private RenderTexture filterTexture;

        private Texture2D _depth2DProcessingTex;
        private Texture2D _color2DProcessingTex;
        private SegmentationAlgorithm _segmentationAlgorithm;

        private bool textureCopyFlag = true;

        public System.Action<GeneralDataStructure.AreaStruct, Texture> OnProjectorAreaScan;
        public System.Action<List<GeneralDataStructure.AreaStruct>, int, int> OnTargetsAreaScan;

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
                OnTexture(Stream.Depth, value);
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
                OnTexture(Stream.Color, value);
            }
        }

        private void OnTexture(Stream stream_type, Texture p_texture) {
            if (stream_type == Stream.Depth) {
                SetDepthmapTexture(p_texture);
            }

            if (stream_type == Stream.Color) {
                SetColorTexture(p_texture);
            }
        }

        private void Update()
        {
            if (filterTexture == null || grayDepthMapTexture == null) return;

            Graphics.Blit(rawDepthMapTexture, grayDepthMapTexture, realsenseMat);

            Graphics.Blit(grayDepthMapTexture, imageProcessA, customizeMat, 0); // Guassian
            Graphics.Blit(imageProcessA, imageProcessB, customizeMat, 3); // Filter
            Graphics.Blit(imageProcessB, imageProcessA, customizeMat, 1); // Erode
            Graphics.Blit(imageProcessA, filterTexture, customizeMat, 2); // Dilate

            ExecEdgeProcessing();

            //if (debugAreaFlag)
            //    ExecProjectorContourProcessing();
        }


        public void ExecEdgeProcessing()
        {
            if (textureCopyFlag && filterTexture != null)
            {
                textureCopyFlag = false;
                AsyncGPUReadback.Request(filterTexture, 0, TextureFormat.RGB24, OnTexCompleteReadback);
            }
        }

        private void ExecProjectorContourProcessing()
        {
            if (rawColorTexture == null || colorMapTexture == null) return;

            Graphics.Blit(rawColorTexture, colorMapTexture); // Filter
            AsyncGPUReadback.Request(colorMapTexture, 0, TextureFormat.RGB24, OnColTexCompleteReadback);
        }

        private void OnTexCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.Log("GPU readback error detected.");
                return;
            }

            if (_depth2DProcessingTex == null) return;

            _depth2DProcessingTex.LoadRawTextureData(request.GetData<uint>());
            _depth2DProcessingTex.Apply();

            if (_segmentationAlgorithm != null) {
                var areas = _segmentationAlgorithm.FindAreaStruct(_depth2DProcessingTex.GetPixels());

                if (OnTargetsAreaScan != null)
                    OnTargetsAreaScan(areas, _depth2DProcessingTex.height, _depth2DProcessingTex.width);

                if (debugAreaFlag)
                    DrawAreaHint(_depth2DProcessingTex, Color.white, areas);
            }

            textureCopyFlag = true;
        }

        private void SetDepthmapTexture(Texture p_texture) {
            float aspectRatio = p_texture.height / (float)p_texture.width;
            int outputWidth = outputTexSize;
            int outputHeight = Mathf.FloorToInt(aspectRatio * outputTexSize);

            Debug.Log("OnTexture width " + outputWidth + ", " + outputHeight);

            if (_segmentationAlgorithm == null)
                _segmentationAlgorithm = new SegmentationAlgorithm(threshold_area: 120, width: outputWidth, height: outputHeight, offsetX: -3, offsetY: 2);

            if (grayDepthMapTexture == null)
                grayDepthMapTexture = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 16, RenderTextureFormat.R16);

            if (imageProcessA == null)
                imageProcessA = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            if (imageProcessB == null)
                imageProcessB = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            if (filterTexture == null)
                filterTexture = TextureUtility.GetRenderTexture(outputWidth, outputHeight, 8, RenderTextureFormat.ARGB32);

            if (_depth2DProcessingTex == null)
                _depth2DProcessingTex = new Texture2D(outputWidth, outputHeight, TextureFormat.RGB24, false);

            if (debugTexture_a != null)
                debugTexture_a.texture = p_texture;

            if (debugTexture_b != null)
                debugTexture_b.texture = _depth2DProcessingTex;
        }

        private void SetColorTexture(Texture p_texture)
        {
            float aspectRatio = p_texture.height / (float)p_texture.width;
            int resizeSize = 64;
            int outputWidth = resizeSize;
            int outputHeight = Mathf.FloorToInt(aspectRatio * resizeSize);

            if (_color2DProcessingTex == null)
                _color2DProcessingTex = new Texture2D(outputWidth, outputHeight, TextureFormat.RGB24, false);

            if (colorMapTexture == null)
                colorMapTexture = TextureUtility.GetRenderTexture(outputWidth, outputHeight, 8, RenderTextureFormat.ARGB32);

            if (colorTexture_canvas != null)
                colorTexture_canvas.texture = _color2DProcessingTex;

            StartCoroutine(Hsinpa.Utility.UtilityFunc.DoDelayCoroutineWork(1, () =>
            {
                ExecProjectorContourProcessing();
            }));
        }

        private void DrawAreaHint(Texture2D tex2D, Color color, List<GeneralDataStructure.AreaStruct> areaStructs) {
            //Debug.Log("Segmentation " + areaStructs.Count);

            foreach (GeneralDataStructure.AreaStruct areaStruct in areaStructs) {

                int radiusX = Mathf.RoundToInt(areaStruct.width * 0.5f);
                int radiusY = Mathf.RoundToInt(areaStruct.height * 0.5f);

                for (int x = -radiusX; x < radiusX; x++) {
                    int constX = Mathf.Clamp(areaStruct.x + x, 0, tex2D.width -1);
                    int upY = Mathf.Clamp(areaStruct.y + radiusY, 0, tex2D.height -1);
                    int downY = Mathf.Clamp(areaStruct.y - radiusY, 0, tex2D.height -1);

                    //Up
                    tex2D.SetPixel(constX, upY, color);

                    //Down
                    tex2D.SetPixel(constX, downY, color);
                }

                for (int y = -radiusY; y < radiusY; y++)
                {
                    int constY = Mathf.Clamp(areaStruct.y + y, 0, tex2D.height - 1);
                    int leftX = Mathf.Clamp(areaStruct.x - radiusX, 0, tex2D.width - 1);
                    int rightX = Mathf.Clamp(areaStruct.x + radiusX, 0, tex2D.width - 1);

                    //Left
                    tex2D.SetPixel(leftX, constY, color);

                    //Right
                    tex2D.SetPixel(rightX, constY, color);
                }
            }

            tex2D.Apply();
        }

        private async void OnColTexCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.Log("GPU readback error detected.");
                return;
            }

            if (_color2DProcessingTex == null) return;

            _color2DProcessingTex.LoadRawTextureData(request.GetData<uint>());
            _color2DProcessingTex.Apply();

            GeneralDataStructure.AreaStruct areaStruct =  await projectorSizeCorrector.ProcressFindProjectorSize(_color2DProcessingTex);

            DrawAreaHint(_color2DProcessingTex, Color.white, new List<GeneralDataStructure.AreaStruct>() { areaStruct });

            if (OnProjectorAreaScan != null)
                OnProjectorAreaScan(areaStruct, _color2DProcessingTex);
        }
    }
}
