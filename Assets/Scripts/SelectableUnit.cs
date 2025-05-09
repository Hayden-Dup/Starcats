using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public float moveSpeed = 3f;
    private bool isSelected = false;
    private Vector3? targetPosition = null;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (targetPosition.HasValue)
        {
            Vector3 dir = (targetPosition.Value - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPosition.Value) < 0.1f)
                targetPosition = null;
        }

        if (sr != null)
            sr.color = isSelected ? Color.cyan : Color.white;
    }

    public void SetSelected(bool value)
    {
        isSelected = value;
    }

    public void MoveTo(Vector3 destination)
    {
        targetPosition = destination;
    }
}
