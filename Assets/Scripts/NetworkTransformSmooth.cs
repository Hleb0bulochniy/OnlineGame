using Photon.Pun;
using UnityEngine;

public class NetworkTransformSmooth : MonoBehaviourPun, IPunObservable
{
    [Header("Smoothing")]
    public float positionLerp = 12f;
    public float rotationLerp = 12f;
    public float snapDistance = 3f;

    private Vector3 netPos;
    private Quaternion netRot;

    void Awake()
    {
        netPos = transform.position;
        netRot = transform.rotation;
    }

    void Update()
    {
        if (photonView.IsMine) return;

        if ((transform.position - netPos).sqrMagnitude > snapDistance * snapDistance)
            transform.position = netPos;
        else
            transform.position = Vector3.Lerp(transform.position, netPos, Time.deltaTime * positionLerp);

        transform.rotation = Quaternion.Slerp(transform.rotation, netRot, Time.deltaTime * rotationLerp);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            netPos = (Vector3)stream.ReceiveNext();
            netRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
