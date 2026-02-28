using UnityEngine;

public class BattleCameraController : MonoBehaviour
{
    public static BattleCameraController Instance { get; private set; }

    [SerializeField] private Camera cam;
    [SerializeField] private Transform worldFollowTarget; // player camera target
    private Vector3 _savedPos;
    private Quaternion _savedRot;

    private void Awake() => Instance = this;

    public void LookAtBattle(Vector3 islandPos)
    {
        if (!cam) cam = Camera.main;
        _savedPos = cam.transform.position;
        _savedRot = cam.transform.rotation;

        cam.transform.position = islandPos + new Vector3(0, 12f, -12f);
        cam.transform.LookAt(islandPos);
    }

    public void ReturnToWorld()
    {
        if (!cam) cam = Camera.main;
        cam.transform.SetPositionAndRotation(_savedPos, _savedRot);
        // hoặc follow lại worldFollowTarget nếu bạn dùng Cinemachine
    }
}
