using UnityEngine;

[RequireComponent(typeof(ParticleSystemRenderer))]
public class ParticleOrderHelper : MonoBehaviour
{
    [Header("default will find canvas in parent")]
    public Canvas canvas;

    public int orderOffset;
    
    private void OnEnable()
    {
        SortParticle();
    }

    public void SortParticle()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;
        }
        var render = GetComponent<Renderer>() as ParticleSystemRenderer;
        if (render == null)
        {
            return;
        }

        render.sortingOrder = canvas.sortingOrder + orderOffset;
    }
    
}
