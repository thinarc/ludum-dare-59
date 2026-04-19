using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        // [SerializeField] private float speed = 2f;
        //
        // private Rigidbody2D _rb;
        //
        // private float _horizontal;
        // private bool _facingRight;
        //
        // private void Start()
        // {
        //     _rb = GetComponent<Rigidbody2D>();
        // }
        //
        // private void Update()
        // {
        //     _horizontal = Input.GetAxis("Horizontal");
        //     if (!_facingRight && _horizontal > 0 || _facingRight && _horizontal < 0) Flip();
        // }
        //
        // private void Flip()
        // {
        //     _facingRight = !_facingRight;
        //     var scale = transform.localScale;
        //     scale.x *= -1;
        //     transform.localScale = scale;
        // }
        //
        // private void FixedUpdate() => _rb.linearVelocity = new Vector2(_horizontal * speed, _rb.linearVelocity.y);
    }
}