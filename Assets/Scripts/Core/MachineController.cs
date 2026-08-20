using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineController : MonoBehaviour
{
    [SerializeField] private Animator m_handleAnimator;
    [SerializeField] private ReelController[] m_Reels;
    [SerializeField] private SCO_SlotItem[] m_SlotItems;
    
    private RNG m_RNG;
    private float m_cooldown = 0.5f;
    private float m_cooldownDuration = 0.5f;

    private int m_currentReel = -1; //To keep track of the reel which is to be stopped next

    private InputSystem_Actions m_inputSytem;
    private void Awake()
    {
        m_inputSytem = new InputSystem_Actions();
        m_RNG = new RNG();
        foreach(var reel in m_Reels)
        { 
            reel.ReelInit(m_SlotItems, m_RNG);
        }
    }
    
    private void OnEnable()
    {
        m_inputSytem.Enable();
        m_inputSytem.Player.Spin.performed += PullHandle_InputAction;
    }
    
    private void OnDisable()
    {
        m_inputSytem.Player.Spin.performed -= PullHandle_InputAction;
        m_inputSytem.Disable();
    }

    private void PullHandle_InputAction(InputAction.CallbackContext context)
    {
        PullHandle();
    }
    
    
    public void PullHandle()
    {
        
        SCO_SlotItem target = m_RNG.Pick(m_SlotItems, s=>s.weight);
        m_handleAnimator.Play("HandlePlay"); //Using direct name because there is just one animation else I would be using hashed values
        if (m_currentReel == -1)
        {
            if(!BettingManager.instance.TryPlaceBet()) return;
            foreach (var reel in m_Reels){ reel.RequestSpin(); } 
            m_currentReel++;
        }
        else { m_Reels[m_currentReel].RequestSpin();
            m_currentReel++;
        }
        
        //Checking if all reels stopped....If yes, then CheckingResult and Resetting Machine
        if (m_currentReel >= m_Reels.Length)
        {
            m_currentReel = -1;
            GameManager.instance.CheckSlots(m_Reels);
        }
        
        m_cooldown = m_cooldownDuration;
    }
    
    private void Update()
    {
        m_cooldown -= Time.deltaTime;
        if(m_cooldown > 0) return; 
    }
}

