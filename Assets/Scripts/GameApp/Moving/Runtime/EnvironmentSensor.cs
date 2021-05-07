using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Moving
{
    [ExecuteInEditMode]
    public class EnvironmentSensor : MonoBehaviour
    {
        private const int NearLimit = 100;
        
        [Range(0, 10)]
        public float senseRadius = 2f;

        [Range(0, 90)]
        public float angleTolerance = 10f;

        public int NearCount { get; private set; }

        public LayerMask senseLayer;
        public Collider[] Nears { get; } = new Collider[NearLimit];

        public IEnumerable<Collider> CurNears
        {
            get
            {
                for (var i = 0; i < NearCount; i++)
                {
                    yield return Nears[i];
                }
            }
        }

        public IEnumerable<AnchorSetHolder> CurHolders
        {
            get
            {
                foreach (var col in CurNears)
                {
                    yield return col.GetComponent<AnchorSetHolder>();
                }
            }
        }

        private void Update()
        {
            Sense();
        }

        private void Sense()
        {
            NearCount = Physics.OverlapSphereNonAlloc(transform.position, senseRadius, Nears, senseLayer, QueryTriggerInteraction.Ignore);
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var preColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            foreach (var col in CurNears)
            {
                Gizmos.DrawLine(transform.position, col.transform.position);
            }

            Gizmos.color = preColor;
        }
        #endif

        
    }
}