using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class MineInfoPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button mineBtn;
        [SerializeField] private Image mineIcon;
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI productionTxt;
        [SerializeField] private TextMeshProUGUI TimeToHavest;
        public event Action OnMineClicked;
        protected override void Awake()
        {
            base.Awake();
            mineBtn.onClick.AddListener(() =>
            {
                OnMineClicked?.Invoke();
            });
        }
        public void Show(ItemData itemData)
        {
            var itemResourseData = itemData as ItemResourseData;
            mineIcon.sprite = itemResourseData.itemIcon;
            nameTxt.text = itemResourseData.itemName;
            productionTxt.text = itemResourseData.currentAmount.ToString() + "/" + itemResourseData.maxStorage.ToString();
            TimeToHavest.text = itemResourseData.currentMiningProgress.ToString();
        }
        public void UpdateProduction(ItemData itemData)
        {
            var itemResourseData = itemData as ItemResourseData;
            int currentSecond = Mathf.FloorToInt(itemResourseData.currentMiningProgress);
            productionTxt.text = itemResourseData.currentAmount.ToString() + "/" + itemResourseData.maxStorage.ToString();
            TimeToHavest.text = currentSecond.ToString() + "s";
        }
    }
}