using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BillManager : MonoBehaviour
{
    public static BillManager Instance;

    [Header("Bills")]
    public List<BillData> bills;

    [Header("UI")]
    public Button payBillButton;
    public TMP_Text rentDueText;
    public GameObject NotifyTab;

    [Header("Notify Settings")]
    public int notifyThreshold = 3;
    public float slideDuration = 0.35f;
    public float notifyHoldTime = 2.5f;
    public float hiddenX = 300f;
    public float visibleX = -20f;

    [Header("Colors")]
    public Color paidColor = Color.green;
    public Color warningColor = new Color(1f, 0.65f, 0f);
    public Color urgentColor = Color.red;
    public Color normalColor = Color.white;
    public Color overdueColor = new Color(0.5f, 0f, 0f); // dark red

    [Header("Selection")]
    public BillData selectedBill;

    private RectTransform notifyRect;
    private CanvasGroup notifyGroup;
    private Coroutine notifyRoutine;

    private BillData _billPendingNextCycle;

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

        // NotifyTab setup
        if (NotifyTab != null)
        {
            notifyRect = NotifyTab.GetComponent<RectTransform>();
            notifyGroup = NotifyTab.GetComponent<CanvasGroup>();

            if (notifyGroup == null)
                notifyGroup = NotifyTab.AddComponent<CanvasGroup>();

            notifyRect.anchoredPosition =
                new Vector2(hiddenX, notifyRect.anchoredPosition.y);

            notifyGroup.alpha = 0f;
            NotifyTab.SetActive(false);
        }
    }

    private void Start()
    {
        if (payBillButton != null)
        {
            payBillButton.onClick.RemoveAllListeners();
            payBillButton.onClick.AddListener(PaySelectedBill);
        }

        UpdateRentDueText();
    }

    /// <summary>
    /// Call from meaningful player actions (buy seed, harvest, sell, etc.)
    /// </summary>
    public void RecordPlayerAction(int count = 1)
    {
        foreach (var bill in bills)
        {
            if (bill.isPaid)
                continue;

            bill.ConsumeActions(count);

            if (bill.IsDue())
            {
                HandleOverdueBill(bill);
                continue;
            }

            if (bill.actionsRemaining <= notifyThreshold && !bill.warningShown)
            {
                bill.warningShown = true;
                ShowNotifyTab(bill);
            }
        }

        UpdateRentDueText();
    }

    // ─────────────────────────────
    // 🔔 Notify Tab Animation
    // ─────────────────────────────

    private void ShowNotifyTab(BillData bill)
    {
        if (NotifyTab == null)
            return;

        if (notifyRoutine != null)
            StopCoroutine(notifyRoutine);

        notifyRoutine = StartCoroutine(NotifyTabRoutine(bill));
    }

    private IEnumerator NotifyTabRoutine(BillData bill)
    {
        NotifyTab.SetActive(true);

        // Update text inside tab
        TMP_Text text = NotifyTab.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = $"{bill.billName} due in {bill.actionsRemaining} actions!";

        // Slide in
        yield return SlideNotify(hiddenX, visibleX, 0f, 1f);

        // Hold
        yield return new WaitForSeconds(notifyHoldTime);

        // Slide out
        yield return SlideNotify(visibleX, hiddenX, 1f, 0f);

        NotifyTab.SetActive(false);
        notifyRoutine = null;
    }

    private IEnumerator SlideNotify(
        float fromX,
        float toX,
        float fromAlpha,
        float toAlpha
    )
    {
        float t = 0f;

        Vector2 startPos = notifyRect.anchoredPosition;
        Vector2 endPos = new Vector2(toX, startPos.y);

        notifyRect.anchoredPosition = new Vector2(fromX, startPos.y);
        notifyGroup.alpha = fromAlpha;

        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / slideDuration);
            float eased = Mathf.SmoothStep(0f, 1f, p);

            notifyRect.anchoredPosition =
                Vector2.Lerp(
                    new Vector2(fromX, startPos.y),
                    endPos,
                    eased
                );

            notifyGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);
            yield return null;
        }

        notifyRect.anchoredPosition = endPos;
        notifyGroup.alpha = toAlpha;
    }

    // ─────────────────────────────
    // 💸 Billing Logic
    // ─────────────────────────────

    public void SelectBill(BillData bill)
    {
        selectedBill = bill;
        UpdateRentDueText();
    }

    private void PaySelectedBill()
    {
        if (selectedBill == null || selectedBill.isPaid)
            return;

        if (selectedBill.actionsRemaining > 0)
        {
            rentDueText.text =
                $"{selectedBill.billName}: NOT DUE YET\n" +
                $"Due in {selectedBill.actionsRemaining} actions";

            rentDueText.color = warningColor;
            return;
        }

        PayBill(selectedBill);
    }

    private void PayBill(BillData bill)
    {
        var gm = GameManager.Instance;
        if (gm == null || !gm.SpendMoney(bill.currentAmount))
            return;

        bill.isPaid = true;
        UpdateRentDueText();

        _billPendingNextCycle = bill;
        Invoke(nameof(DelayedNextCycle), 1.5f);
    }

    private void DelayedNextCycle()
    {
        if (_billPendingNextCycle != null)
        {
            _billPendingNextCycle.StartNewCycle();
            _billPendingNextCycle = null;
            UpdateRentDueText();
        }
    }

    private void HandleOverdueBill(BillData bill)
    {
        Debug.LogError($"BILL OVERDUE: {bill.billName}");
        Application.Quit(); // TEMP
    }

    private void UpdateRentDueText()
    {
        if (rentDueText == null)
            return;

        if (selectedBill == null)
        {
            rentDueText.text = "Select a bill";
            rentDueText.color = normalColor;
            payBillButton.interactable = false;
            return;
        }

        if (selectedBill.isPaid)
        {
            rentDueText.text =
                $"{selectedBill.billName}: PAID\nNext cycle starting...";
            rentDueText.color = paidColor;
            payBillButton.interactable = false;
            return;
        }

        int actionsLeft = selectedBill.actionsRemaining;

        if (actionsLeft > 3)
        {
            rentDueText.text =
                $"{selectedBill.billName}: NOT DUE\n" +
                $"Due in {actionsLeft} actions\n" +
                $"Amount: ${selectedBill.currentAmount}";
            rentDueText.color = normalColor;
            payBillButton.interactable = false;
        }
        else if (actionsLeft > 0)
        {
            rentDueText.text =
                $"{selectedBill.billName}: DUE SOON\n" +
                $"Due in {actionsLeft} actions\n" +
                $"Amount: ${selectedBill.currentAmount}";
            rentDueText.color = warningColor;
            payBillButton.interactable = false;
        }
        else if (actionsLeft <= 0)
        {
            rentDueText.text =
                $"{selectedBill.billName}: DUE NOW\n" +
                $"Amount: ${selectedBill.currentAmount}";
            rentDueText.color = urgentColor;
            payBillButton.interactable = true;
        }
        else // actionsLeft < 0
        {
            rentDueText.text =
                $"{selectedBill.billName}: OVERDUE\n" +
                $"Amount: ${selectedBill.currentAmount}";
            rentDueText.color = overdueColor;
            payBillButton.interactable = true;
            Debug.LogError($"BILL PAST DUE: {selectedBill.billName}");
        }
    }
}
