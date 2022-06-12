using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticleEffect {
    public class SeasonParticleSystem : MonoBehaviour
    {
        [SerializeField]
        private List<ParticleStruct> particleStructs;

        private ParticleSystem g_particle;

        private void Start()
        {
            if (particleStructs != null && particleStructs.Count > 0)
                Instantiate(particleStructs[0].ParticleSystem, this.transform);

            Hsinpa.Utility.SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
        }

        private void OnSimpleEventSystem(int id, params object[] customObjects) {
            if (id == MessageEventFlag.HsinpaEvent.Season.ExecSeasonState) {
                string seasonID = (string) customObjects[0];
                if (g_particle != null) {
                    Destroy(g_particle);
                    g_particle = null;
                }

                var findPStruct = particleStructs.Find(x => x.id == seasonID);
                if (!string.IsNullOrEmpty(findPStruct.id))
                    Instantiate(particleStructs[0].ParticleSystem, this.transform);
            }
        }

        [System.Serializable]
        public struct ParticleStruct {
            public string id;
            public ParticleSystem ParticleSystem;
        }
    }
}
