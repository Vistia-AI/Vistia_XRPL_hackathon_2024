using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Analysis
{
    public class SwitchController : MonoBehaviour
    {
        [SerializeField] private Button TOSbtn;
        [SerializeField] private Button TOBbtn;
        [SerializeField] private TMP_Text TOStxt;
        [SerializeField] private TMP_Text TOBtxt;
        [SerializeField] private TableCreate tableCreate;
        private TMP_Text currentChooseTxt;
        private Button currentChooseBtn;
        private Color32 unselectColor = new Color32(50, 50, 50, 150);
        private List<StockData> TOBList = new List<StockData>();
        private List<StockData> TOSList = new List<StockData>();
        private bool isOnTOB = true;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            TOSbtn.onClick.AddListener(TOSSelect);
            TOBbtn.onClick.AddListener(TOBSelect);
            AlchemistAPIHandle.Instance.TopOverBoughtUpdatedStream += OnTopOverBoughtUpdated;
            AlchemistAPIHandle.Instance.TopOverSoldUpdatedStream += OnTopOverSoldUpdated;
        }

        private void OnTopOverBoughtUpdated(List<StockData> list)
        {
            this.TOBList = list;
            if (isOnTOB)
            {
                TOBSelect();
            }
        }
        private void OnTopOverSoldUpdated(List<StockData> list)
        {
            this.TOSList = list;
            if (!isOnTOB)
            {
                TOSSelect();
            }
        }
        private void TOBSelect()
        {
            isOnTOB = true;
            ChangeColorBtn(TOBtxt);
            if (currentChooseBtn != null)
            {
                currentChooseBtn.interactable = true;
            }
            TOBbtn.interactable = false;
            currentChooseBtn = TOBbtn;
            tableCreate.CreateTable(TOBList, Signal.Overbought);
        }

        private void TOSSelect()
        {
            isOnTOB = false;
            ChangeColorBtn(TOStxt);
            if (currentChooseBtn != null)
            {
                currentChooseBtn.interactable = true;
            }
            TOSbtn.interactable = false;
            currentChooseBtn = TOSbtn;

            tableCreate.CreateTable(TOSList, Signal.Oversold);
        }

        private void ChangeColorBtn(TMP_Text select)
        {
            if (currentChooseTxt != null)
            {
                currentChooseTxt.color = unselectColor;
            }
            currentChooseTxt = select;
            currentChooseTxt.color = Color.black;

        }

        private void OnDestroy()
        {
            AlchemistAPIHandle.Instance.TopOverBoughtUpdatedStream -= OnTopOverBoughtUpdated;
            AlchemistAPIHandle.Instance.TopOverSoldUpdatedStream -= OnTopOverSoldUpdated;

        }
    }
}
