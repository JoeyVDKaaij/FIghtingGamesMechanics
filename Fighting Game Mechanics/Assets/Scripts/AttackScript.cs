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
    [SerializeField, Tooltip("Set if the ai should be in control.")]
    private bool ai = false;
    
    private Animator _animator;
    private bool _stunned = false;
    private bool _attacked = false;
    AnimatorStateInfo _currentStateInfo;
    private bool _blocking = false;
    private float _timer;
    private float _aiTimeLimit;
    private int _attacking;
    private bool _moving;

    private Coroutine _currentCoroutine;
    private bool _rightState = true;
    
     private void Awake()
    {
        _animator = GetComponent<Animator>();
        _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    }

    void Update()
    {
        if (!_stunned && !ai) InputAttacks();
        else if (!_stunned) AIControls();
    }

    private void InputAttacks()
    {
        // Ready's up next attack
        if (_attacked && !_currentStateInfo.Equals(_animator.GetCurrentAnimatorStateInfo(0)))
        {
            _attacked = false;
            GameManager.instance.InvokeOnNewAttackEvent();
            _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        }
        
        // Light attack
        if (Input.GetKeyDown(attackButton))
        {
            _animator.SetTrigger("LightAttack");
            _attacked = true;
        }

        // Blocking
        _animator.SetBool("Blocking", Input.GetKey(blockButton));
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Block")) _blocking = true;
        else _blocking = false;
    }

    private void AIControls()
    {
        if (_attacked)
        {
            if (_rightState)
                AIAttacks();
            else if (_attacking == -1)
            {
                _timer += Time.deltaTime;
                if (_timer >= _aiTimeLimit) _attacked = false;
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
        else if (_blocking)
        {
            _timer += Time.deltaTime;
            _animator.SetBool("Blocking", _blocking);
            if (_timer >= _aiTimeLimit)
            {
                _blocking = false;
                _animator.SetBool("Blocking", _blocking);
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
        
        if (action < 33)
        {
            // The AI uses their attack
            _attacked = true;
            _attacking = Random.Range(0, 3);
            _aiTimeLimit = 1;
        }
        else if (action < 66)
        {
            // The AI moves
            _moving = true;
            _aiTimeLimit = 5;
        }
        else
        {
            // The AI blocks
            _blocking = true;
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
        _stunned = true;

        int i = 0;
        while (i < pFrames)
        {
            yield return null;
            i++;
        }
        Debug.Log("Stun stop");
        _stunned = false;
        _currentCoroutine = null;
    }

    public bool Stunned
    {
        get { return _stunned; }
    }

    public bool Blocking
    {
        get { return _blocking; }
    }

    public bool Moving
    {
        get { return _moving; }
    }

    public bool AI
    {
        get { return ai; }
    }
}
