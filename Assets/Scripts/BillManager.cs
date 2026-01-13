using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BillManager : MonoBehaviour
{
    public static BillManager Instance;
    public List<BillData> bills;

    [Header("PayButton")]
    public Button PayBillButton;

    [Header("bills")]
    public BillData selectedBill;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var bill in bills)
            bill.Initialize();
    }

    private void Start()
    {
        Debug.Log("BillManager START");

        if (PayBillButton != null)
        {
            PayBillButton.onClick.RemoveAllListeners();
            PayBillButton.onClick.AddListener(PaySelectedBill);
            Debug.Log("PayBillButton listener added");
        }
        else
        {
            Debug.LogError("PayBillButton is NULL");
        }

    }


    // 🔔 Call this when a meaningful action happens
    public void NotifyPlayerBillDue()
    {
        foreach (var bill in bills)
        {
            if (bill.isPaid)
                continue;

            bill.actionsRemaining--;
            if (bill.actionsRemaining <= 3)
            {
                NotifyPlayerBillDue(bill);
            }
            if (bill.actionsRemaining <= 0)
            {
                HandleOverdueBill(bill);
            }
        }
    }

    public bool PayBill(BillData bill)
    {
        var gm = GameManager.Instance;

        if (gm == null)
            return false;

        if (!gm.SpendMoney(bill.currentAmount))
            return false;

        bill.isPaid = true;
        bill.StartNewCycle();

        Debug.Log($"Paid {bill.billName}");

        return true;
    }

    public void SelectBill(BillData bill)
    {
        selectedBill = bill;
        Debug.Log($"Selected bill: {bill.billName}");
    }

    public void PaySelectedBill()
    {
        Debug.Log("PaySelectedBill called");

        if (selectedBill == null)
        {
            Debug.LogError("selectedBill is NULL");
            return;
        }

        PayBill(selectedBill);
    }


    private void HandleOverdueBill(BillData bill)
    {
        Debug.LogError($"BILL OVERDUE: {bill.billName}");

        // ⚠️ TEMP TEST PENALTY
        Application.Quit();
    }

    public void NotifyPlayerBillDue(BillData bill)
    {
        Debug.LogError($"BILL OVERDUE SOON: {bill.billName}");

    }
}
