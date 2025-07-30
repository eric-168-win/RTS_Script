using System.Collections.Generic;
using System.Linq;
using RTS_LEARN.Commands;
using RTS_LEARN.Event;
using RTS_LEARN.EventBus;
using RTS_LEARN.Units;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace RTS_LEARN.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private new Camera camera;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private LayerMask selectableUnitsLayers;
        [SerializeField] private LayerMask interactableUnitsLayers;
        [SerializeField] private LayerMask floorLayers;

        [SerializeField] private RectTransform selectionBox; // UI element for selection box


        private Vector2 startingMousePosition;

        private ActionBase activeAction;
        private bool wasMouseDownOnUI;
        private CinemachineFollow cinemachineFollow;
        private float zoomStartTime;
        private Vector3 startingFollowOffset;

        private float rotationStartTime;
        private float maxRotationAmount;

        private HashSet<AbstractUnit> aliveUnits = new(100);
        private HashSet<AbstractUnit> addedUnits = new(24);
        private List<ISelectable> selectedUnits = new(12);//keep the list Constant, don't want to resize it (resize is computationally expensive)

        [SerializeField] private SupplySO mineralsSO;
        [SerializeField] private SupplySO gasSO;
        private int minerals;
        private int gas;

        private void Awake()
        {

            // cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
            if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
            {
                Debug.LogError("CinemachineFollow component not found on CinemachineCamera.");
            }
            startingFollowOffset = cinemachineFollow.FollowOffset;

            maxRotationAmount = Mathf.Abs(cinemachineFollow.FollowOffset.z);

            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawned;
            // Bus<UnitRemoveEvent>.OnEvent += HandleUnitRemoved;
            Bus<ActionSelectedEvent>.OnEvent += HandleActionSelected;
            Bus<SupplyEvent>.OnEvent += (evt) =>
            {
                if (evt.Supply == mineralsSO)
                {
                    minerals += evt.Amount;
                }
                else if (evt.Supply == gasSO)
                {
                    gas += evt.Amount;
                }
                // Debug.Log($"Minerals: {minerals}, Gas: {gas}"); // For debugging purposes
            };
        }

        private void HandleUnitSpawned(UnitSpawnEvent evt) => aliveUnits.Add(evt.Unit);
        // private void HandleUnitRemoved(UnitRemoveEvent evt) => aliveUnits.Remove(evt.Unit);
        private void HandleUnitSelected(UnitSelectedEvent evt) => selectedUnits.Add(evt.Unit);
        private void HandleUnitDeselected(UnitDeselectedEvent evt) => selectedUnits.Remove(evt.Unit);
        private void HandleActionSelected(ActionSelectedEvent evt)
        {
            activeAction = evt.Action;
            if (!activeAction.RequiresClickToActivate)
            {
                ActivateAction(new RaycastHit());
            }
        }


        private void OnDestroy()
        {   //subscribe to events
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawned;
            // Bus<UnitRemoveEvent>.OnEvent -= HandleUnitRemoved;
            Bus<ActionSelectedEvent>.OnEvent -= HandleActionSelected;
        }

        void Update()
        {
            HandleRotation();
            HandleZooming();
            HandlePanning();
            HandleRightClick();
            HandleDragSelectByLeft();
        }


        private void HandleDragSelectByLeft()
        {
            if (selectionBox == null) { return; }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseDown();
            }
            else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasReleasedThisFrame)
            {
                HandleMouseDrag();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseUp()
        {
            if (!wasMouseDownOnUI
                && activeAction == null
                && !Keyboard.current.shiftKey.isPressed)
            {
                DeselectAllUnits();
            }
            HandleLeftClick();
            foreach (AbstractUnit unit in addedUnits)
            {
                unit.Select();
            }
            // Disable UI
            selectionBox.gameObject.SetActive(false);
        }

        private void HandleMouseDrag()
        {
            if (activeAction != null || wasMouseDownOnUI)
            {
                return; // Ignore drag if an action is active or mouse was down on UI
            }

            Bounds selectionBounds = ResizeSelectionBox();
            foreach (AbstractUnit unit in aliveUnits)
            {
                Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
                //WorldToScreenPoint converts world position to screen position
                if (selectionBounds.Contains(unitPosition))
                {
                    addedUnits.Add(unit);
                }
            }
        }

        private void HandleMouseDown()
        {
            // Enable the UI
            selectionBox.sizeDelta = Vector2.zero;
            startingMousePosition = Mouse.current.position.ReadValue();
            selectionBox.gameObject.SetActive(true);
            addedUnits.Clear();
            wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject();
        }

        private void DeselectAllUnits()
        {
            // Debug.Log("DeselectAllUnits called");
            ISelectable[] currentSelectedUnits = selectedUnits.ToArray();
            foreach (ISelectable selectable in currentSelectedUnits)
            {
                selectable.Deselect();
            }
            selectedUnits.Clear();
        }

        private Bounds ResizeSelectionBox()
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();

            float width = currentMousePosition.x - startingMousePosition.x;
            float height = currentMousePosition.y - startingMousePosition.y;
            Vector2 selectionBoxSize = currentMousePosition - startingMousePosition;

            selectionBox.anchoredPosition = startingMousePosition + selectionBoxSize / 2;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            return new Bounds(
                selectionBox.anchoredPosition,
                selectionBox.sizeDelta
            );
        }

        private void HandleRightClick()
        {
            if (selectedUnits.Count == 0) { return; }

            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, interactableUnitsLayers | floorLayers))
                {
                    List<AbstractUnit> abstractUnits = new(selectedUnits.Count);
                    foreach (ISelectable selectable in selectedUnits)
                    {
                        if (selectable is AbstractUnit unit)
                        {
                            abstractUnits.Add(unit);
                        }
                    }

                    for (int i = 0; i < abstractUnits.Count; i++)
                    {
                        CommandContext context = new(abstractUnits[i], hit, i);
                        foreach (ICommand command in abstractUnits[i].AvailableCommands)
                        {
                            if (command.CanHandle(context))
                            {
                                command.Handle(context);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void HandleLeftClick()
        {
            if (camera == null) { return; }
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (activeAction == null
                && Physics.Raycast(ray, out RaycastHit hit, float.MaxValue
                /* infinitely long */, /*LayerMask.GetMask("Default")*/ selectableUnitsLayers)
                && hit.collider.TryGetComponent(out ISelectable selectable))
            {
                selectable.Select();
            }
            else if (activeAction != null
                && !EventSystem.current.IsPointerOverGameObject()
                && Physics.Raycast(ray, out hit, float.MaxValue, interactableUnitsLayers | floorLayers))
            {
                ActivateAction(hit);
            }

        }

        private void ActivateAction(RaycastHit hit)
        {
            List<AbstractCommandable> absCmdables = selectedUnits
                .Where((unit) => unit is AbstractCommandable)
                .Cast<AbstractCommandable>()
                .ToList();

            for (int i = 0; i < absCmdables.Count; i++)
            {
                CommandContext context = new(absCmdables[i], hit, i);
                if (activeAction.CanHandle(context))
                {
                    activeAction.Handle(context);
                }
            }

            activeAction = null; // Reset active action after handling
        }

        private void HandlePanning()
        {
            return;
            Vector2 moveAmount = GetKeyboardMovement();
            moveAmount += GetMouseMovement();
            // cameraTarget(Transform) insead of using Rigidbody
            // moveAmount *= Time.deltaTime;
            // cameraTarget.position += new Vector3(moveAmount.x, 0, moveAmount.y);

            cameraTarget.linearVelocity = new Vector3(moveAmount.x, 0, moveAmount.y);
        }

        private Vector2 GetMouseMovement()
        {
            Vector2 moveAmount = Vector2.zero;

            if (cameraConfig.EnableEdgePan)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);

                if (mousePosition.x <= cameraConfig.EdgePanSize)
                {
                    moveAmount.x -= cameraConfig.MousePanSpeed;
                }
                else if (mousePosition.x > screenSize.x - cameraConfig.EdgePanSize)
                {
                    moveAmount.x += cameraConfig.MousePanSpeed;
                }

                if (mousePosition.y <= cameraConfig.EdgePanSize)
                {
                    moveAmount.y -= cameraConfig.MousePanSpeed;
                }
                else if (mousePosition.y > screenSize.y - cameraConfig.EdgePanSize)
                {
                    moveAmount.y += cameraConfig.MousePanSpeed;
                }
            }

            return moveAmount;
        }

        private Vector2 GetKeyboardMovement()
        {
            Vector2 moveAmount = Vector2.zero;

            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveAmount.y += cameraConfig.KeyboardPanSpeed;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveAmount.x -= cameraConfig.KeyboardPanSpeed;
            }
            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveAmount.y -= cameraConfig.KeyboardPanSpeed;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveAmount.x += cameraConfig.KeyboardPanSpeed;
            }

            return moveAmount;
        }

        private void HandleZooming()
        {
            if (ShouldSetZoomStartTime())
            {
                zoomStartTime = Time.time;
                // Debug.Log("Zoom start time set: " + zoomStartTime);
            }


            float zoomTime = Mathf.Clamp01((Time.time - zoomStartTime) * cameraConfig.ZoomSpeed);
            Vector3 targetFollowOffset;

            if (Keyboard.current.endKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    cameraConfig.MinZoomDistance,
                    cinemachineFollow.FollowOffset.z
                );

                // cinemachineFollow.FollowOffset = Vector3.Slerp(
                //     startingFollowOffset,
                //     targetFollowOffset,
                //     zoomTime
                // );
            }
            else
            {
                // cinemachineFollow.FollowOffset = Vector3.Slerp(
                //     targetFollowOffset,
                //     startingFollowOffset,
                //     zoomTime
                // );

                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    startingFollowOffset.y,
                    cinemachineFollow.FollowOffset.z
                );
            }

            cinemachineFollow.FollowOffset = Vector3.Slerp(
                   cinemachineFollow.FollowOffset,
                   targetFollowOffset,
                   zoomTime
            );
        }

        private bool ShouldSetZoomStartTime()
        {
            return Keyboard.current.endKey.wasPressedThisFrame
            || Keyboard.current.endKey.wasReleasedThisFrame;
        }

        private bool ShouldSetRotationStartTime()
        {
            return Keyboard.current.pageUpKey.wasPressedThisFrame
            || Keyboard.current.pageDownKey.wasPressedThisFrame
            || Keyboard.current.pageUpKey.wasReleasedThisFrame
            || Keyboard.current.pageDownKey.wasReleasedThisFrame
            ;
        }

        private void HandleRotation()
        {
            if (ShouldSetRotationStartTime())
            {
                rotationStartTime = Time.time;
                // Debug.Log("Rotation start time set: " + rotationStartTime);
            }

            float rotationAmount = Mathf.Clamp01((Time.time - rotationStartTime) * cameraConfig.RotationSpeed);

            Vector3 targetFollowOffset;

            if (Keyboard.current.pageDownKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    maxRotationAmount, cinemachineFollow.FollowOffset.y, 0);
            }
            else if (Keyboard.current.pageUpKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    -maxRotationAmount, cinemachineFollow.FollowOffset.y, 0);

            }
            else
            {
                targetFollowOffset = new Vector3(
                    startingFollowOffset.x,
                    cinemachineFollow.FollowOffset.y,
                    startingFollowOffset.z
                );
            }

            cinemachineFollow.FollowOffset = Vector3.Slerp(
                 cinemachineFollow.FollowOffset,
                 targetFollowOffset,
                 rotationAmount
             );

        }
    }
}