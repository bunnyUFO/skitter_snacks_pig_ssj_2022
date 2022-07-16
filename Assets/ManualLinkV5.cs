using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManualLinkV5 : MonoBehaviour
{
    NavMeshAgent agent;
    bool MoveAcrossNavMeshesStarted = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.isOnOffMeshLink && !MoveAcrossNavMeshesStarted)
        {
            StartCoroutine(MoveAcrossNavMeshLink());
            MoveAcrossNavMeshesStarted = true;
        }

        IEnumerator MoveAcrossNavMeshLink()
        {
            Debug.Log("Is on OffMesh link");
            OffMeshLinkData data = agent.currentOffMeshLinkData;
            agent.updateRotation = false;

            Vector3 startPos = agent.transform.position;
            Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
            float duration = (endPos - startPos).magnitude / agent.velocity.magnitude;
            float t = 0.0f;
            float tStep = 1.0f / duration;
            while (t < 1.0f)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                agent.destination = transform.position;
                t += tStep * Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
            agent.updateRotation = true;
            agent.CompleteOffMeshLink();
            MoveAcrossNavMeshesStarted = false;
            Debug.Log("Has finished OffMesh link");
        }
    }
}