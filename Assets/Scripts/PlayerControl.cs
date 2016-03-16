using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    Rigidbody2D _rb2d;
    Animator _animator;
    // the position of first spawn
    Vector3 _first_position;
    [SerializeField]
    float _player_lateral_speed; // speed for lateral mouvement
    [SerializeField]
    float _player_speed; // speed for normal mouvement
    [SerializeField]
    bool _isFacing_Right;
    [SerializeField]
    bool _attacking;  // bool to stop the mouvement in the attacking animation

    void Start()
    {
        _first_position = GameObject.FindGameObjectWithTag("FirstPosition").GetComponent<Transform>().position;
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        transform.position = _first_position;
        _player_speed = 1.5f;
        // this is used to insure having the same speed in all the directions
        _player_lateral_speed = _player_speed / Mathf.Sqrt(2);
        _isFacing_Right = true;
        _attacking = true;
    }

    void Update()
    {
        // stops mouvement if the attack animation is on
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            _attacking = false;
        else
            _attacking = true;

        Vector2 mouvement_vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //checks if there is mouvement or no attack animation is on, then move
        if (mouvement_vector != Vector2.zero && _attacking)
        {
            _player_lateral_speed = _player_speed / Mathf.Sqrt(2);
            if ((mouvement_vector.x > 0 && !_isFacing_Right) || (mouvement_vector.x < 0 && _isFacing_Right))
                Flip();
            _animator.SetBool("walking", true);
            Move(mouvement_vector);
        }
        else
            _animator.SetBool("walking", false);

        //either with one press or continuous press trigger the attack
        if (Input.GetButton("Attack"))
            _animator.SetTrigger("attacking");

        if (Input.GetButtonUp("Attack"))
            _animator.ResetTrigger("attacking");
    }

    // uses the same animations, just flip the sprite
    void Flip()
    {
        _isFacing_Right = !_isFacing_Right;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

    }

    //to adjust the speed to the type of mouvement (lateral or horizontal/vertical)
    void Move(Vector2 mouvement_vector)
    {
        if (mouvement_vector.x * mouvement_vector.y == 0)
            _rb2d.MovePosition(_rb2d.position + mouvement_vector * Time.deltaTime * _player_speed);
        else
            _rb2d.MovePosition(_rb2d.position + mouvement_vector * Time.deltaTime * _player_lateral_speed);
    }

}
