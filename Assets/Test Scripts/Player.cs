using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    #region Input
    private DefaultInputActions input;

    private void Awake()
    {
        input = new DefaultInputActions();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
    #endregion Input

    private Rigidbody rb;
    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(input.Player.Move.ReadValue<Vector2>().x, 0.0f, input.Player.Move.ReadValue<Vector2>().y);

        rb.AddForce(Quaternion.Euler(Vector3.up * cam.transform.eulerAngles.y) * movement * 10.0f);
    }
}
