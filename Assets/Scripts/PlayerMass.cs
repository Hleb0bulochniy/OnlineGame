using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMass : MonoBehaviourPun, IPunObservable
{
    [Header("Parameters")]
    [SerializeField] private float _mass = 10f;
    [SerializeField] private float _massLossSpeed = 500f;

    [Header("Coroutine")]
    [SerializeField] private Coroutine _massLossCoroutine;

    [Header("Action")]
    [SerializeField] public UnityAction OnMassChangeAction;

    private bool _dead;

    private Tween _scaleTween;
    public float Mass => _mass;

    void Start()
    {
        ResizeLocal();
        _massLossCoroutine = StartCoroutine(MassLossCoroutine());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (_dead) return;

        var trigger = collision.gameObject;

        if (trigger.CompareTag("Food"))
        {
            _mass++;
            trigger.GetComponent<Food>().OnEatFoodAction?.Invoke();
            ResizeLocal();
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        if (!trigger.CompareTag("Player")) return;

        var triggerPlayerMass = trigger.GetComponent<PlayerMass>();
        if (triggerPlayerMass == null || triggerPlayerMass._dead) return;

        if (photonView.ViewID > triggerPlayerMass.photonView.ViewID) return;

        ResolveEat(this, triggerPlayerMass);
    }

    private void ResolveEat(PlayerMass a, PlayerMass b)
    {
        if (a._mass >= b._mass * 1.1f)
        {
            // a ест b
            photonView.RPC(nameof(RPC_Eat), RpcTarget.All, a.photonView.ViewID, b.photonView.ViewID);
        }
        else if (b._mass >= a._mass * 1.1f)
        {
            // b ест a
            photonView.RPC(nameof(RPC_Eat), RpcTarget.All, b.photonView.ViewID, a.photonView.ViewID);
        }
    }

    [PunRPC]
    private void RPC_Eat(int winnerViewId, int loserViewId)
    {
        var winnerView = PhotonView.Find(winnerViewId);
        var loserView = PhotonView.Find(loserViewId);

        if (winnerView == null || loserView == null) return;

        var winner = winnerView.GetComponent<PlayerMass>();
        var loser = loserView.GetComponent<PlayerMass>();

        if (winner == null || loser == null) return;
        if (winner._dead || loser._dead) return;

        winner._mass += loser._mass;
        winner.ResizeLocal();

        loser._dead = true;
        loser.DisableLocal();

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ForceLeave), loser.photonView.Owner);
        }

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    PhotonNetwork.Destroy(loser.photonView);
        //}
    }

    private void DisableLocal()
    {
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        

        if (photonView.IsMine)
        {
            FindFirstObjectByType<GameManager>()?.Leave();
        }
    }

    [PunRPC] void RPC_ForceLeave() { PhotonNetwork.LeaveRoom(); }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_mass);
            stream.SendNext(_dead);
        }
        else
        {
            _mass = (float)stream.ReceiveNext();
            _dead = (bool)stream.ReceiveNext();
            ResizeLocal();
        }
    }

    private void ResizeLocal()
    {
        float newSize = _mass / 10f;
        Vector3 targetScale = Vector3.one * newSize;

        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(targetScale, 0.5f);

        if (photonView.IsMine)
        {
            OnMassChangeAction?.Invoke();
            PlayerUI.OnMassChangedEvent?.Invoke((int)_mass);
        }
    }

    private IEnumerator MassLossCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_massLossSpeed / _mass);
            if (_mass > 35)
            {
                _mass--;
                ResizeLocal();
            }
        }
    }
}
