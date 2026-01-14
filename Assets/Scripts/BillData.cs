using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Bills/Bill")]
public class BillData : ScriptableObject
{
    [Header("Bill Info")]
    public string billName;
    public int baseAmount;

    [Header("Recurring Rules")]
    public int recurringIncrease;          // how much it increases each cycle

    [Header("Action-Based Deadline")]
    public int actionsUntilDue;             // e.g. 10 sales
    [HideInInspector] public int actionsRemaining;

    [HideInInspector] public int currentAmount;
    [HideInInspector] public bool isPaid;

    public void StartNewCycle()
    {
        isPaid = false;
        actionsRemaining = actionsUntilDue;
        currentAmount += recurringIncrease;
    }

    public void Initialize()
    {
        currentAmount = baseAmount;
        actionsRemaining = actionsUntilDue;
        isPaid = false;
    }

}
