using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementControllerScript : MonoBehaviour
{
    [SerializeField, Tooltip("Set the player movement speed.")]
    private float speed = 1;
    [SerializeField, Tooltip("Set the player jump force.")]
    private float jumpForce = 10;
    [SerializeField, Tooltip("Set the player left movement button.")]
    private KeyCode moveLeft = KeyCode.A;
    [SerializeField, Tooltip("Set the player right movement button.")]
    private KeyCode moveRight = KeyCode.D;
    [SerializeField, Tooltip("Set the player jump button.")]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField, Tooltip("Set if the player moves at a set speed instead of the input speed.")]
    private bool oneContinuesSpeed = false;
    [SerializeField, Tooltip("Set the ground drag.")]
    private float groundDrag;
    [SerializeField, Tooltip("Set the playerHeight.")]
    private float PlayerHeight;
    [SerializeField, Tooltip("Set what the ground is.")]
    private LayerMask whatIsGround;
    
    [SerializeField, Tooltip("Set the cooldown between 2 jumps.")]
    private float jumpCooldown;
    [SerializeField, Tooltip("Set the speed when the player is in the air.")]
    private float airMultiplier;
    [SerializeField, Tooltip("Set the player Scriptable Object to use.")]
    private PlayerObject playerObject;
    
    private float _aiWalkTime;
    
    private bool readyToJump = true;

    private Rigidbody _rb;
    private float _moveDirection;
    private float _horizontalInput;

    private bool _stunned;
    private ComboMeterScript _otherPlayerComboMeterScript;
    private bool _aiAlreadyMoved = false;
    private float _timer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        if (playerObject.playerState != PlayerState.Stunned)
        {
            if (!playerObject.ai)
                _horizontalInput = 0;

            _timer += Time.deltaTime;
            
            // ground check
            if (CheckGrounded())
            {
                if (playerObject != null && GameManager.instance.GetOtherPlayerDetails(gameObject).playerState != PlayerState.Stunned)
                    playerObject.comboActive = false;
            }
            
            if (Physics.Raycast(transform.position, Vector3.down, PlayerHeight * transform.lossyScale.y / 2 + 0.2f,
                    LayerMask.NameToLayer("Player")))
            {
                transform.position += Vector3.left;
            }

            if (!playerObject.ai)
                PlayerInput();
            else
            {
                if (!_aiAlreadyMoved && playerObject.playerState == PlayerState.AIMoving || playerObject.playerState == PlayerState.Alive)
                {
                    _aiAlreadyMoved = true;
                    _horizontalInput = Random.Range(-1, 2);
                    if (_horizontalInput == 0) _horizontalInput = 1;
                    _aiWalkTime = Random.Range(0, 2);
                    _timer = 0;
                }
                else if (playerObject.playerState != PlayerState.AIMoving)
                {
                    _aiAlreadyMoved = false;
                }
            }
            
            SpeedControl();

            // Handle drag
            if (CheckGrounded())
                _rb.drag = groundDrag;
            else
                _rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        if (playerObject.playerState == PlayerState.Alive || playerObject.ai && playerObject.playerState == PlayerState.AIMoving)
            MovePlayer();
    }
    
    /* Players input. Change the actual input in the inspector. */
    private void PlayerInput()
    {
        if (Input.GetKey(moveLeft) || Input.GetKey(moveRight))
        {
            if (oneContinuesSpeed)
                _horizontalInput = Input.GetAxis("Horizontal");
            else
                _horizontalInput = Input.GetKey(moveLeft)?-1:1;
        }
        
        if (Input.GetKey(jumpKey) && CheckGrounded() && readyToJump && playerObject.playerState == PlayerState.Alive)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    
    /* Move the player at a speed set in the inspector. */
    private void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(_horizontalInput, 0, 0);
        
        if (CheckGrounded()) 
            _rb.AddForce(moveDirection * 10 * speed, ForceMode.Force);
        else 
            _rb.AddForce(moveDirection * 10 * speed * airMultiplier, ForceMode.Force);

        if (playerObject.ai)
        {
            if (_timer >= _aiWalkTime)
                _horizontalInput = 0;
        }
    }

    /* Controls the speed so it doesn't go faster than the set speed. */
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, 0f);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, 0);
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    
    /* Applies knock back based on the attack it received. */
    public void ApplyKnockback(float pKnockbackForward, float pKnockbackUp, bool pTowardsTheLeft)
    {
        _rb.velocity = Vector3.zero;
            _rb.AddForce(pTowardsTheLeft?-pKnockbackForward:pKnockbackForward,pKnockbackUp, 0, ForceMode.Impulse);
    }

    public bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down,
            PlayerHeight * transform.lossyScale.y / 2 + 0.2f, whatIsGround);
    }
}
