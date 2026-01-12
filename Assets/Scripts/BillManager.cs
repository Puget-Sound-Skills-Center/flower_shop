using UnityEngine;
using System.Collections.Generic;

public class BillManager : MonoBehaviour
{
    public static BillManager Instance;

    public List<BillData> bills;

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
