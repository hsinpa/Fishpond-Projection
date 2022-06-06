using Hsinpa.Realsense;
using Hsinpa.Utility.Algorithm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Water2DTool;

namespace Hsinpa.AI.Flocking
{
    public class FlockingSampleCode : MonoBehaviour
    {
        [SerializeField]
        private Camera targetCamera;

        [SerializeField]
        private FlockManager flockManager;

        [SerializeField]
        private Transform debugColliderHolder;

        [SerializeField]
        private Vector2 PondSize;

        [SerializeField, Range(0, 200)]
        private int SpawnCount;

        [SerializeField, Range(0.1f, 20f)]
        private float Sensitivity;

        [SerializeField, Range(0f, 5f)]
        private float AgentMaxSpeed;

        [SerializeField, Range(0f, 5f)]
        private float AgentMinSpeed;

        [SerializeField, Range(0f, 10f)]
        private float AgendEscapeValue;

        [SerializeField, Range(0, 10)]
        private int MaxRipplePerFrame = 5;

        [SerializeField]
        private RealsenseCtrl realsenseCtrl;

        [SerializeField]
        private Water2D_Ripple water2D_Ripple;

        private SpaceTransformAlgorithm spaceTransformAlgorithm;

        void Start()
        {
            FlockEnvStruct flockEnvStruct = new FlockEnvStruct();
            flockEnvStruct.max_speed = AgentMaxSpeed;
            flockEnvStruct.min_speed = AgentMinSpeed;
            flockEnvStruct.waypoint_spawn_time = 0.5f;

            flockEnvStruct.escape_multiplier = AgendEscapeValue;
            flockEnvStruct.sense_range = Sensitivity;
            flockEnvStruct.centerWorldPos = this.transform.position;
            flockEnvStruct.centerRadius = PondSize.magnitude * 0.5f;

            flockManager.Init(PondSize, SpawnCount, flockEnvStruct);

            FlockDebugCollider[] debugColliders = debugColliderHolder.GetComponentsInChildren<FlockDebugCollider>();
            var colliders = debugColliders.Select(x => x.FlockColliderStruct).ToList();

            flockManager.SetColliders(colliders);
            flockManager.OnCollideEvent += OnCollideRipple;

            realsenseCtrl.OnProjectorAreaScan += OnProjectorAreaScan;
            realsenseCtrl.OnTargetsAreaScan += OnProjectorAreaScan;
        }

        private void OnProjectorAreaScan(GeneralDataStructure.AreaStruct areaStruct, Texture fullTex) {
            if (spaceTransformAlgorithm != null) return;

            float height = Camera.main.orthographicSize * 2.0f;
            float width = height * Camera.main.aspect;

            Debug.Log("Camera " + width + ", " + height);
            spaceTransformAlgorithm = new SpaceTransformAlgorithm(Camera.main.transform.position, width, height, fullTex.width, fullTex.height, areaStruct);
        }

        private void OnProjectorAreaScan(List<GeneralDataStructure.AreaStruct> areaStructs, int spaceHeight, int spaceWidth)
        {
            if (spaceTransformAlgorithm == null) return;
            var colliders = areaStructs.Select(area => {

                Vector4 transfromArea = spaceTransformAlgorithm.ToGameWorldSpace(area, spaceHeight, spaceWidth);

                return new FlockColliderStruct()
                {
                    position = new Vector3(transfromArea.x, 0, transfromArea.y),
                    height = transfromArea.w,
                    width = transfromArea.z
                };


            }).ToList();

            flockManager.SetColliders(colliders);
        }

        private void OnCollideRipple(List<Vector3> collidePoints) {
            int cLens = collidePoints.Count;

            for (int i = 0; i < cLens; i++)
            {
                if (i >= MaxRipplePerFrame) return;
                water2D_Ripple.AddRippleAtPosition(collidePoints[i], 2, 2);
            }
        }
    }
}