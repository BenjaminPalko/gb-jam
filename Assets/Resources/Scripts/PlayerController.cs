using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private GameObject beam;
    
    private Vector3 m_Movement;
    private bool m_Abduction;
    
    private void Update() {
        if (m_Movement != Vector3.zero && !m_Abduction) Movement();
        else if (m_Abduction) Abduct();
    }

    public void OnMove(InputValue inputValue) {
        m_Movement = inputValue.Get<Vector2>();
    }

    public void OnA(InputValue inputValue) {
        m_Abduction = inputValue.isPressed;
        beam.SetActive(m_Abduction);
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
    
    private void Movement() {
        var position = transform.position;
        position = Vector3.MoveTowards(position, position + m_Movement,
            Time.deltaTime * movementSpeed);
        transform.position = position;
    }

    private void Abduct() {
    }
}
