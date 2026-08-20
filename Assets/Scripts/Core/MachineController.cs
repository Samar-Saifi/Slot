using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineController : MonoBehaviour
{
    [SerializeField] private Animator m_handleAnimator;
    [SerializeField] private ReelController m_Reel;
    [SerializeField] private SCO_SlotItem[] m_SlotItems;
    
    private RNG m_RNG;
    private float m_cooldown = 0.5f;
    private float m_cooldownDuration = 0.5f;

    private InputSystem_Actions m_inputSytem;
    private void Awake()
    {
        m_RNG = new RNG();
        m_Reel.ReelInit(m_SlotItems, m_RNG);
        m_inputSytem = new InputSystem_Actions();
    }
    
    private void OnEnable()
    {
        m_inputSytem.Enable();
        m_inputSytem.Player.Spin.performed += PullHandle;
    }
    
    private void OnDisable()
    {
        m_inputSytem.Player.Spin.performed -= PullHandle;
        m_inputSytem.Disable();
    }

    private void PullHandle(InputAction.CallbackContext context)
    {
        if (m_cooldown > 0) return;
        SCO_SlotItem target = m_RNG.Pick(m_SlotItems, s=>s.weight);
        m_handleAnimator.Play("HandlePlay"); //Using direct name because there is just one animation else I would be using hashed values
        m_Reel.RequestSpin();
        m_cooldown = m_cooldownDuration;
    }
    
    private void Update()
    {
        m_cooldown -= Time.deltaTime;
        if(m_cooldown > 0) return; 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }
}

