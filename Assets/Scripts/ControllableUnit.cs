using UnityEngine;

public class ControllableUnit : MonoBehaviour
{
    public UnitData data; // assign in prefab or on spawn
    private bool isSelected = false;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Update()
    {
        if (isMoving)
        {
            float step = data.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                isMoving = false;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        // Optional: Add outline/highlight visuals here
    }

    public void MoveTo(Vector3 destination)
    {
        targetPosition = destination;
        isMoving = true;
    }
}
