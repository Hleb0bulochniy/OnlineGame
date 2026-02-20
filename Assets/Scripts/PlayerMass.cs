using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMass : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _mass = 10f;
    [SerializeField] private float _massLossSpeed = 50f;

    [Header("Coroutine")]
    [SerializeField] private Coroutine _massLossCoroutine;

    [Header("Action")]
    [SerializeField] public UnityAction OnMassChangeAction;

    private bool _isDeadOrLeaving;

    void Start()
    {
        _massLossCoroutine = StartCoroutine(MassLossCoroutine());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (_isDeadOrLeaving) return;

        var triggerPlayer = collision.gameObject;

        if (triggerPlayer.CompareTag("Food"))
        {
            _mass++;
            triggerPlayer.GetComponent<Food>().OnEatFoodAction?.Invoke();
            Resize();
            return;
        }

        if (!triggerPlayer.CompareTag("Player")) return;

        var triggerPlayerMass = triggerPlayer.GetComponent<PlayerMass>();
        if (triggerPlayerMass == null || triggerPlayerMass._isDeadOrLeaving) return;

        //if (_mass > triggerPlayerMass._mass)
        //{
        //    _mass += triggerPlayerMass._mass;
        //    triggerPlayerMass.LeaveGame();
        //    Resize();
        //}

        if (triggerPlayerMass._mass * 1.1 < _mass)
        {
            _mass += triggerPlayerMass._mass;
            Resize();
        }
    }

    private void LeaveGame()
    {
        if (_isDeadOrLeaving) return;
        _isDeadOrLeaving = true;

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        var gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        gameManager?.Leave();
    }

    private void Resize()
    {
        OnMassChangeAction.Invoke();
        float newSize = _mass / 10;
        gameObject.transform.localScale = new Vector3 (newSize, newSize, newSize);
    }

    private IEnumerator MassLossCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_massLossSpeed / _mass);
            if (_mass > 35)
            {
                _mass--;
                Resize();
            }
        }
    }
}
