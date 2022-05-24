using Hsinpa.AI.Flocking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Hsinpa.Ultimate.Scrollview
{
    [CreateAssetMenu(fileName = "FishCreatures", menuName = "Tools/FishPool/PoolSRP", order = 1)]
    public class FishCreaturePoolSRP : ScriptableObject
    {
        [SerializeField]
        private List<FlockGameObject> flockGameObjects;

        public FlockGameObject GetRandomFlockObject() {
            if (flockGameObjects.Count > 0) {
                int randomIndex = UnityEngine.Random.Range(0, flockGameObjects.Count);
                return flockGameObjects[randomIndex];
            }

            return null;
        }
    }
}
