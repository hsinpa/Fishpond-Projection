using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hsinpa.Utility;

namespace Hsinpa.AI.Flocking
{
    [RequireComponent(typeof(MultiThreadProcess))]
    public class FlockManager : MonoBehaviour
    {
        [SerializeField]
        private FlockGameObject _flockAgentPrefab;



        private List<FlockColliderStruct> _colliders = new List<FlockColliderStruct>();
        private List<FlockAgent> _flockAgents = new List<FlockAgent>();
        private List<FlockGameObject> _flockGameObjs = new List<FlockGameObject>();
        private MultiThreadProcess multiThreadProcess;

        public IReadOnlyCollection<FlockAgent> FlockAgents => this._flockAgents;

        private Vector2 _pondSize;
        private Vector3 _pondSize3D;
        FlockEnvStruct _flockEnvStruct;

        private int flockIncrementalID = 0;
        private int colliderIncrementalID = 0;

        public static float TIME;

        #region Public API
        public void Init(Vector2 pondSize, int spawnCount, FlockEnvStruct flockEnvStruct ) {
            this.multiThreadProcess = gameObject.GetComponent<MultiThreadProcess>();

            this._pondSize = pondSize;
            this._pondSize3D = new Vector3(this._pondSize.x, 0.1f, this._pondSize.y);

            _flockEnvStruct = flockEnvStruct;

            PreparePondStage(spawnCount);

            System.Threading.Thread sender = new System.Threading.Thread(ProcessCalculation);
            sender.Start();
        }

        public void SetColliders(List<FlockColliderStruct> colliders) {
            _colliders.Clear();
            _colliders.AddRange(colliders);
        }

        //public void RegisterCollider(FlockColliderStruct collider) {
        //    collider.id = colliderIncrementalID;

        //    _colliders.Add(collider);

        //    colliderIncrementalID++;
        //}

        //public void RemoveCollider(FlockColliderStruct collider) {
        //    int l = _colliders.Count;

        //    for (int i = l - 1; i >= 0; i--) {
        //        if (_colliders[i].id == collider.id)
        //            _colliders.RemoveAt(i);
        //    }
        //}
        #endregion

        #region Private API
        private void PreparePondStage(int spawnCount)
        {
            _flockAgents = new List<FlockAgent>();

            float radiusWidth = this._pondSize.x / 2f;
            float radiusHeight = this._pondSize.y / 2f;
            float y = 0f;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 position = new Vector3(
                        Random.Range(-radiusWidth, radiusWidth),
                        y,
                        Random.Range(-radiusHeight, radiusHeight)
                    );

                Vector3 velocity = new Vector3(
                    Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)
                );

                velocity = velocity.normalized;

                var flockObject = CreateFlockAgent(position);

                var flockAgent = new FlockAgent(this.flockIncrementalID, position, velocity, this._flockEnvStruct);

                flockObject.SetUp(flockAgent);

                _flockGameObjs.Add(flockObject);
                _flockAgents.Add(flockAgent);
            }
        }

        private FlockGameObject CreateFlockAgent(Vector3 position) {
            var flockObject = GameObject.Instantiate<FlockGameObject>(_flockAgentPrefab, position, Quaternion.identity, this.transform);

            this.flockIncrementalID++;

            return flockObject;
        }

        #endregion

        #region Monobehavior

        private void Update()
        {
            TIME = Time.time;

            int flockCount = _flockGameObjs.Count;

            for (int i = 0; i < flockCount; i++)
            {
                _flockGameObjs[i].UpdateTransform();
            }

            multiThreadProcess.Enqueue(ProcessCalculation);
        }

        private void ProcessCalculation() {
            if (_flockAgents == null) return;

            int flockCount = _flockAgents.Count;
            for (int i = 0; i < flockCount; i++)
            {
                _flockAgents[i].OnUpdate(_flockAgents, _colliders);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, this._pondSize3D);
        }

        private void OnDrawGizmos()
        {
            if (_colliders == null) return;
            Gizmos.color = Color.green;

            foreach (var collider in _colliders)
            {
                Gizmos.DrawWireCube(collider.position, new Vector3(collider.width, 0,collider.height));
            }
        }
        #endregion

    }
}