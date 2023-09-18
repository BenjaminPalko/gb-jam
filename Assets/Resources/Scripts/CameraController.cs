using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    
    public Target target {
        set {
            switch (value) {
                case Target.None:
                    m_TargetEnum = value;
                    m_Target = null;
                    break;
                case Target.Player:
                    m_TargetEnum = value;
                    var playerController = FindObjectOfType<PlayerController>();
                    if (playerController) {
                        m_TargetEnum = value;
                        m_Target = playerController.transform;
                    } else {
                        m_TargetEnum = Target.None;
                        m_Target = null;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Target");
            }
        }
    }

    private Target m_TargetEnum;
    private Transform m_Target;
    
    private Camera m_Camera;

    private void Awake() {
        m_Camera = GetComponent<Camera>();
    }

    private void Start() {
        target = Target.Player;
    }

    private void Update() {
        if (!m_Target) return;
        var targetPosition = m_Target.position;
        var cameraTransform = transform;
        cameraTransform.position = new Vector3(targetPosition.x, targetPosition.y, cameraTransform.position.z);
    }

    public enum Target {
        None,
        Player
    }
}
