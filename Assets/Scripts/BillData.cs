using UnityEngine;

[CreateAssetMenu(fileName = "NewBill", menuName = "Bills/Bill Data")]
public class BillData : ScriptableObject
{
    [Header("Bill Info")]
    public string billName;
    public int baseAmount;

    [Header("Cycle Settings")]
    public int actionsPerCycle = 10;

    [Header("Recurring")]
    public int recurringIncrease = 0;

    [HideInInspector] public int currentAmount;
    [HideInInspector] public int actionsRemaining;
    [HideInInspector] public bool isPaid;
    [HideInInspector] public bool warningShown;

    public void Initialize()
    {
        isPaid = false;
        currentAmount = baseAmount;
        actionsRemaining = actionsPerCycle;
        warningShown = false;
    }

    public void StartNewCycle()
    {
        isPaid = false;
        currentAmount += recurringIncrease;
        actionsRemaining = actionsPerCycle;
        warningShown = false;
    }

    public void ConsumeActions(int count)
    {
        if (isPaid)
            return;

        actionsRemaining = Mathf.Max(0, actionsRemaining - count);
    }

    public bool IsDue() => actionsRemaining <= 0 && !isPaid;
    public bool IsDueSoon() => actionsRemaining <= 3 && actionsRemaining > 0 && !isPaid;
}
