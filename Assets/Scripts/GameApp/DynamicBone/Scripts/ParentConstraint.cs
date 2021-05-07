#define NO_NEED_ALWAYS_ANIM
using UnityEngine;
using System.Collections;

public class ParentConstraint : MonoBehaviour {
	public Transform[] parents;
	public float[] weights;
	public Vector3 constrainedPosition;
	public Vector3 constrainedForward;
	public Vector3 constrainedUp;
	Transform t;
	Vector3[] pos, forward, up;
	int pc;

	public void UpdateConstraint() {
		int i;
		Vector3 p = Vector3.zero, f = Vector3.zero, u = Vector3.zero;
		float cw = weights[0];
		p = parents[0].TransformPoint(pos[0]);
		f = parents[0].TransformDirection(forward[0]);
		u = parents[0].TransformDirection(up[0]);
		for (i=1; i<pc; i++) {
			p = Vector3.Lerp(p, parents[i].TransformPoint(pos[i]), weights[i]/(cw+weights[i]));
			f = Vector3.Lerp(f, parents[i].TransformDirection(forward[i]), weights[i]/(cw+weights[i]));
			u = Vector3.Lerp(u, parents[i].TransformDirection(up[i]), weights[i]/(cw+weights[i]));
			cw += weights[i];
		}
		constrainedPosition = p;
		constrainedForward = f;
		constrainedUp = u;

        t.position = constrainedPosition;
        t.rotation = Quaternion.LookRotation(constrainedForward, constrainedUp);
	}

	void Awake() {
		t = transform;
	}
	
	// Use this for initialization
	void Start () {
		int i;
		pc = parents.Length;
		pos = new Vector3[pc];
		forward = new Vector3[pc];
		up = new Vector3[pc];
		for (i=0; i<pc; i++) {
			pos[i] = parents[i].InverseTransformPoint(t.position);
			forward[i] = parents[i].InverseTransformDirection(t.forward);
			up[i] = parents[i].InverseTransformDirection(t.up);
		}
	}

#if NO_NEED_ALWAYS_ANIM
    bool IsNeedUpdate()
    {
        if(Camera.main == null)
        {
            return false;
        }

        if(10 > Vector3.Distance(Camera.main.transform.position,t.position))
        {
            return true;
        }

        return false;
    }
#endif
	// Update is called once per frame
	void LateUpdate () {
#if NO_NEED_ALWAYS_ANIM
        if(!IsNeedUpdate())
        {
            return;
        }
#endif

		//calculate current position based on initial local positions;
		UpdateConstraint();
	}
}