using AbilityModule;
using CombatUnitModule;
using Cinemachine;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// RTS control hub: camera pan/zoom/rotate, selection hooks, and unit command input.
/// Game-specific flow (menus, level load, battle HUD) belongs in a project layer, not here.
/// </summary>
public class GameStateMachine : StateMachine
{
    #region Battle Camera
    private CinemachineVirtualCamera virtualCam;
    private CinemachineTransposer camTransposer;

    public void SetCinemachineVirtualCamera(CinemachineVirtualCamera virtualCamera)
    {
        this.virtualCam = virtualCamera;
        camTransposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
        if (camTransposer != null)
            targetFollowOffset = camTransposer.m_FollowOffset;
        else
            targetFollowOffset = camTransform.position - transform.position;
        camTransform.LookAt(this.transform);
        virtualCam.Follow = this.transform;
        virtualCam.LookAt = this.transform;
        lastPosition = this.transform.position;
    }

    public void SetMiniMapCam(MiniMapCam miniMapCam)
    {
        miniMapCam.SetFollowTarget(this.gameObject);
    }

    private Transform camPivot;
    private Transform camTransform { get { return virtualCam.transform; } }
    private GameObject lockTarget;
    #endregion

    #region Horizontal Translation
    private float maxSpeed = 100f;
    private float speed;
    private float acceleration = 10f;
    private float damping = 15f;
    #endregion

    #region Camera Zoom
    private float stepSize = 10;
    private float zoomDampening = 7.5f;
    private float minZoomDistance = 30f;
    private float maxZoomDistance = 220f;
    #endregion

    #region Rotation
    private float maxRotationSpeed = 1;
    #endregion

    private float edgeTolerance = 0.05f;

    private Vector3 targetCameraPosition;
    private Vector3 targetFollowOffset;
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;
    private Vector3 startDrag;

    private float startBattleTime = 0;
    public float curBattleTime
    {
        get
        {
            if (startBattleTime == 0)
                return 0;
            return Time.time - startBattleTime;
        }
    }

    public void UpdateStartBattleTime()
    {
        startBattleTime = Time.time;
    }

    public void EndStartBattleTime()
    {
        startBattleTime = 0;
    }

    public List<CombatUnitAgent> unitList
    {
        get
        {
            return CursorManager.singleton.selectableList
                .Where(s => s.GetGameObject().GetComponent<CombatUnitAgent>() != null)
                .Select(s => s.GetGameObject().GetComponent<CombatUnitAgent>())
                .ToList();
        }
    }

    public List<CombatUnitAgent> selectedUnitList
    {
        get
        {
            List<CombatUnitAgent> result = unitList.Where(x => x.isSelected).ToList();
            if (lastSelectUnit != null && !result.Contains(lastSelectUnit))
                result.Add(lastSelectUnit);
            return result;
        }
    }

    public CombatUnitAgent lastSelectUnit;
    public CombatUnitAgent recentSelectUnit
    {
        get
        {
            CombatUnitAgent unitEntity = null;
            CursorManager.singleton.currentSelectable?.GetGameObject().TryGetComponent(out unitEntity);
            return unitEntity;
        }
    }

    public CombatUnitAgent recentHoverUnit
    {
        get
        {
            CombatUnitAgent unitEntity = null;
            CursorManager.singleton.CurrentHoverable?.GetGameObject().TryGetComponent(out unitEntity);
            return unitEntity;
        }
    }

    public List<CombatUnitAgent> playerUnitEntities
    {
        get
        {
            return CombatUnitAgent.allUnitEntities.Where(x => x.team == Team.Blue).OrderBy(x => x.index).ToList();
        }
    }

    public void SetInitCamView(GameObject playerGo)
    {
        this.lockTarget = playerGo;
    }

    public bool IsPauseState()
    {
        return GameStateController.singleton != null && GameStateController.singleton.IsPause;
    }

    public bool IsGameStartState()
    {
        return currentState == null;
    }

    public bool IsBattleActive()
    {
        return true;
    }

    public void CheckMouseAtScreenEdge()
    {
        if (IsPauseState() || virtualCam == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x < edgeTolerance * Screen.width)
            moveDirection += -GetCameraRight();
        else if (mousePosition.x > (1 - edgeTolerance) * Screen.width)
            moveDirection += GetCameraRight();

        if (mousePosition.y < edgeTolerance * Screen.height)
            moveDirection += -GetCameraForward();
        else if (mousePosition.y > (1 - edgeTolerance) * Screen.height)
            moveDirection += GetCameraForward();

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            if (camPivot != null && (!virtualCam.Follow.Equals(camPivot) || !virtualCam.LookAt.Equals(camPivot)))
                lockTarget = null;
        }

