using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUI;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using LocalizationModule;

public class ControlPrefSettingPanel : CommonPopUpPanel
{
    [SerializeField]
    private Button moveForward, moveBackward, moveLeft, moveRight, interaction;
    [SerializeField]
    private Text moveFowardKeyCode, moveBackwardKeyCode, moveLeftKeyCode, moveRightKeyCode, interactionKeyCode;
    private InputAction moveAction, interactAction;

    private void Start()
    {
        Show();
    }

    public override void Show()
    {
        moveAction = InputManager.singleton.playerControl.Player.Move;
        interactAction = InputManager.singleton.playerControl.UI.Click;

        int moveForwardBindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[0]);
        moveFowardKeyCode.text = InputControlPath.ToHumanReadableString(
            moveAction.bindings[moveForwardBindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        int moveBackwardBindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[1]);
        moveBackwardKeyCode.text = InputControlPath.ToHumanReadableString(
            moveAction.bindings[moveBackwardBindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        int moveLeftBindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[2]);
        moveLeftKeyCode.text = InputControlPath.ToHumanReadableString(
            moveAction.bindings[moveLeftBindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        int moveRightBindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[3]);
        moveRightKeyCode.text = InputControlPath.ToHumanReadableString(
            moveAction.bindings[moveRightBindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        int interactionBindingIndex = interactAction.GetBindingIndexForControl(interactAction.controls[0]);
        interactionKeyCode.text = InputControlPath.ToHumanReadableString(
            interactAction.bindings[interactionBindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        moveForward.onClick.AddListener(() => {
            moveFowardKeyCode.text = LocalizationManager.singleton.GetLocalization("SYS_WaitingPressKeyCode");
            int bindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[0]);
            moveAction.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f).OnCancel(operation => {
                moveFowardKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
            }).OnComplete(operation => {
                if (CheckDuplicateBindings(moveAction, bindingIndex, true))
                {
                    moveAction.RemoveBindingOverride(bindingIndex);
                    moveFowardKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                    operation.Dispose();
                    return;
                }
                moveFowardKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
                InputManager.singleton.SaveCurrentKeycodeBindings();
            }).Start();
        });

        moveBackward.onClick.AddListener(() => {
            moveBackwardKeyCode.text = LocalizationManager.singleton.GetLocalization("SYS_WaitingPressKeyCode");
            int bindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[1]);
            moveAction.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f).OnCancel(operation => {
                moveBackwardKeyCode.text = InputControlPath.ToHumanReadableString(
                        moveAction.bindings[bindingIndex].effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
            }).OnComplete(operation => {
                if (CheckDuplicateBindings(moveAction, bindingIndex, true))
                {
                    moveAction.RemoveBindingOverride(bindingIndex);
                    moveBackwardKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                    operation.Dispose();
                    return;
                }
                moveBackwardKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
                InputManager.singleton.SaveCurrentKeycodeBindings();
            }).Start();
        });

        moveLeft.onClick.AddListener(() => {
            moveLeftKeyCode.text = LocalizationManager.singleton.GetLocalization("SYS_WaitingPressKeyCode");
            int bindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[2]);
            moveAction.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f).OnCancel(operation => {
                moveLeftKeyCode.text = InputControlPath.ToHumanReadableString(
                        moveAction.bindings[bindingIndex].effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
            }).OnComplete(operation => {
                if (CheckDuplicateBindings(moveAction, bindingIndex, true))
                {
                    moveAction.RemoveBindingOverride(bindingIndex);
                    moveLeftKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                    operation.Dispose();
                    return;
                }
                moveLeftKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
                moveAction.SaveBindingOverridesAsJson();
                InputManager.singleton.SaveCurrentKeycodeBindings();
            }).Start();
        });

        moveRight.onClick.AddListener(() => {
            moveRightKeyCode.text = LocalizationManager.singleton.GetLocalization("SYS_WaitingPressKeyCode");
            int bindingIndex = moveAction.GetBindingIndexForControl(moveAction.controls[3]);
            moveAction.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f).OnCancel(operation => {
                moveRightKeyCode.text = InputControlPath.ToHumanReadableString(
                        moveAction.bindings[bindingIndex].effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
            }).OnComplete(operation => {
                if (CheckDuplicateBindings(moveAction, bindingIndex, true))
                {
                    moveAction.RemoveBindingOverride(bindingIndex);
                    moveRightKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                    operation.Dispose();
                    return;
                }
                moveRightKeyCode.text = InputControlPath.ToHumanReadableString(
                    moveAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
                InputManager.singleton.SaveCurrentKeycodeBindings();
            }).Start();
        });

        interaction.onClick.AddListener(() => {
            interactionKeyCode.text = LocalizationManager.singleton.GetLocalization("SYS_WaitingPressKeyCode");
            int bindingIndex = interactAction.GetBindingIndexForControl(interactAction.controls[0]);
            interactAction.PerformInteractiveRebinding().WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f).OnCancel(operation => {
                interactionKeyCode.text = InputControlPath.ToHumanReadableString(
                        interactAction.bindings[bindingIndex].effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
            }).OnComplete(operation => {
                if(CheckDuplicateBindings(interactAction, bindingIndex))
                {
                    interactAction.RemoveBindingOverride(bindingIndex);
                    interactionKeyCode.text = InputControlPath.ToHumanReadableString(
                    interactAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                    operation.Dispose();
                    return;
                }
                interactionKeyCode.text = InputControlPath.ToHumanReadableString(
                    interactAction.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                operation.Dispose();
                InputManager.singleton.SaveCurrentKeycodeBindings();
            }).Start();
        });

        base.Show();
    }

    private bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        InputBinding newBinding = action.bindings[bindingIndex];
        foreach (InputBinding binding in action.actionMap.bindings)
        {
            if (binding.action == newBinding.action)
                continue;

            if (binding.effectivePath == newBinding.effectivePath)
            {
                Debug.LogWarning("Found Duplicate Bindings");
                return true;
            }
        }

        if (allCompositeParts)
        {
            for(int i = 0; i < action.bindings.Count; i++)
            {
                if (i == bindingIndex)
                    continue;

                if (action.bindings[i].effectivePath == newBinding.effectivePath)
                {
                    Debug.LogWarning($"Found Duplicate Bindings ,{action.bindings[i].effectivePath}");
                    return true;
                }
            }
        }
        return false;
    }

    public override void Hide()
    {
        tweenAlpha.SetOnCompleteCB(() => { Destroy(gameObject); });
        base.Hide();
    }
}