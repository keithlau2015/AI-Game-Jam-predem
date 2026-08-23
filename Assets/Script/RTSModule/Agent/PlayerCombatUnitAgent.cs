
using AbilityModule;
using CombatUnitModule;
using EquipmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Model.SkillModel;
using static UnityEngine.InputSystem.InputAction;

namespace CombatUnitModule
{
    public class PlayerCombatUnitAgent : CombatUnitAgent
    {
        private float turnSpeed = 50f;
        private Vector3 rotate;
        private int currentSpeedModifierIndex = 0;
        public float currentSpeedModifier { get; private set; }
        public readonly float[] speedModifier = new float[] { 0f, 0.25f, 0.5f, 0.75f, 1f };
        public event Action<float> onSpeedRateChange;
        private float speedLerpTime;

        public override void SetUp(CombatUnitData ins)
        {
            if (!InputManager.singleton.playerControl.Player.Move.enabled)
                InputManager.singleton.playerControl.Player.Move.Enable();

            //InputManager.singleton.playerControl.Player.Move.started += MovementHandler;
            InputManager.singleton.playerControl.Player.Move.performed += MovementHandler;
            InputManager.singleton.playerControl.Player.Move.canceled += MovementHandler;

            if (!InputManager.singleton.playerControl.Player.Skill_1.enabled)
            {
                InputManager.singleton.playerControl.Player.Skill_1.Enable();
                InputManager.singleton.playerControl.Player.Skill_1.performed += Skill_1Handler;
            }

            if (!InputManager.singleton.playerControl.Player.Skill_2.enabled)
            {
                InputManager.singleton.playerControl.Player.Skill_2.Enable();
                InputManager.singleton.playerControl.Player.Skill_2.performed += Skill_2Handler;
            }

            if (!InputManager.singleton.playerControl.Player.Skill_3.enabled)
            {
                InputManager.singleton.playerControl.Player.Skill_3.Enable();
                InputManager.singleton.playerControl.Player.Skill_3.performed += Skill_3Handler;
            }

            if (!InputManager.singleton.playerControl.Player.Skill_4.enabled)
            {
                InputManager.singleton.playerControl.Player.Skill_4.Enable();
                InputManager.singleton.playerControl.Player.Skill_4.performed += Skill_4Handler;
            }

            base.SetUp(ins);
        }

        private void MovementHandler(CallbackContext callbackContext)
        {
            Vector2 input = callbackContext.ReadValue<Vector2>().normalized;
            //Change the speed modifier logic
            if (input.y == 1)
            {
                currentSpeedModifierIndex++;
                if (currentSpeedModifierIndex > speedModifier.Length - 1)
                    currentSpeedModifierIndex = speedModifier.Length - 1;
                onSpeedRateChange?.Invoke(speedModifier[currentSpeedModifierIndex]);
            }
            else if (input.y == -1)
            {
                currentSpeedModifierIndex--;
                if (currentSpeedModifierIndex < 0)
                    currentSpeedModifierIndex = 0;
                onSpeedRateChange?.Invoke(speedModifier[currentSpeedModifierIndex]);
            }

            //Decide x direction
            rotate = input.x != 0 ? input.x == 1f ? new Vector3(0, 0.5f, 0) : input.x == -1f ? new Vector3(0, -0.5f, 0) : Vector3.zero : Vector3.zero;
        }

        private void Skill_1Handler(CallbackContext callbackContext)
        {
            if (skillMap.Count == 0) return;
            List<Skill> skills = this.skillMap[0];
            foreach (Skill skill in skills)
            {
                TriggerSkill(skill);
            }
        }

        private void Skill_2Handler(CallbackContext callbackContext)
        {
            if (skillMap.Count == 1) return;
            List<Skill> skills = this.skillMap[1];
            foreach (Skill skill in skills)
            {
                TriggerSkill(skill);
            }
        }

        private void Skill_3Handler(CallbackContext callbackContext)
        {
            if (skillMap.Count == 2) return;
            List<Skill> skills = this.skillMap[2];
            foreach (Skill skill in skills)
            {
                TriggerSkill(skill);
            }
        }

        private void Skill_4Handler(CallbackContext callbackContext)
        {
            if (skillMap.Count == 3) return;
            List<Skill> skills = this.skillMap[3];
            foreach (Skill skill in skills)
            {
                TriggerSkill(skill);
            }
        }

        private void Skill_5Handler(CallbackContext callbackContext)
        {
            if (skillMap.Count == 5) return;
            List<Skill> skills = this.skillMap[4];
            foreach (Skill skill in skills)
            {
                TriggerSkill(skill);
            }
        }