        targetCameraPosition += moveDirection;
    }

    public void UpdateBasePosition()
    {
        if (lockTarget != null)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            this.transform.position = new Vector3(lockTarget.transform.position.x, this.transform.position.y, lockTarget.transform.position.z);
        }
        else if (targetCameraPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            this.transform.position += targetCameraPosition * speed * Time.deltaTime;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            this.transform.position += horizontalVelocity * Time.deltaTime;
        }
        targetCameraPosition = Vector3.zero;
    }

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        if (IsPauseState())
            return;

        float inputValue = obj.ReadValue<float>();
        if (Mathf.Approximately(inputValue, 0f))
            return;

        float scrollDelta = Mathf.Abs(inputValue) <= 1f ? inputValue : inputValue / 120f;
        float delta = scrollDelta * stepSize;
        delta = Mathf.Clamp(delta, -stepSize, stepSize);

        if (camTransposer == null)
            return;

        Vector3 offset = camTransposer.m_FollowOffset;
        float currentMagnitude = offset.magnitude;
        if (currentMagnitude < 0.01f)
            return;

        float newMagnitude = Mathf.Clamp(currentMagnitude - delta, minZoomDistance, maxZoomDistance);
        if (Mathf.Approximately(newMagnitude, currentMagnitude))
            return;

        targetFollowOffset = offset / currentMagnitude * newMagnitude;
    }

    public void UpdateCameraPosition()
    {
        if (virtualCam == null)
            return;

        camTransform.LookAt(transform.position);

        if (camTransposer == null)
            return;

        Vector3 lerpedOffset = Vector3.Lerp(
            camTransposer.m_FollowOffset,
            targetFollowOffset,
            Time.deltaTime * zoomDampening
        );

        float rawMagnitude = lerpedOffset.magnitude;
        if (rawMagnitude < 0.01f)
            return;

        float clampedMagnitude = Mathf.Clamp(rawMagnitude, minZoomDistance, maxZoomDistance);
        Vector3 clampedOffset = lerpedOffset / rawMagnitude * clampedMagnitude;

        camTransposer.m_FollowOffset = clampedOffset;
        targetFollowOffset = clampedOffset;
    }

    private void RotateCamera()
    {
        if (virtualCam == null || !Mouse.current.middleButton.isPressed || IsPauseState())
            return;

        Vector2 vector2 = Mouse.current.delta.ReadValue();
        float inputValueX = vector2.x;
        float inputValueY = vector2.y;
        float rotateX = inputValueY * maxRotationSpeed + this.transform.eulerAngles.x;
        if (inputValueY < 0 && rotateX > 180 && rotateX < 280)
            rotateX = 280;
        if (rotateX > 10 && rotateX < 180)
        {
            float distWithTop = Math.Abs(rotateX - 10);
            float distWithBot = Math.Abs(rotateX - 180);
            if (distWithTop < distWithBot) rotateX = 10;
            else rotateX = 280;
        }

        this.transform.rotation = Quaternion.Euler(rotateX, inputValueX * maxRotationSpeed + this.transform.eulerAngles.y, this.transform.eulerAngles.z);
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = camTransform.forward;
        forward.y = 0f;
        return forward;
    }

    private Vector3 GetCameraRight()
    {
        Vector3 right = camTransform.right;
        right.y = 0f;
        return right;
    }

    private async void SelectAction(CallbackContext ctx)
    {
        if (IsPauseState())
            return;

        if (recentSelectUnit == null)
        {
            if (lastSelectUnit)
                lastSelectUnit.OnDeselect();

            if (camPivot != null && virtualCam != null && (!virtualCam.Follow.Equals(camPivot) || !virtualCam.LookAt.Equals(camPivot)))
                lockTarget = null;

            lastSelectUnit = null;
        }
        else
        {
            if (lastSelectUnit && lastSelectUnit.Equals(recentSelectUnit))
                lockTarget = recentSelectUnit.gameObject;
            else
                lockTarget = null;

            lastSelectUnit = recentSelectUnit;
        }

        foreach (CombatUnitAgent unitEntity in playerUnitEntities)
        {
            if (unitEntity == null)
                continue;
            foreach (Skill skill in unitEntity.skillMap.SelectMany(skillList => skillList.Value).ToList())
            {
                if (skill == null || skill.isCoolingDown)
                    continue;

                if (skill.IsPreviewingRange() && skill.Model.targetType == (int)SkillModel.TargetType.assignable)
                {
                    if (recentSelectUnit != null)
                    {
                        if (!skill.SelectTarget(new List<GameObject> { recentSelectUnit.gameObject }))
                            return;
                        skill.Execute();
                        skill.CancelPreviewRange();
                        Cursor.visible = true;
                        Texture2D cursorTexture = await AssetsBundleManager.LoadTexture2D("Default_Cursor");
                        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    }
                    else
                    {
                        skill.CancelPreviewRange();
                        Cursor.visible = true;
                        Texture2D cursorTexture = await AssetsBundleManager.LoadTexture2D("Default_Cursor");
                        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    }
                }
                else if (skill.IsPreviewingRange() && skill.Model.targetType == (int)SkillModel.TargetType.direction)
                {
                    Cursor.visible = true;
                    skill.Execute();
                    skill.CancelPreviewRange();
                }
            }
        }
    }

    private void FocusPlayerShip(CallbackContext ctx)
    {
        if (lockTarget == null)
            lockTarget = playerUnitEntities.FirstOrDefault()?.gameObject;
        else
            lockTarget = null;
    }

    public void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = this.transform.position;
    }

    private void UnitAction(CallbackContext ctx)
    {
        if (IsPauseState())
            return;

        RaycastHit hit = CursorManager.singleton.GetFirstHitFilterLayer(LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            lastSelectUnit = null;
            return;
        }

        if (recentHoverUnit != null && lastSelectUnit != null)
        {
            if (recentHoverUnit.team == lastSelectUnit.team || recentHoverUnit.Equals(lastSelectUnit) || !recentHoverUnit.isAlive)
                return;
        }
    }

    /// <summary>
    /// Loads a level from LevelModel by build-index scene.
    /// Override/extend in a project subclass if you need Addressables or custom spawn flow.
    /// </summary>
    public bool LoadLevel(string levelKey)
    {
        if (string.IsNullOrEmpty(levelKey) || !LevelModel.map.TryGetValue(levelKey, out LevelModel levelModel))
        {
            Debug.LogError($"[RTS] LoadLevel failed: unknown level key '{levelKey}'");
            SetErrorCode("LEVEL_NOT_FOUND");
            return false;
        }

        if (levelModel.sceneIndex < 0 || levelModel.sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"[RTS] LoadLevel failed: sceneIndex {levelModel.sceneIndex} is out of build settings range.");
            SetErrorCode("LEVEL_SCENE_INVALID");
            return false;
        }

        GameStateController.singleton?.SetPaused(false);
        UpdateStartBattleTime();
        HookEssentialInputAction();
        HookBattleInputAction();
        PlayerPrefs.SetString("LastLevelKey", levelKey);
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelModel.sceneIndex);
        return true;
    }

    /// <summary>
    /// Fallback when no LevelModel exists: load first enabled build-settings scene (or active scene index 0).
    /// </summary>
    public bool LoadDefaultScene()
    {
        if (SceneManager.sceneCountInBuildSettings <= 0)
            return false;

        GameStateController.singleton?.SetPaused(false);
        UpdateStartBattleTime();
        HookEssentialInputAction();
        HookBattleInputAction();
        SceneManager.LoadScene(0);
        return true;
    }

    public void HookEssentialInputAction()
    {
        if (InputManager.singleton == null)
            return;
        InputManager.singleton.AddBackAction(OnPressESC, true);
    }

    public void UnhookEssentialInputAction()
    {
        if (InputManager.singleton == null)
            return;
        InputManager.singleton.RemoveBackPermanentAction(OnPressESC);
    }

    public void HookBattleInputAction()
    {
        if (InputManager.singleton == null)
            return;

        if (!InputManager.singleton.playerControl.UI.Click.enabled)
            InputManager.singleton.playerControl.UI.Click.Enable();
        if (!InputManager.singleton.playerControl.UI.ScrollWheel.enabled)
            InputManager.singleton.playerControl.UI.ScrollWheel.Enable();
        if (!InputManager.singleton.playerControl.UI.Point.enabled)
            InputManager.singleton.playerControl.UI.Point.Enable();
        if (!InputManager.singleton.playerControl.UI.MiddleClick.enabled)
            InputManager.singleton.playerControl.UI.MiddleClick.Enable();
        if (!InputManager.singleton.playerControl.Player.Move.enabled)
            InputManager.singleton.playerControl.Player.Move.Enable();
        if (!InputManager.singleton.playerControl.UI.RightClick.enabled)
            InputManager.singleton.playerControl.UI.RightClick.Enable();
        if (!InputManager.singleton.playerControl.Player.SpaceBar.enabled)
            InputManager.singleton.playerControl.Player.SpaceBar.Enable();
        if (!InputManager.singleton.playerControl.Player.Skill_1.enabled)
            InputManager.singleton.playerControl.Player.Skill_1.Enable();
        if (!InputManager.singleton.playerControl.Player.Skill_2.enabled)
            InputManager.singleton.playerControl.Player.Skill_2.Enable();
        if (!InputManager.singleton.playerControl.Player.Skill_3.enabled)
            InputManager.singleton.playerControl.Player.Skill_3.Enable();
        if (!InputManager.singleton.playerControl.Player.Skill_4.enabled)
            InputManager.singleton.playerControl.Player.Skill_4.Enable();

        InputManager.singleton.playerControl.Player.SpaceBar.performed += FocusPlayerShip;
        InputManager.singleton.playerControl.UI.RightClick.performed += UnitAction;
        InputManager.singleton.playerControl.UI.Click.performed += SelectAction;
        InputManager.singleton.playerControl.UI.ScrollWheel.performed += ZoomCamera;
    }

    public void UnhookBattleInputAction()
    {
        if (InputManager.singleton == null)
            return;

        if (InputManager.singleton.playerControl.UI.ScrollWheel.enabled)
            InputManager.singleton.playerControl.UI.ScrollWheel.Disable();
        if (InputManager.singleton.playerControl.UI.MiddleClick.enabled)
            InputManager.singleton.playerControl.UI.MiddleClick.Disable();
        if (InputManager.singleton.playerControl.Player.Move.enabled)
            InputManager.singleton.playerControl.Player.Move.Disable();
        if (InputManager.singleton.playerControl.UI.RightClick.enabled)
            InputManager.singleton.playerControl.UI.RightClick.Disable();
        if (InputManager.singleton.playerControl.Player.SpaceBar.enabled)
            InputManager.singleton.playerControl.Player.SpaceBar.Disable();
        if (InputManager.singleton.playerControl.Player.Skill_1.enabled)
            InputManager.singleton.playerControl.Player.Skill_1.Disable();
        if (InputManager.singleton.playerControl.Player.Skill_2.enabled)
            InputManager.singleton.playerControl.Player.Skill_2.Disable();
        if (InputManager.singleton.playerControl.Player.Skill_3.enabled)
            InputManager.singleton.playerControl.Player.Skill_3.Disable();
        if (InputManager.singleton.playerControl.Player.Skill_4.enabled)
            InputManager.singleton.playerControl.Player.Skill_4.Disable();

        InputManager.singleton.playerControl.Player.SpaceBar.performed -= FocusPlayerShip;
        InputManager.singleton.playerControl.UI.RightClick.performed -= UnitAction;
        InputManager.singleton.playerControl.UI.Click.performed -= SelectAction;
        InputManager.singleton.playerControl.UI.ScrollWheel.performed -= ZoomCamera;
    }

    private void OnPressESC()
    {
        if (playerUnitEntities.Any(x => x.skillMap.SelectMany(skillList => skillList.Value).ToList().Any(y => y.IsPreviewingRange())))
        {
            foreach (CombatUnitAgent unitEntity in playerUnitEntities)
            {
                foreach (Skill skill in unitEntity.skillMap.SelectMany(skillList => skillList.Value).ToList())
                {
                    if (skill.IsPreviewingRange())
                        skill.CancelPreviewRange();
                }
            }
        }
        else
        {
            GameStateController.singleton.BattlePause();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!IsPauseState() && virtualCam != null)
        {
            CheckMouseAtScreenEdge();
            UpdateBasePosition();
            UpdateCameraPosition();
            UpdateVelocity();
            RotateCamera();
        }
    }
}
