using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float      moveSpeed    = 5f;
    public MapGenerator mapGen;       // drag in your MapRoot
    public int        visionRadius = 3;

    Rigidbody2D _rb;
    Vector2     _input;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        RevealAroundPlayer();
    }

    void Update()
    {
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 newPos = _rb.position + _input.normalized * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPos);
        RevealAroundPlayer();
    }

    void RevealAroundPlayer()
    {
        if (mapGen == null || mapGen.tileGrid == null) return;

       // New code using mapGen.origin:
        Vector2 ori = mapGen.origin;  
        Vector3 pos = transform.position;
        int px = Mathf.FloorToInt(pos.x - ori.x);
        int py = Mathf.FloorToInt(pos.y - ori.y);

        for (int dx = -visionRadius; dx <= visionRadius; dx++)
        for (int dy = -visionRadius; dy <= visionRadius; dy++)
        {
            int x = px + dx;
            int y = py + dy;
            if (x >= 0 && x < mapGen.width && y >= 0 && y < mapGen.height)
                mapGen.tileGrid[x, y].Reveal(fadeDuration: 0.3f);
        }

    }
}
