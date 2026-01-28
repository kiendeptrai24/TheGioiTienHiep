using UnityEngine;
using UnityEngine.UI;

public class LoadingSlider : MonoBehaviour
{
    [SerializeField] private Slider processSlider;
    private float smoothSpeed = 15f;

    private SceneLoadManager sceneLoadManager;
    private float targetValue;

    private void Awake()
    {
        sceneLoadManager = SceneLoadManager.Instance;
        sceneLoadManager.process += OnProcessChanged;
    }

    private void OnDestroy()
    {
        if (sceneLoadManager != null)
            sceneLoadManager.process -= OnProcessChanged;
    }

    private void OnProcessChanged(float value)
    {
        targetValue = value;
    }

    private void Update()
    {
        if (processSlider == null) return;

        processSlider.value = Mathf.Lerp(
            processSlider.value,
            targetValue,
            Time.deltaTime * smoothSpeed
        );
    }
}
