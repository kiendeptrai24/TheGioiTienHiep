using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SET : MonoBehaviour
{
    private struct Row
    {
        public string left;
        public string right;
        public Row(string l, string r)
        {
            left = l;
            right = r;
        }
    }

    private List<Row> rows = new List<Row>
    {
        // ===== CỘT TRÁI =====
        new Row("Sát Thương Linh Thể", "500"),
        new Row("Sát Thương Linh Lực", "200"),
        new Row("Sát Thương Linh Thức", "200"),
        new Row("Sát Thương Chí Mạng", "3"),
        new Row("Tỉ Lệ Chí Mạng", "30%"),
        new Row("Tốc Độ Đánh", "1.2"),
        new Row("Sát Thương Chuẩn", "Đang cập nhật"),
        new Row("Xuyên Phòng Ngự", "Đang cập nhật"),
        new Row("Hút Sinh Lực", "Đang cập nhật"),

        // ===== CỘT PHẢI =====
        new Row("Sinh Lực", "20k"),
        new Row("Linh Lực", "5k"),
        new Row("Linh Thức", "2k"),
        new Row("Phục Hồi Sinh Lực / Phút", "1%"),
        new Row("Phục Hồi Linh Lực / Phút", "1%"),
        new Row("Phòng Ngự Linh Thể", "5%"),
        new Row("Phòng Ngự Linh Lực", "5%"),
        new Row("Phòng Ngự Linh Thức", "5%"),
        new Row("Giảm Sát Thương Chí Mạng", "Đang cập nhật"),
    };

    void Start()
    {
        AutoSet();
    }
    [ContextMenu("dsdad")]
    public void AutoSet()
    {
        int count = Mathf.Min(transform.childCount, rows.Count);

        for (int i = 0; i < count; i++)
        {
            var texts = transform.GetChild(i)
                .GetComponentsInChildren<TextMeshProUGUI>(true);

            if (texts.Length < 2) continue;

            // trái / phải theo vị trí X
            TextMeshProUGUI left =
                texts[0].rectTransform.anchoredPosition.x <
                texts[1].rectTransform.anchoredPosition.x
                ? texts[0]
                : texts[1];

            TextMeshProUGUI right =
                left == texts[0] ? texts[1] : texts[0];

            left.text = rows[i].left;
            right.text = rows[i].right;
        }
    }
}
