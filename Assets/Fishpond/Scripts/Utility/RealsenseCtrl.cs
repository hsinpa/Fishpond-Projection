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
        private MultiThreadProcess multiThreadProcess;

        [SerializeField, Range(0, 1)]
        private float depthmapThreshold;

        [SerializeField]
        private int outputTexSize;

        [SerializeField]
        private Material realsenseMat;

        [SerializeField]
        private Material customizeMat;

        private Texture rawDepthMapTexture;

        private RenderTexture imageProcessA;
        private RenderTexture imageProcessB;
        private RenderTexture grayDepthMapTexture;
        private RenderTexture filterTexture;

        private Texture2D _imgProcessingTex;
        private FloodfillAlgorithm _floodfillAlgorithm;

        private float aspectRatio = 0;
        private bool textureCopyFlag = true;

        public Texture texture
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

        private void Start()
        {
            _floodfillAlgorithm = new FloodfillAlgorithm(threshold_area : 1);
        }

        private void OnTexture(Texture p_texture) {
            aspectRatio = p_texture.height / (float)p_texture.width;
            int outputWidth = outputTexSize;
            int outputHeight = Mathf.FloorToInt(aspectRatio * outputTexSize);

            Debug.Log("OnTexture width " + outputWidth +", " + outputHeight);


            if (grayDepthMapTexture == null)
                grayDepthMapTexture = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 16, RenderTextureFormat.R16);

            if (imageProcessA == null)
                imageProcessA = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            if (imageProcessB == null)
                imageProcessB = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            if (filterTexture == null) {
                filterTexture = TextureUtility.GetRenderTexture(outputWidth, outputHeight, 8, RenderTextureFormat.ARGB32);
            }

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
            Graphics.Blit(imageProcessB, imageProcessA, customizeMat, 1); // Erode

            Graphics.Blit(imageProcessA, filterTexture, customizeMat, 1); // Erode

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

            _floodfillAlgorithm.FindAreaStruct(_imgProcessingTex.GetPixels(), _imgProcessingTex.width, _imgProcessingTex.height);

            textureCopyFlag = true;
        }
    }
}
