using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviourPun
{
    private Canvas _canvas;
    private Camera _playerCamera;
    private TextMeshProUGUI _massText;

    public static UnityAction<int> OnMassChangedEvent;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        _canvas = FindAnyObjectByType<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("PlayerUI: No Canvas found in the scene.");
            return;
        }

        _playerCamera = GetComponentInChildren<Camera>(true);

        _massText = GameObject.Find("MassText")?.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetCanvasCamera();
        //SetMouse();
        //setLeaveButton();
        //_massText.text = "Масса: 10";
    }

    private void OnEnable()
    {
        OnMassChangedEvent += ChangeMassText;
    }

    private void OnDisable()
    {
        OnMassChangedEvent -= ChangeMassText;
    }

    private void SetCanvasCamera()
    {
        if (_canvas != null && _playerCamera != null)
        {
            _canvas.worldCamera = _playerCamera;
        }
    }

    private void ChangeMassText(int newMass)
    {
        _massText.text = $"Масса: {newMass}";
    }

    //private void setLeaveButton()
    //{
    //    var canvas = GameObject.Find("Canvas");
    //    if (canvas == null)
    //    {
    //        Debug.LogError("Canvas not found");
    //        return;
    //    }
    //
    //    var button = canvas.GetComponentInChildren<Button>(true);
    //    if (button == null)
    //    {
    //        Debug.LogError("Button not found");
    //        return;
    //    }
    //
    //    Debug.Log($"Found button: {button.name}");
    //
    //    button.onClick.AddListener(Leave);
    //    Debug.Log("Leave listener added");
    //}
    //
    //public void Leave()
    //{
    //    PhotonNetwork.LeaveRoom();
    //}
    //
    //public void SetMouse()
    //{
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //}
}