        private async void TriggerSkill(Skill skill)
        {
            if (skill != null && (skill.Model.targetType.Equals((int)TargetType.assignable) || skill.Model.targetType.Equals((int)TargetType.direction)))
            {
                DisableOtherSkillPreviewRange(skill);
                if (skill.IsPreviewingRange())
                {
                    Cursor.visible = true;
                    skill.CancelPreviewRange();
                    Texture2D cursorTexture = await AssetsBundleManager.LoadTexture2D("Default_Cursor");
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    int equipmentIndex = GetEquipmentIndexBySkill(skill.Model.key.ToString());
                    GameObject slotGo = equipmentSlots[equipmentIndex];
                    Transform equipmentTran = slotGo.transform.GetChild(0);
                    AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
                    if (autoRotate)
                    {
                        autoRotate.SetTarget(null);
                    }
                    else
                    {
                        Debug.LogWarning($"Skill {skill.Model.key} does not have AutoRotateToLockedTarget component on equipment index {equipmentIndex}.");
                    }
                    //skill.Execute();
                }
                else
                {
                    skill.PreviewRange();
                    if (skill.Model.targetType.Equals((int)TargetType.assignable))
                    {
                        Cursor.visible = true;
                        Texture2D cursorTexture = await AssetsBundleManager.LoadTexture2D("Default_Select_Target_Cursor");
                        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    }
                    else
                    {
                        Cursor.visible = false;
                    }

                    int equipmentIndex = GetEquipmentIndexBySkill(skill.Model.key.ToString());
                    GameObject slotGo = equipmentSlots[equipmentIndex];
                    Transform equipmentTran = slotGo.transform.GetChild(0);
                    AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
                    if (autoRotate)
                    {
                        autoRotate.SetTarget(CursorManager.singleton.cursorAnchor.transform);
                    }
                    else
                    {
                        Debug.LogWarning($"Skill {skill.Model.key} does not have AutoRotateToLockedTarget component on equipment index {equipmentIndex}.");
                    }
                }
            }
            else if (skill.Model.targetType.Equals((int)TargetType.auto))
            {
                skill.SetActive(!skill.IsActive);
            }
        }

        private async void DisableAllSkillPreviewRange()
        {
            foreach (Skill skill in skillMap.SelectMany(skillList => skillList.Value).ToList())
            {
                if (skill.IsPreviewingRange())
                {
                    Cursor.visible = true;
                    skill.CancelPreviewRange();
                    Texture2D cursorTexture = await AssetsBundleManager.LoadTexture2D("Default_Cursor");
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    int equipmentIndex = GetEquipmentIndexBySkill(skill.Model.key.ToString());
                    GameObject slotGo = equipmentSlots[equipmentIndex];
                    Transform equipmentTran = slotGo.transform.GetChild(0);
                    AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
                    if (autoRotate)
                    {
                        autoRotate.SetTarget(null);
                    }
                    else
                    {
                        Debug.LogWarning($"Skill {skill.Model.key} does not have AutoRotateToLockedTarget component on equipment index {equipmentIndex}.");
                    }
                    skill.CancelPreviewRange();
                }
            }
        }

        private void DisableOtherSkillPreviewRange(Skill exclude)
        {

            foreach (Skill skill in skillMap.SelectMany(skillList => skillList.Value).ToList())
            {
                if (skill.Equals(exclude)) continue;
                if (skill.IsPreviewingRange())
                {
                    int equipmentIndex = GetEquipmentIndexBySkill(skill.Model.key.ToString());
                    GameObject slotGo = equipmentSlots[equipmentIndex];
                    Transform equipmentTran = slotGo.transform.GetChild(0);
                    AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
                    if (autoRotate)
                    {
                        autoRotate.SetTarget(null);
                    }
                    else
                    {
                        Debug.LogWarning($"Skill {skill.Model.key} does not have AutoRotateToLockedTarget component on equipment index {equipmentIndex}.");
                    }
                    skill.CancelPreviewRange();
                }
            }
        }

        private void Update()
        {
            if (GameStateController.singleton.IsPause) return;

            //Speed Lerp
            if (currentSpeedModifierIndex > 0 && currentSpeedModifier != speedModifier[currentSpeedModifierIndex])
            {
                currentSpeedModifier = currentSpeedModifier + speedLerpTime;
                if (currentSpeedModifier > speedModifier[currentSpeedModifierIndex])
                {
                    currentSpeedModifier = speedModifier[currentSpeedModifierIndex];
                }
                speedLerpTime = 0;
            }

            //Debug.Log($"current speed modifier: {currentSpeedModifier}");

            transform.Rotate(rotate * turnSpeed * (0.5f + (currentSpeedModifier * _agent.speed) / 2) * Time.deltaTime);

            if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
                _agent.Move((currentSpeedModifierIndex > 0 ? transform.forward : Vector3.zero) * currentSpeedModifier * _agent.speed * Time.deltaTime);
            if (currentSpeedModifierIndex > 0 && currentSpeedModifier != speedModifier[currentSpeedModifierIndex])
            {
                speedLerpTime += Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            if (!InputManager.singleton || InputManager.singleton.playerControl == null)
                return;

            if (InputManager.singleton.playerControl.Player.Move.enabled)
                InputManager.singleton.playerControl.Player.Move.Disable();
            if (InputManager.singleton.playerControl.Player.Skill_1.enabled)
                InputManager.singleton.playerControl.Player.Skill_1.Disable();
            if (InputManager.singleton.playerControl.Player.Skill_2.enabled)
                InputManager.singleton.playerControl.Player.Skill_2.Disable();
            if (InputManager.singleton.playerControl.Player.Skill_3.enabled)
                InputManager.singleton.playerControl.Player.Skill_3.Disable();
            if (InputManager.singleton.playerControl.Player.Skill_4.enabled)
                InputManager.singleton.playerControl.Player.Skill_4.Disable();
            if (InputManager.singleton.playerControl.Player.Move.enabled)
                InputManager.singleton.playerControl.Player.Move.Disable();

            InputManager.singleton.playerControl.Player.Skill_1.performed -= Skill_1Handler;
            InputManager.singleton.playerControl.Player.Skill_2.performed -= Skill_2Handler;
            InputManager.singleton.playerControl.Player.Skill_3.performed -= Skill_3Handler;
            InputManager.singleton.playerControl.Player.Skill_4.performed -= Skill_4Handler;
            InputManager.singleton.playerControl.Player.Move.performed -= MovementHandler;

            //InputManager.singleton.playerControl.Player.Skill_5.Disable();
        }
    }
}