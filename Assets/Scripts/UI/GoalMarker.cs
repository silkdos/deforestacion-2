using UnityEngine;

public class GoalMarker : MonoBehaviour
{
    public Transform goal;
    public Camera playerCamera;
    public RectTransform marker;

    public float edgePadding = 40f;

    void Update()
    {
        Vector3 viewportPos = playerCamera.WorldToViewportPoint(goal.position);

        bool isBehind = viewportPos.z < 0;

        if (isBehind)
        {
            viewportPos.x = 1 - viewportPos.x;
            viewportPos.y = 1 - viewportPos.y;
        }

        Vector3 screenPos = new Vector3(
            viewportPos.x * Screen.width,
            viewportPos.y * Screen.height,
            0
        );

        screenPos.x = Mathf.Clamp(screenPos.x, edgePadding, Screen.width - edgePadding);
        screenPos.y = Mathf.Clamp(screenPos.y, edgePadding, Screen.height - edgePadding);

        marker.position = screenPos;
    }
}