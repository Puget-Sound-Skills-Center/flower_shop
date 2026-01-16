using System;
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
    public BillData selectedBill;
    public TMP_Text rentDueText;

    public BillData billData;
    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

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

        // 🔔 NEW: Show bill status immediately on game start
        RefreshBillsOnGameStart();
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
                NotifyPlayerBillDue(bill);

            if (bill.actionsRemaining <= 0)
                HandleOverdueBill(bill);
        }

        // 🔁 Refresh UI if selected bill changed
        UpdateRentDueText();
    }

    // Added overload to handle single-bill notifications.
    private void NotifyPlayerBillDue(BillData bill)
    {
        if (bill == null)
            return;

        // Basic notification behavior: log and refresh UI if the notified bill is selected.
        Debug.LogWarning($"{bill.billName} due in {Math.Max(0, bill.actionsRemaining)} actions. Amount: ${bill.currentAmount}");

        if (selectedBill == bill)
            UpdateRentDueText();

        // Extend this method to show UI notifications, sounds, tooltips, etc.
    }

    private void RefreshBillsOnGameStart()
    {
        foreach (var bill in bills)
        {
            if (bill.isPaid)
                continue;

            // Warn player immediately if bill is already close
            if (bill.actionsRemaining <= 3)
            {
                NotifyPlayerBillDue(bill);
            }
        }

        // Update UI text if a bill is selected
        UpdateRentDueText();
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
        audioManager.PlaySFX(audioManager.sellBouquet);

        Debug.Log($"Paid {bill.billName}");

        UpdateRentDueText();
        return true;
    }


    public void SelectBill(BillData bill)
    {
        selectedBill = bill;
        Debug.Log($"Selected bill: {bill.billName}");
        UpdateRentDueText();
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

    private void UpdateRentDueText()
    {
        if (rentDueText == null)
            return;

        if (selectedBill == null)
        {
            rentDueText.text = "Select a bill to view details";
            return;
        }

        if (selectedBill.isPaid)
        {
            rentDueText.text = $"{selectedBill.billName}: PAID";
            return;
        }

        rentDueText.text =
            $"{selectedBill.billName} due in {selectedBill.actionsRemaining} actions\n" +
            $"Amount: ${selectedBill.currentAmount}";
    }
}
