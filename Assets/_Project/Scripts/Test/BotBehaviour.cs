using Unity.Netcode;
using UnityEngine;

public class BotBehaviour : NetworkBehaviour
{
    [SerializeField] private float moveInterval = 0.2f;
    [SerializeField] private float actionInterval = 2f;
    [SerializeField] private float moveSpeed = 3f;

    private float moveTimer;
    private float actionTimer;

    private void Update()
    {
        if (!IsOwner) return;

        moveTimer += Time.deltaTime;
        actionTimer += Time.deltaTime;

        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;

            Vector2 input = Random.insideUnitCircle.normalized;
            BotMoveServerRpc(input);
        }
    }

    [ServerRpc]
    private void BotMoveServerRpc(Vector2 input)
    {
        Vector3 dir = new Vector3(input.x, 0f, input.y);

        if (dir.sqrMagnitude <= 0.001f)
            return;

        transform.position += dir.normalized * moveSpeed * moveInterval;
        transform.forward = dir.normalized;
    }
    private void Move(Vector2 input)
    {
        Vector3 dir = new Vector3(input.x, 0f, input.y);

        if (dir.sqrMagnitude <= 0.001f)
            return;

        transform.position += dir.normalized * moveSpeed * moveInterval;
        transform.forward = dir.normalized;
    }
}