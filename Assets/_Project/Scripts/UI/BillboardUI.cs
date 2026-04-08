using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private Transform target; // nhân vật
    [SerializeField] private Vector3 offset = new Vector3(.5f, 2f, 0);
    private void Awake()
    {
        mainCam = Camera.main;
        target = gameObject.transform.root;
        offset =  new Vector3(.5f, 2f, 0);
    }

    private void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.LookAt(transform.position + mainCam.transform.forward);
    }
}