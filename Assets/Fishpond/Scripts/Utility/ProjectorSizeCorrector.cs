using Hsinpa.Utility.Algorithm;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Realsense
{
    public class ProjectorSizeCorrector : MonoBehaviour
    {
        [SerializeField]
        private float WaitTime;

        [SerializeField]
        private Image GreenShotCanvas;

        FindCenterBoxAlgorithm findCenterBoxAlgorithm;

        public void Start()
        {
            findCenterBoxAlgorithm = new FindCenterBoxAlgorithm();
            GreenShotCanvas.gameObject.SetActive(true);
        }

        public async Task<GeneralDataStructure.AreaStruct> ProcressFindProjectorSize(Texture2D colTex) {
            //await Task.Delay(System.TimeSpan.FromSeconds(WaitTime));
            GeneralDataStructure.AreaStruct areaStruct = await findCenterBoxAlgorithm.FindSize(colTex.GetPixels(), colTex.width, colTex.height);
            GreenShotCanvas.gameObject.SetActive(false);

            //Debug.Log($"x {areaStruct.x}, y {areaStruct.y}, height {areaStruct.height}, width {areaStruct.width}, area {areaStruct.area}");

            return areaStruct;
        }
    }
}