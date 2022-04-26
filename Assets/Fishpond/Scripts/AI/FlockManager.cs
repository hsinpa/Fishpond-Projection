using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Hsinpa.AI.Flocking
{
    public class FlockManager : MonoBehaviour
    {
        [SerializeField]
        private FlockAgent _flockAgentPrefab;

        private List<FlockColliderStruct> _colliders = new List<FlockColliderStruct>();
        private List<FlockAgent> _flockAgents = new List<FlockAgent>();
        public IReadOnlyCollection<FlockAgent> FlockAgents => this._flockAgents;

        private Vector2 _pondSize;
        private Vector3 _pondSize3D;
        FlockEnvStruct _flockEnvStruct;

        private int flockIncrementalID = 0;
        private int colliderIncrementalID = 0;

        #region Public API
        public void Init(Vector2 pondSize, int spawnCount, FlockEnvStruct flockEnvStruct ) {
            this._pondSize = pondSize;
            this._pondSize3D = new Vector3(this._pondSize.x, 0.1f, this._pondSize.y);

            _flockEnvStruct = flockEnvStruct;

            PreparePondStage(spawnCount);
        }

        public void RegisterCollider(FlockColliderStruct collider) {
            collider.id = colliderIncrementalID;

            _colliders.Add(collider);

            colliderIncrementalID++;
        }

        public void RemoveCollider(FlockColliderStruct collider) {
            int l = _colliders.Count;

            for (int i = l - 1; i >= 0; i--) {
                if (_colliders[i].id == collider.id)
                    _colliders.RemoveAt(i);
            }
        }
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
                var flockAgent =  CreateFlockAgent(position, velocity);

                _flockAgents.Add(flockAgent);
            }
        }

        private FlockAgent CreateFlockAgent(Vector3 position, Vector3 velocity) {

            velocity = velocity.normalized;

            var flockAgent = GameObject.Instantiate<FlockAgent>(_flockAgentPrefab, position, Quaternion.identity, this.transform);

            flockAgent.SetUp(this.flockIncrementalID, velocity, this._flockEnvStruct);

            this.flockIncrementalID++;

            return flockAgent;
        }

        #endregion

        #region Monobehavior

        private void Update()
        {
            if (_flockAgents == null) return;

            int flockCount = _flockAgents.Count;
            var flockDataSets = _flockAgents.Select(x => x.flockDataStruct).ToList();
            for (int i = 0; i < flockCount; i++) {
                _flockAgents[i].OnUpdate(flockDataSets, _colliders);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, this._pondSize3D);
        }
        #endregion

    }
}