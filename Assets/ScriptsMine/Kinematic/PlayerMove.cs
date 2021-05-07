using System;
using System.Collections;
using UnityEngine;

public class PlayerKinmeticMove : MonoBehaviour {

    public void StartMoveToPosition(Vector3 pos, float speed, Action onMoveFinish = null)
    {
        StartCoroutine(MoveToPosition(pos, speed, onMoveFinish));
    }

    private IEnumerator MoveToPosition(Vector3 pos, float speed, Action onMoveFinish = null)
    {
        var agent = transform;
        while (Vector3.Distance(agent.position, pos) > MoveConst.STOP_DIST)
        {
            Vector3 move = (pos - agent.position).normalized * Time.deltaTime * speed;
            agent.position += move;
            yield return null;
        }
        if (onMoveFinish != null)
        {
            onMoveFinish();
        }
    }

}
