using UnityEngine;

public class GlobalTime : MonoBehaviour
{
    public static GlobalTime Instance { get; private set; }

    // Real unscaled time (good for timers that must keep going if game is paused or rooms deactivate)
    public double RealNow => Time.realtimeSinceStartupAsDouble;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
