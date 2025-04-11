using UnityEngine;
using UnityEngine.InputSystem;

public enum MyStates
{
    IDLE,
    WALKING,
    RUNNING,
    JUMPING,
    FALLING,
    ATTACKING,
    DEAD
}

public class StatesTest : MonoBehaviour
{
    [SerializeField] MyStates m_States;
    [SerializeField] float m_Speed;
    [SerializeField] float m_JumpForce;
    [SerializeField] LayerMask m_GroundMask;
    float elapsed = 0;
    Rigidbody m_Rigidbody;
    Vector2 m_MovementInput;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (m_States)
        {
            case MyStates.IDLE:
                UpdateIdle();
                break;
            case MyStates.RUNNING:
                UpdateRunning();
                break;
            case MyStates.FALLING:
                UpdateFalling();
                break;
            case MyStates.DEAD:
                UpdateDead();
                break;
        }
    }

    void UpdateIdle()
    {
        Debug.Log("I'm in idle state");

        var movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (movementInput != Vector2.zero)
        {
            m_States = MyStates.RUNNING;
        }

        /*if(Input.GetButtonDown("Jump"))
        {
            m_States = MyStates.JUMPING;
        }*/

        
    }

    void UpdateRunning()
    {
        Debug.Log("I'm in running state");

        var movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (movementInput == Vector2.zero)
        {
            m_States = MyStates.IDLE;
        }

        transform.Translate(new Vector3(movementInput.x, 0, movementInput.y) * Time.deltaTime * m_Speed);

        /*if(Input.GetButtonDown("Jump"))
         {
            m_States = MyStates.JUMPING;
            GetComponent<Rigidbody>().AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
         }*/
    }

    public void UpdateFalling()
    {
        Debug.Log("I'm falling :(");

        if (m_Rigidbody.linearVelocity.y < 0 && Physics.Raycast(transform.position, Vector3.down, 1, m_GroundMask)) 
        {
            m_States = MyStates.IDLE;
            elapsed = 0;
        }
    }

    void UpdateDead()
    {
        Debug.Log("OH NO IM DEAD");
    }

    void OnMovement(InputValue v)
    {
        m_MovementInput = v.Get<Vector2>();
        Debug.Log("Moving...");
    }

    void OnJump()
    {
        m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
        m_States = MyStates.FALLING;
    }



}
