using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Selection, Hovering
public class CursorManager : Singleton<CursorManager>
{
    [SerializeField]
    private Camera cam;

    private IHoverable _currentHoverable;
    public IHoverable CurrentHoverable { get { return _currentHoverable; } }
    public List<ISelectable> selectableList { get; private set; } = new List<ISelectable>();

    private ISelectable _currentSelectable;
    public ISelectable currentSelectable { get { return _currentSelectable; } }

    private List<RaycastHit> raycastHitList = new List<RaycastHit>();
    public RaycastHit GetLastHit { get {
            return raycastHitList.FirstOrDefault(); } 
    }
    public RaycastHit GetFirstHit { get { return raycastHitList.LastOrDefault(); } }

    public bool IsCursorOutBound { get; private set; }

    public event Action<ISelectable> onSelectableAdd;
    public event Action<ISelectable> onSelectableRemove;

    private void Start()
    {
        if(!InputManager.singleton.playerControl.UI.Click.enabled)
            InputManager.singleton.playerControl.UI.Click.Enable();

        InputManager.singleton.playerControl.UI.Click.performed += ctx => Selection();
    }

    private void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if(cam == null)
                return;
        }

        /*
        if (UIManager.singleton.IsAnyPanelExist())
        {
            if (_currentHoverable != null)
            {
                _currentHoverable.OnHoverOut();
                _currentHoverable = null;
            }

            if (_currentSelectable != null)
            {
                _currentSelectable.OnDeselect();
                _currentSelectable = null;
            }
            //return;
        }
        */

        Ray ray = cam.ScreenPointToRay(InputManager.singleton.playerControl.UI.Point.ReadValue<Vector2>());
        Debug.DrawRay(ray.origin, ray.direction*100, Color.cyan);
        raycastHitList = Physics.RaycastAll(ray, Mathf.Infinity).ToList();
        raycastHitList.Sort((x, y) => x.distance.CompareTo(y.distance));
        if (raycastHitList.Count == 0)
        {            
            if (_currentHoverable != null) _currentHoverable.OnHoverOut();
            return;
        }

        if (_currentHoverable != null) _currentHoverable.OnHoverOut();

        //Hoverable Guard
        if (!GetLastHit.transform.TryGetComponent(out _currentHoverable))
            return;

        _currentHoverable.OnHoverIn();
    }

    public RaycastHit GetFirstHitFilterLayer(LayerMask layerMask)
    {
        foreach (RaycastHit raycastHit in raycastHitList)
        {
            if ((layerMask.value & (1 << raycastHit.transform.gameObject.layer)) > 0)
                return raycastHit;
        }

        return default;
    }

    public void FPSMode()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void WindowMode()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void MultiSelection()
    {
        //Null Guard
        if (GetLastHit.transform == null)
            return;

        if (!GetLastHit.transform.TryGetComponent(out _currentSelectable))
        {
            //Remove Other Selection
            selectableList.ForEach(s =>
            {
                s.OnDeselect();
                onSelectableRemove?.Invoke(s);
            });
            selectableList.Clear();
            return;
        }

        //Whenever select a selected object, will be count as deselect target object
        if (selectableList.Contains(_currentSelectable))
        {
            _currentSelectable.OnDeselect();
            selectableList.Remove(_currentSelectable);
            onSelectableRemove?.Invoke(_currentSelectable);
            Debug.Log($"{_currentSelectable.GetGameObject().name} Deselected");
        }
        else
        {
            _currentSelectable.OnSelect();
            selectableList.Add(_currentSelectable);
            onSelectableAdd?.Invoke(_currentSelectable);
            Debug.Log($"{_currentSelectable.GetGameObject().name} Selected");
        }
    }

    private void Selection()
    {
        if (InputManager.singleton.isMultiMode)
            MultiSelection();
        else
        {
            //Remove Other Selection
            selectableList.ForEach(s =>
            {
                s.OnDeselect();
                onSelectableRemove?.Invoke(s);
            });
            selectableList.Clear();

            //Null Guard
            if (GetLastHit.transform == null)
                return;
            //Whenever selecting an object dont have ISelectable, will be count as deselect all
            if (!GetLastHit.transform.TryGetComponent(out _currentSelectable))
                return;

            _currentSelectable.OnSelect();
            selectableList.Add(_currentSelectable);
            onSelectableAdd?.Invoke(_currentSelectable);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(InputManager.singleton != null)
        {
            InputManager.singleton.playerControl.UI.Click.performed -= ctx => Selection();
        }        
        onSelectableAdd = null;
    }
}
