using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldClickSystem : TGTHMonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager input;
    [SerializeField] private GameObject clickEffectPrefab;
    public NavMeshPathFollower pathFollowerRB;
    public LayerMask whatIsEntity;
    public LayerMask whatIsGround;
    private bool canClick = false;
    protected override void Awake()
    {
        base.Awake();
        input = FindAnyObjectByType<InputManager>();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
        input.OnPointerPositionClick += (Vector2 pos) =>
        {
            if (!canClick) return;
            HandleClick(pos);
        };
    }
    private void OnPlayerExists(NetworkObject playerNet)
    {
        this.pathFollowerRB = playerNet.GetComponent<NavMeshPathFollower>();
        canClick = true;
    }
    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
    }
    private void HandleClick(Vector2 position)
    {
        Ray ray = mainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hitEntity, 100f, whatIsEntity))
        {
            if (hitEntity.collider.TryGetComponent<IWorldClickable>(out var clickable))
            {
                if (hitEntity.collider.TryGetComponent<NetworkObject>(out var netObj))
                {
                    if (netObj.IsPlayerObject && netObj.IsOwner)
                        return;
                }

                clickable.OnClicked();
                return;
            }
        }

        if (pathFollowerRB == null) return;
        if (Physics.Raycast(ray, out RaycastHit hitGround, 100, whatIsGround))
        {
            var pos = pathFollowerRB.transform.position;
            if (NavMeshPathUtility.TryGetCorners(pos, hitGround.point, out List<Vector3> orners))
            {
                pathFollowerRB.SetPath(orners);
                pathFollowerRB.Move();
                ShowClickEffect(hitGround.point);
                PathVisualizer.Instance.Draw(orners);
            }
        }
    }
    private void ShowClickEffect(Vector3 pos)
    {
        GameObject fx = ObjectPool.Instance.GetObject(clickEffectPrefab, pos, Quaternion.identity);

        ObjectPool.Instance.ReturnObject(fx, .5f);
    }
}
