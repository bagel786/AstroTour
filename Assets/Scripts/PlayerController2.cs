using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2 : MonoBehaviour

{
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    public GameObject consoleUI;
    // Variables related to player character movement
    public InputAction MoveAction;
    public InputAction talkAction;

    private Rigidbody2D rigidbody2d;
    private Vector2 move;
    public float speed = 3.0f;

    // Variables related to animation
    private Animator animator;
    private Vector2 moveDirection = new Vector2(1, 0);

    void OnEnable()
    {
        // Enable input actions and hook into event
        MoveAction.Enable();
        talkAction.Enable();

        talkAction.performed += OnTalkPerformed;
    }

    void OnDisable()
    {
        // Always disable actions and unsubscribe
        MoveAction.Disable();
        talkAction.Disable();

        talkAction.performed -= OnTalkPerformed;
    }

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Read movement input
        if (PauseController.IsGamePaused)
        {
            rigidbody2d.linearVelocity = Vector2.zero;
            StopFootSteps();
            return;
        }
        move = MoveAction.ReadValue<Vector2>();

        // Update move direction if moving
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        // Footstep logic
        if (move.magnitude > 0.1f && !playingFootsteps)
        {
            StartFootSteps();
        }
        else if (move.magnitude <= 0.1f && playingFootsteps)
        {
            StopFootSteps();
        }

        // Update animation parameters
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);
    }

    void FixedUpdate()
    {
        if (PauseController.IsGamePaused)
        {
            rigidbody2d.linearVelocity = Vector2.zero; // hard stop
            return;
        }

        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    // Event callback for talk action
    private void OnTalkPerformed(InputAction.CallbackContext context)
    {
        // Don't process interactions when game is paused (e.g., console is open)
        if (PauseController.IsGamePaused)
        {
            return;
        }

        FindFriend();
        FindSpaceShip();
        FindBox();
        // FindComputer();
        FindTerminal();
        FindMarketingTerminal();
    }
    void FindBox()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("Decorations"));
        if (hit.collider != null)
        {
            Box box = hit.collider.GetComponent<Box>();
            if (box != null)
            {
                box.Interact();
            }
        }
    }
    void FindSpaceShip()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("Decorations"));
        
    }
   

    void FindTerminal()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("Terminal"));
        if (hit.collider != null)
        {
            // Check for SimpleTerminalInteract first
            SimpleTerminalInteract terminal = hit.collider.GetComponent<SimpleTerminalInteract>();
            if (terminal != null && terminal.canInteract())
            {
                // Force stop movement + reset animation
                move = Vector2.zero;
                animator.SetFloat("Speed", 0f);
                StopFootSteps();

                terminal.Interact();
                return;
            }

            // Check for SimpleDNASequencingTerminal
            SimpleDNASequencingTerminal dnaTerminal = hit.collider.GetComponent<SimpleDNASequencingTerminal>();
            if (dnaTerminal != null && dnaTerminal.canInteract())
            {
                // Force stop movement + reset animation
                move = Vector2.zero;
                animator.SetFloat("Speed", 0f);
                StopFootSteps();

                dnaTerminal.Interact();
                return;
            }
        }
    }

    void FindMarketingTerminal()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("Terminal"));
        if (hit.collider != null)
        {
            MarketingTerminal marketingTerminal = hit.collider.GetComponent<MarketingTerminal>();
            if (marketingTerminal != null && marketingTerminal.canInteract())
            {
                // Force stop movement + reset animation
                move = Vector2.zero;
                animator.SetFloat("Speed", 0f);
                StopFootSteps();

                marketingTerminal.Interact();
            }
        }
    }

    void FindFriend()
    {
        RaycastHit2D hit = Physics2D.Raycast(
        rigidbody2d.position + Vector2.up * 0.2f,
        moveDirection,
        1.5f,
        LayerMask.GetMask("NPC")
        );

        if (hit.collider != null)
        {

            LabNPC npc = hit.collider.GetComponent<LabNPC>();
            if (npc != null)
            {
                // Force stop movement + reset animation
                move = Vector2.zero;
                animator.SetFloat("Speed", 0f);
                StopFootSteps();

                npc.StartDialogue();
            }
        }
    }

    void StartFootSteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootStep), 0f, footstepSpeed);
    }
    void StopFootSteps()
    {
        playingFootsteps = false; 
        CancelInvoke(nameof(PlayFootStep));
    }
    void PlayFootStep()
    {
        SoundEffectManager.Play("Footstep", true);

    }

}

