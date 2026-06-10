namespace TGTH.Mobile
{
    using System.Collections.Generic;
    using UnityEngine;
    using static PathFinding;

    public class MapSearchResultPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapSearchResultPageView view;
        [SerializeField] private PathFinding pathFinding;
        [SerializeField] private RectTransform arrowRoot;
        [SerializeField] private RectTransform arrowIcon; // hình mũi tên (con)
        [SerializeField] private float radius = 200f;
        [SerializeField] private Transform target;
        private List<FindPathResult> findPathResults;
        private int currentIndex = 0;
        private FindPathResult result;
        protected override void Awake()
        {
            base.Awake();
            pathFinding = PathFinding.Instance;
            view.OnOkClicked += OnOkClicked;
            view.OnCancelClicked += OnCancelClicked;
            view.OnNextClicked += OnNextClicked;
            view.OnPreviousClicked += OnPreviousClicked;
        }
        public void ShowData(List<FindPathResult> results, int startIndex = 0)
        {
            findPathResults = results;
            if (findPathResults.Count == 0) return;


            currentIndex = startIndex;
            result = findPathResults[currentIndex];

            view.ShowData(result);
        }


        private void OnNextClicked()
        {
            if (findPathResults.Count == 0) return;

            var maxIndex = findPathResults.Count;
            currentIndex++;

            if (currentIndex >= maxIndex)
                currentIndex = 0;

            result = findPathResults[currentIndex];

            if (result == null) return;

            var ok = pathFinding.FindPathWithPossition(result.goal);
            if (ok.ok)
            {
                result.distance = ok.distance;
                result.start = ok.start;
                result.goal = ok.goal;
                result.path = ok.path;

                view.ShowData(result);
            }
        }


        private void OnPreviousClicked()
        {
            if (findPathResults.Count == 0) return;

            currentIndex--;

            if (currentIndex <= -1)
                currentIndex = findPathResults.Count - 1;

            result = findPathResults[currentIndex];
            if (result == null) return;

            pathFinding.FindPathWithPossition(result.goal);
            view.ShowData(result);
        }


        private void OnOkClicked()
        {
            pathFinding.StartFollowPath();
            PathVisualizer.Instance.Draw(result.path);
        }
        private void Update()
        {
            if (result == null) return;

            Vector3 to = result.goal;
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
