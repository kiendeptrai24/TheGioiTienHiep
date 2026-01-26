namespace TGTH.Mobile
{
    using UnityEngine;
    using static PathTest;

    public class MapSearchResultPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapSearchResultPageView view;
        [SerializeField] private PathTest pathTest;
        [SerializeField] private RectTransform arrowRoot;
        [SerializeField] private RectTransform arrowIcon; // hình mũi tên (con)
        [SerializeField] private float radius = 200f;
        [SerializeField] private Transform target;
        private FindPathResult result;
        protected override void Awake()
        {
            base.Awake();
            view.OnOkClicked += OnOkClicked;
            view.OnCancelClicked += OnCancelClicked;
        }

        private void OnOkClicked()
        {
            pathTest.StartFollowPath();
        }
        private void Update()
        {
            if (result == null) return;

            Vector3 to = pathTest.mapSpawn.GridToWorld(result.goal);
            Vector3 dir = to - target.position;

            Vector2 d = new Vector2(dir.x, dir.z);
            if (d.sqrMagnitude < 0.0001f) return;

            d.Normalize();

            // góc hướng tới goal (trên mặt phẳng XZ)
            float angleZ = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;

            // nếu sprite mũi tên mặc định chỉ "lên" thì mở dòng này:
            // angleZ -= 90f;

            // 1) quay root
            arrowRoot.localEulerAngles = new Vector3(0f, 0f, angleZ);

            // 2) đặt icon cách tâm đúng radius theo trục X local của root
            // (vì root đã quay nên icon sẽ nằm đúng hướng)
            arrowIcon.anchoredPosition = new Vector2(radius, 0f);
        }

        private void OnCancelClicked()
        {

        }
        public void ShowData(FindPathResult result, ItemData itemData)
        {
            this.result = result;
            view.ShowData(result, itemData);
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<MapSearchResultPageView>();
        }
    }
}
