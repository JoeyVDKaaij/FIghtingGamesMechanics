using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    public static event Action onNewAttack;
    public static event Action<bool, GameObject, int> onTakingDamage;
    
    [HideInInspector]
    public GameObject[] _players = new GameObject[2];
    public PlayerObject[] _playerObjects = null;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.parent != null ? transform.parent.gameObject : gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;

        _players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Update()
    {
        _PlayersRotationCheck();
    }

    /* Check the rotation and adjust the rotation if needed. */
    private void _PlayersRotationCheck()
    {
        if (_players[0] != null && _players[1] != null)
        {
            if (_players[0].transform.position.x > _players[1].transform.position.x)
            {
                _players[0].transform.rotation = Quaternion.Euler(0, 180, 0);
                _players[1].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (_players[0].transform.position.x < _players[1].transform.position.x)
            {
                _players[0].transform.rotation = Quaternion.Euler(0, 0, 0);
                _players[1].transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
    
    public void AddPlayer(GameObject pPlayer)
    {
        if (_players[0] != null)
            _players[1] = pPlayer;
        else if (_players[0] != null)
            _players[0] = pPlayer;
    }

    /* Get the other players game object. */
    public GameObject GetOtherPlayerGameObject(GameObject pPlayer)
    {
        if (_players[0] != pPlayer)
            return _players[0];
        
        return _players[1];
    }
    
    public PlayerObject GetOtherPlayerDetails(GameObject pPlayer)
    {
        if (_players[0] != pPlayer)
            return _playerObjects[0];
        
        return _playerObjects[1];
    }
    
    public void InvokeOnNewAttackEvent()
    {
        onNewAttack?.Invoke();
    }

    public void InvokeOnTakingDamageEvent(bool pAttack, GameObject pPlayer, int pDamage)
    {
        onTakingDamage?.Invoke(pAttack, pPlayer, pDamage);
    }
}
