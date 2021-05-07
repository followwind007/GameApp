using UnityEngine;
using UnityEngine.Playables;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ParticlePlayableBehaviour : PlayableBehaviour
    {
        public GameObject Target { get; set; }

        [PathRef(typeof(GameObject))]
        public string particlePath;

        public Vector3 offset = Vector3.zero;

        private GameObject _particleInst;
        private ParticleSystem _particleSys;

        private const uint RandomSeed = 0xffffffff;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            InitParticle();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (Application.isPlaying || _particleInst == null || _particleSys == null) return;
            if (!_particleSys.gameObject.activeInHierarchy) return;

            var time = (float)playable.GetTime();

            // Edit mode: Re-prepare the particle system every frame.
            if (!_particleSys.isPlaying) PrepareParticleSystem(playable);
            _particleSys.Simulate(time, true);
        } 

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            DestroyParticle();
        }

        private void InitParticle()
        {
            if (string.IsNullOrEmpty(particlePath) || Target == null) return;
            Vector3 particlePosition = Target.transform.position + offset;
            GameObject particle = PlayableLoader.LoadAssetAtPath<GameObject>(particlePath);
            if (!Application.isPlaying)
            {
                particle.hideFlags = HideFlags.DontSave;
            }
            _particleInst = Object.Instantiate(particle, particlePosition, particle.transform.rotation);
            _particleSys = _particleInst.GetComponent<ParticleSystem>();
        }

        private void DestroyParticle()
        {
            if (_particleInst)
            {
                Object.DestroyImmediate(_particleInst);
                _particleSys = null;
            }
        }

        private void PrepareParticleSystem(Playable playable)
        {
            // Disable automatic random seed to get deterministic results.
            if (_particleSys.useAutoRandomSeed)
                _particleSys.useAutoRandomSeed = false;

            // Override the random seed number.
            if (_particleSys.randomSeed != RandomSeed)
                _particleSys.randomSeed = RandomSeed;

            // Retrieve the total duration of the track.
            var rootPlayable = playable.GetGraph().GetRootPlayable(0);
            var duration = (float)rootPlayable.GetDuration();

            // Particle system duration should be longer than the track duration.
            var main = _particleSys.main;
            if (main.duration < duration) main.duration = duration;
        }

    }

}