using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    Patrol,
    Chase,
    Attack
}

public class AIFSM : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public AIState currentState = AIState.Patrol;
    private Vector3 targetPosition;

    private List<Vector3> path;
    private int pathIndex = 0;
    private Vector3 lastPlayerPosition;
    private bool isPathfinding = false;

    public bool check = false;

    public LayerMask obstacleMask;

    private void Awake()
    {
        ManagerByAStar.Instance.GetAI(this);
    }

    void Update()
    {
        if (!check) return;

        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"[AI] Player Distance: {distanceToPlayer} (Detection Range: {detectionRange})");

        if (distanceToPlayer < detectionRange)
        {
            Debug.Log("[AI] Player Detected! Switching to Chase Mode.");
            currentState = AIState.Chase;
            RequestPath();
        }
    }

    void Chase()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Vector3.Distance(lastPlayerPosition, player.position) > 0.5f)
        {
            Debug.Log("[AI] Player moved! Updating path...");
            RequestPath();
        }

        if (path != null && pathIndex < path.Count)
        {
            Vector3 nextPosition = path[pathIndex];

            if (!IsPathClear(transform.position, nextPosition))
            {
                Debug.Log("[AI] Obstacle detected! Recalculating path...");
                RequestPath();
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPosition) < 0.5f)
            {
                pathIndex++;
            }
        }

        if (distanceToPlayer < attackRange)
        {
            Debug.Log("[AI] Entering Attack Mode!");
            currentState = AIState.Attack;
        }
    }

    bool IsPathClear(Vector3 start, Vector3 end)
    {
        RaycastHit hit;
        if (Physics.Linecast(start, end, out hit, obstacleMask))
        {
            Debug.Log("[AI] Obstacle Detected: " + hit.collider.name);
            return false;
        }
        return true;
    }

    void Attack()
    {
        Debug.Log("Attacking player!");
        currentState = AIState.Patrol;
    }

    void RequestPath()
    {
        if (GridManager.Instance == null) return;
        path = AStarPathfinding.FindPath(transform.position, player.position, GridManager.Instance);
        pathIndex = 0;
        lastPlayerPosition = player.position;
        isPathfinding = true;
    }
}
