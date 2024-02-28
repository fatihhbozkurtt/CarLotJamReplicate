using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    Animator animator;
    CharacterHandler characterHandler;
     void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterHandler = GetComponent<CharacterHandler>();
    }

    private void Start()
    {
        InputManager.instance.NewCharacterSelected += OnNewCharacterSelected;
    }

    private void OnNewCharacterSelected(CharacterHandler selectedChar)
    {
        if (characterHandler != selectedChar) return;

        TriggerAnimation("victory");
    }


    public void TriggerAnimation(string animName)
    {
        animator.SetTrigger(animName);
    }
}
