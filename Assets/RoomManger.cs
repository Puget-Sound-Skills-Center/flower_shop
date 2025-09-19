using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Rooms")]
    public GameObject[] rooms;   // Assign all room GameObjects in Inspector
    private int currentRoomIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple RoomManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Optional: Validate rooms array
        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RoomManager: No rooms assigned in the Inspector.");
        }
    }

    public void SwitchRoom(int roomIndex)
    {
        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RoomManager: No rooms to switch.");
            return;
        }

        if (roomIndex < 0 || roomIndex >= rooms.Length)
        {
            Debug.LogWarning($"RoomManager: Invalid room index {roomIndex}.");
            return;
        }

        // Disable all rooms
        foreach (GameObject room in rooms)
        {
            if (room != null)
                room.SetActive(false);
            else
                Debug.LogWarning("RoomManager: Room GameObject is null.");
        }

        // Enable the target room
        if (rooms[roomIndex] != null)
        {
            rooms[roomIndex].SetActive(true);
            currentRoomIndex = roomIndex;
        }
        else
        {
            Debug.LogError($"RoomManager: Target room at index {roomIndex} is null.");
        }
    }

    public void NextRoom()
    {
        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RoomManager: No rooms to switch.");
            return;
        }

        int nextIndex = (currentRoomIndex + 1) % rooms.Length;
        SwitchRoom(nextIndex);
    }
}
