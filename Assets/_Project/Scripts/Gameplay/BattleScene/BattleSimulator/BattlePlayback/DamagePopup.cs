using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    public float floatSpeed = 50f;
    public float duration = 0.8f;
    public Vector3 randomOffset = new Vector3(30f, 0, 0);

    private Vector3 moveDir;
    private float timer;

    public void Init(int damage, bool crit)
    {
        text.text = damage.ToString();

        if (crit)
        {
            text.color = Color.yellow;
            text.fontSize *= 1.3f;
        }

        moveDir = new Vector3(
            Random.Range(-0.5f, 0.5f),
            1f,
            0
        ).normalized;

        transform.localPosition += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            0,
            0
        );
    }

    void Update()
    {
        timer += Time.deltaTime;

        transform.localPosition += moveDir * floatSpeed * Time.deltaTime;

        canvasGroup.alpha = 1f - (timer / duration);

        transform.localScale = Vector3.one * (1f + (1f - timer / duration) * 0.3f);

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}