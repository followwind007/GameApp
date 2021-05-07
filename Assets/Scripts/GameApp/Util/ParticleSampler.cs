
using UnityEngine;
 
public class ParticleSystemReverseSimulationSuperSimple : MonoBehaviour
{
    private ParticleSystem[] _particleSystems;

    private float[] _simulationTimes;
 
    public float startTime = 2.0f;
    public float simulationSpeedScale = 1.0f;

    private void Initialize()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>(false);
        _simulationTimes = new float[_particleSystems.Length];
    }

    private void OnEnable()
    {
        if (_particleSystems == null)
        {
            Initialize();
        }
 
        for (var i = 0; i < _simulationTimes.Length; i++) { _simulationTimes[i] = 0.0f; } _particleSystems[0].Simulate(startTime, true, false, true); }

    private void Update() { _particleSystems[0].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); for (var i = _particleSystems.Length - 1; i >= 0; i--)
        {
            var useAutoRandomSeed = _particleSystems[i].useAutoRandomSeed;
            _particleSystems[i].useAutoRandomSeed = false;
 
            _particleSystems[i].Play(false);
 
            var deltaTime = _particleSystems[i].main.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _simulationTimes[i] -= (deltaTime * _particleSystems[i].main.simulationSpeed) * simulationSpeedScale;
 
            var currentSimulationTime = startTime + _simulationTimes[i];
            _particleSystems[i].Simulate(currentSimulationTime, false, false, true);
 
            _particleSystems[i].useAutoRandomSeed = useAutoRandomSeed;
 
            if (currentSimulationTime < 0.0f)
            {
                _particleSystems[i].Play(false);
                _particleSystems[i].Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}