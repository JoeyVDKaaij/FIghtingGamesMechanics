using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackScript : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField, Tooltip("Set the attack button.")]
    private KeyCode attackButton = KeyCode.F;

    [SerializeField, Tooltip("Set the attack button.")]
    private KeyCode blockButton = KeyCode.E;
    [SerializeField, Tooltip("Set the scriptable object that this script should look at.")]
    private PlayerObject playerObject;
    
    private Animator _animator;
    private float _timer;
    private float _aiTimeLimit;
    private int _attacking;
    private bool _moving;

    private Coroutine _currentCoroutine;
    private bool _rightState = true;
    
    private MovementControllerScript _movementScript;
    
     private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movementScript = GetComponent<MovementControllerScript>();
    }

    void Update()
    {
        if (playerObject != null)
        {
            if (playerObject.playerState != PlayerState.Stunned && !playerObject.ai) InputAttacks();
            else if (playerObject.playerState != PlayerState.Stunned) AIControls();
        }
    }

    private void InputAttacks()
    {
        // Ready's up next attack
        if (playerObject.playerState == PlayerState.Attacked && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            playerObject.playerState = PlayerState.Alive;
            GameManager.instance.InvokeOnNewAttackEvent();
        }
        
        // Light attack
        if (Input.GetKeyDown(attackButton) && _movementScript.CheckGrounded())
        {
            _animator.SetTrigger("LightAttack");
            playerObject.playerState = PlayerState.Attacked;
        }

        // Blocking
        _animator.SetBool("Blocking", Input.GetKey(blockButton));
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Block") && _movementScript.CheckGrounded()) playerObject.playerState = PlayerState.Blocking;
        else if (playerObject.playerState == PlayerState.Blocking) playerObject.playerState = PlayerState.Alive;
    }

    private void AIControls()
    {
        if (playerObject.playerState == PlayerState.Attacked)
        {
            if (_rightState)
                AIAttacks();
            else if (_attacking == -1)
            {
                _timer += Time.deltaTime;
                if (_timer >= _aiTimeLimit) playerObject.playerState = PlayerState.Alive;
            }
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                _rightState = true;
        }
        else if (_moving)
        {
            _timer += Time.deltaTime;
            if (_timer >= _aiTimeLimit)
                _moving = false;
        }
        else if (playerObject.playerState == PlayerState.Blocking)
        {
            _timer += Time.deltaTime;
            _animator.SetBool("Blocking", true);
            if (_timer >= _aiTimeLimit)
            {
                playerObject.playerState = PlayerState.Alive;
                _animator.SetBool("Blocking", false);
            }
        }
        else
        {
            AIChooses();
        }
    }

    private void AIChooses()
    {
        // Chooses their next action
        int action = Random.Range(0, 100);
        
        if (action < 50)
        {
            // The AI uses their attack
            playerObject.playerState = PlayerState.Attacked;
            _attacking = Random.Range(0, 3);
            _aiTimeLimit = 1;
        }
        else if (action < 75)
        {
            // The AI moves
            _moving = true;
            _aiTimeLimit = 5;
        }
        else
        {
            // The AI blocks
            playerObject.playerState = PlayerState.Blocking;
            _aiTimeLimit = Random.Range(1, 3);
        }
        _timer = 0;
    }

    private void AIAttacks()
    {
        _animator.SetTrigger("LightAttack");
        // Light attack
        switch (_attacking)
        {
            case 0:
                _attacking = -1;
                _rightState = false;
                break;
            case 1:
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttackFollowUp"))
                {
                    _attacking = -1;
                    _rightState = false;
                }
                break;
            case 2:
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttackEnding"))
                {
                    _attacking = -1;
                    _rightState = false;
                }
                break;
        }
    }
    
    /* Stun the player for a set amount of time.
        This uses the coroutine to make sure the stun would not get in the way of any other functions
     * pFrames shows how many frames the player should be stunned for.
     */
    public void StunPlayer(int pFrames)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        
        _currentCoroutine = StartCoroutine(StunCoroutine(pFrames));
    }
    
    /* Stun the player for a set amount of time.
    * pFrames shows how many frames the player should be stunned for.
    */
    IEnumerator StunCoroutine(int pFrames)
    {
        int i = 0;
        while (i < pFrames)
        {
            yield return null;
            i++;
        }
        Debug.Log("Stun stop");
        
        playerObject.playerState = PlayerState.Alive;
        _currentCoroutine = null;
    }
}
