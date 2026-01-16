using System;
using System.Collections;
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
    private const string NOT_DUE_TEXT = "NOT DUE YET";
    private const string DUE_NOW_TEXT = "DUE NOW";


    [Header("Bill UI Colors")]
    public Color paidColor = Color.green;
    public Color warningColor = new Color(1f, 0.65f, 0f); // orange/yellow
    public Color urgentColor = Color.red;
    public Color normalColor = Color.white;


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

        audioManager.PlaySFX(audioManager.sellBouquet);

        Debug.Log($"Paid {bill.billName}");

        // 🔁 Update UI immediately
        UpdateRentDueText();

        // ⏳ Start next cycle AFTER UI feedback (or later via game logic)
        StartCoroutine(StartNextCycleDelayed(bill, 1.5f));

        return true;
    }

    private IEnumerator StartNextCycleDelayed(BillData bill, float delay)
    {
        yield return new WaitForSeconds(delay);

        bill.StartNewCycle();

        // If still selected, refresh UI again
        if (selectedBill == bill)
            UpdateRentDueText();
    }

    public void SelectBill(BillData bill)
    {
        selectedBill = bill;
        Debug.Log($"Selected bill: {bill.billName}");
        UpdateRentDueText();
    }


    public void PaySelectedBill()
    {
        if (selectedBill == null)
            return;

        if (selectedBill.isPaid)
            return;

        if (selectedBill.actionsRemaining > 0)
        {
            rentDueText.text =
                $"{selectedBill.billName}: NOT DUE YET\n" +
                $"Come back in {selectedBill.actionsRemaining} actions";

            rentDueText.color = warningColor;
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

    private void UpdatePayButtonState()
    {
        if (PayBillButton == null)
            return;

        if (selectedBill == null)
        {
            PayBillButton.interactable = false;
            return;
        }

        // ❌ Cannot pay if already paid
        if (selectedBill.isPaid)
        {
            PayBillButton.interactable = false;
            return;
        }

        // ❌ Cannot pay if not yet due
        if (selectedBill.actionsRemaining > 0)
        {
            PayBillButton.interactable = false;
            return;
        }

        // ✅ Bill is due or overdue
        PayBillButton.interactable = true;
    }


    private void UpdateRentDueText()
    {
        if (rentDueText == null)
            return;

        if (selectedBill == null)
        {
            rentDueText.text = "Select a bill to view details";
            rentDueText.color = normalColor;
            UpdatePayButtonState();
            return;
        }

        // ✅ PAID
        if (selectedBill.isPaid)
        {
            rentDueText.text =
                $"{selectedBill.billName}: PAID\nNext cycle started";

            rentDueText.color = paidColor;
            UpdatePayButtonState();
            return;
        }

        int actionsLeft = selectedBill.actionsRemaining;

        // ❌ NOT DUE YET
        if (actionsLeft > 3)
        {
            rentDueText.text =
                $"{selectedBill.billName}: {NOT_DUE_TEXT}\n" +
                $"Due in {actionsLeft} actions\n" +
                $"Amount: ${selectedBill.currentAmount}";

            rentDueText.color = normalColor;
            UpdatePayButtonState();
            return;
        }

        // ⚠️ WARNING (approaching due)
        if (actionsLeft > 0)
        {
            rentDueText.text =
                $"{selectedBill.billName} due soon\n" +
                $"Due in {actionsLeft} actions\n" +
                $"Amount: ${selectedBill.currentAmount}";

            rentDueText.color = warningColor;
            UpdatePayButtonState();
            return;
        }

        // 🚨 DUE NOW
        rentDueText.text =
            $"{selectedBill.billName}: {DUE_NOW_TEXT}\n" +
            $"Amount: ${selectedBill.currentAmount}";

        rentDueText.color = urgentColor;
        UpdatePayButtonState();
    }
}
