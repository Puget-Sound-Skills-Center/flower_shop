using UnityEngine;

public class ComputerScreen : MonoBehaviour
{

    [SerializeField] GameObject computerScreen;

    public void ScreenOn()
    {
        computerScreen.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
