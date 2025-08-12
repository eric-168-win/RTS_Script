using System;
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
using UnityEngine.InputSystem.LowLevel;


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
        [SerializeField]
        [ColorUsage(showAlpha: true, hdr: true)]
        private Color errorTintColor = Color.red;
        [SerializeField]
        [ColorUsage(showAlpha: true, hdr: true)]
        private Color errorFresnelColor = new(4, 1.7f, 0, 2);
        [SerializeField]
        [ColorUsage(showAlpha: true, hdr: true)]
        private Color availableToPlaceTintColor = new(0.2f, 0.65f, 1, 2);
        [SerializeField]
        [ColorUsage(showAlpha: true, hdr: true)]
        private Color availableToPlaceFresnelColor = new(4, 1.7f, 0, 2);


        private Vector2 startingMousePosition;

        private BaseCommand activeCommand;
        private GameObject ghostInstance;
        private MeshRenderer ghostRenderer;
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
        private static readonly int TINT = Shader.PropertyToID("_Tint");
        private static readonly int FRESNEL = Shader.PropertyToID("_FresnelColor");


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
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            Bus<CommandSelectedEvent>.OnEvent += HandleCommandSelected;

        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            aliveUnits.Remove(evt.Unit);
            selectedUnits.Remove(evt.Unit);
        }


        private void HandleUnitSpawned(UnitSpawnEvent evt) => aliveUnits.Add(evt.Unit);
        private void HandleUnitSelected(UnitSelectedEvent evt)
        {
            if (!selectedUnits.Contains(evt.Unit))
            {
                selectedUnits.Add(evt.Unit);
            }

        }
        private void HandleUnitDeselected(UnitDeselectedEvent evt) => selectedUnits.Remove(evt.Unit);
        private void HandleCommandSelected(CommandSelectedEvent evt)
        {
            activeCommand = evt.Command;
            if (!activeCommand.RequiresClickToActivate)
            {
                ActivateCommand(new RaycastHit());
            }
            else if (activeCommand.GhostPrefab != null)
            {
                ghostInstance = Instantiate(activeCommand.GhostPrefab);
                ghostInstance.name = "moving_Instance";
                ghostRenderer = ghostInstance.GetComponentInChildren<MeshRenderer>();
            }
        }


        private void OnDestroy()
        {   //subscribe to events
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawned;
            Bus<CommandSelectedEvent>.OnEvent -= HandleCommandSelected;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;

        }

        void Update()
        {
            HandlePanning();
            HandleZooming();
            HandleRotation();
            HandleGhost();
            HandleRightClick();
            HandleDragSelectByLeft();

        }

        private void HandleGhost()
        {
            if (ghostInstance == null) return;

            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
                activeCommand = null;
                return;
            }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, floorLayers))
            {
                ghostInstance.transform.position = hit.point;
                bool allRestrictionsPass = activeCommand.AllRestrictionsPass(hit.point);

                ghostRenderer.material.SetColor(TINT, allRestrictionsPass ? availableToPlaceTintColor : errorTintColor);
                ghostRenderer.material.SetColor(FRESNEL,
                    allRestrictionsPass ? availableToPlaceFresnelColor : errorFresnelColor
                );

            }
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
                && activeCommand == null
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
            if (activeCommand != null || wasMouseDownOnUI)
            {
                return; // Ignore drag if an command is active or mouse was down on UI
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

        private List<BaseCommand> GetAvailableCommands(AbstractUnit unit)
        {
            OverrideCommandsCommand[] overrideCommandsCommands = unit.AvailableCommands
                .Where(command => command is OverrideCommandsCommand)
                .Cast<OverrideCommandsCommand>()
                .ToArray();

            List<BaseCommand> allAvailableCommands = new();

            foreach (OverrideCommandsCommand overrideCommand in overrideCommandsCommands)//hard to understand
            {
                allAvailableCommands.AddRange(overrideCommand.Commands
                    .Where(command => command is not OverrideCommandsCommand)
                );//show building Command has Reset Command !!!
            }
            //--------------------------------------------------------------------
            allAvailableCommands.AddRange(unit.AvailableCommands
                .Where(command => command is not OverrideCommandsCommand)
            );

            return allAvailableCommands;
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
                        CommandContext context = new(abstractUnits[i], hit, i, MouseButton.Right);
                        foreach (ICommand command in GetAvailableCommands(abstractUnits[i]))
                        {
                            if (command.CanHandle(context))
                            {
                                command.Handle(context);
                                if (command.IsSingleUnitCommand)
                                {
                                    return;
                                }
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

            if (activeCommand == null
                && Physics.Raycast(ray, out RaycastHit hit, float.MaxValue
                /* infinitely long */, /*LayerMask.GetMask("Default")*/ selectableUnitsLayers)
                && hit.collider.TryGetComponent(out ISelectable selectable))
            {
                selectable.Select();
            }
            else if (activeCommand != null
                && !EventSystem.current.IsPointerOverGameObject()
                && Physics.Raycast(ray, out hit, float.MaxValue, interactableUnitsLayers | floorLayers))
            {
                ActivateCommand(hit);
            }

        }

        private void ActivateCommand(RaycastHit hit)
        {
            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
            }

            List<AbstractCommandable> absCmdables = selectedUnits
                .Where((unit) => unit is AbstractCommandable)
                .Cast<AbstractCommandable>()
                .ToList();

            for (int i = 0; i < absCmdables.Count; i++)
            {
                CommandContext context = new(absCmdables[i], hit, i);
                Debug.Log($"Handling command: [{activeCommand.name}] for [{absCmdables[i].name}] at hit point: {hit.point}");
                if (activeCommand.CanHandle(context))
                {
                    Debug.Log($"Command [{activeCommand.name}] can handle context for [{absCmdables[i].name}]");
                    activeCommand.Handle(context);
                    if (activeCommand.IsSingleUnitCommand)
                    {
                        break;
                    }
                }

            }

            activeCommand = null; // Reset active command after handling
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

        // private void HandleZooming()
        // {
        //     float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        //     // if (scrollInput != 0f && mainCamera != null)
        //     // {
        //     //     if (mainCamera.orthographic)
        //     //     {
        //     //         // Handle orthographic camera zoom
        //     //         float newSize = mainCamera.orthographicSize - scrollInput * zoomSpeed;
        //     //         mainCamera.orthographicSize = Mathf.Clamp(newSize, minOrthoSize, maxOrthoSize);
        //     //     }
        //     //     else
        //     //     {
        //     //         // Handle perspective camera zoom
        //     //         float newFOV = mainCamera.fieldOfView - scrollInput * zoomSpeed;
        //     //         mainCamera.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
        //     //     }
        //     // }

        //     if (scrollInput != 0f) {
        //         cameraTarget.linearVelocity = new Vector3(0, 1, 0);
        //     }
        // }

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