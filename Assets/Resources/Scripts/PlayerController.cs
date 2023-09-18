using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    [SerializeField] private float movementSpeed = 1.0f;
    
    private Vector3 m_Movement;
    
    private void Update() {
        if (m_Movement == Vector3.zero) return;
        var position = transform.position;
        position = Vector3.MoveTowards(position, position + m_Movement,
            Time.deltaTime * movementSpeed);
        transform.position = position;
    }

    public void OnMove(InputValue inputValue) {
        m_Movement = inputValue.Get<Vector2>();
    }

    public void OnA() {
        Debug.Log("A Pressed");
    }

    public void OnB() {
        Debug.Log("B Pressed");
    }

    public void OnSelect() {
        Debug.Log("Select Pressed");
    }

    public void OnStart() {
        Debug.Log("Start Pressed");
    }
}
