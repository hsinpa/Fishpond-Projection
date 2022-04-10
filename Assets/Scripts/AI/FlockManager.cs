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

        private List<FlockAgent> _flockAgents = new List<FlockAgent>();
        public IReadOnlyCollection<FlockAgent> FlockAgents => this._flockAgents;

        private Vector2 _pondSize;
        private Vector3 _pondSize3D;
        private float _sense_range;
        private int incrementalID = 0;

        #region Public API
        public void Init(Vector2 pondSize, int spawnCount, float sense_range) {
            this._pondSize = pondSize;
            this._pondSize3D = new Vector3(this._pondSize.x, 0.1f, this._pondSize.y);

            PreparePondStage(spawnCount);
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

            flockAgent.SetUp(this.incrementalID, _sense_range, velocity);

            this.incrementalID++;

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
                _flockAgents[i].OnUpdate(flockDataSets);
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