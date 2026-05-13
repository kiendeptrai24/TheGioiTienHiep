using UnityEngine;

public class DamagePopupSpawner : Singleton<DamagePopupSpawner>
{
    public GameObject popupPrefab;
    public Canvas uiCanvas;

    public void Spawn(int damage, Transform target, bool crit)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 2);

        GameObject go = Instantiate(popupPrefab, uiCanvas.transform);
        go.transform.position = pos;

        var popup = go.GetComponent<DamagePopup>();
        popup.Init(damage, crit);
    }
}