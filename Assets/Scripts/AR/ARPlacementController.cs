using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Controla o posicionamento da celula mecatronica em AR.
/// Detecta planos horizontais e posiciona/move a celula por toque.
/// Rotacao com dois dedos. Deve ficar no mesmo objeto que ARRaycastManager e ARPlaneManager.
/// </summary>
public class ARPlacementController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private UIController uiController;
    [SerializeField] private ComponentSelector componentSelector;
    [SerializeField] private float rotationSpeed = 1.0f;

    private GameObject cellInstance;
    private readonly List<ARRaycastHit> hits = new();

    public GameObject CellInstance => cellInstance;
    public bool IsCellPlaced => cellInstance != null;

    void Awake()
    {
        if (raycastManager == null) raycastManager = GetComponent<ARRaycastManager>();
        if (planeManager == null) planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        HandleTwoFingerRotation();
        HandleSingleTouchPlacement();
    }

    void HandleSingleTouchPlacement()
    {
        if (!TryGetSingleTouchBegin(out Vector2 screenPos)) return;
        if (IsPointerOverUI(screenPos)) return;

        // Se um componente 3D foi tocado, nao move a celula
        Camera cam = Camera.main;
        if (cam != null && Physics.Raycast(cam.ScreenPointToRay(screenPos), out _, 50f))
            return;

        TryPlaceOrMove(screenPos);
    }

    void TryPlaceOrMove(Vector2 screenPos)
    {
        if (raycastManager == null) return;
        if (!raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
            return;

        Pose pose = hits[0].pose;

        if (cellInstance == null)
        {
            cellInstance = Instantiate(cellPrefab, pose.position, pose.rotation);
            HidePlanes();
            uiController?.OnCellPlaced();
        }
        else
        {
            cellInstance.transform.position = pose.position;
        }
    }

    void HandleTwoFingerRotation()
    {
        if (!IsCellPlaced || Input.touchCount != 2) return;

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began) return;

        Vector2 prevT0 = t0.position - t0.deltaPosition;
        Vector2 prevT1 = t1.position - t1.deltaPosition;

        float prevAngle = Mathf.Atan2(prevT1.y - prevT0.y, prevT1.x - prevT0.x) * Mathf.Rad2Deg;
        float currAngle = Mathf.Atan2(
            t1.position.y - t0.position.y,
            t1.position.x - t0.position.x) * Mathf.Rad2Deg;

        float delta = Mathf.DeltaAngle(prevAngle, currAngle);
        cellInstance.transform.Rotate(Vector3.up, -delta * rotationSpeed, Space.World);
    }

    public void ResetCell()
    {
        if (cellInstance != null)
        {
            Destroy(cellInstance);
            cellInstance = null;
        }

        ShowPlanes();
        componentSelector?.ClearSelection();
        uiController?.OnCellReset();
    }

    void HidePlanes()
    {
        if (planeManager == null) return;
        planeManager.enabled = false;
        foreach (ARPlane plane in planeManager.trackables)
            plane.gameObject.SetActive(false);
    }

    void ShowPlanes()
    {
        if (planeManager == null) return;
        planeManager.enabled = true;
        foreach (ARPlane plane in planeManager.trackables)
            plane.gameObject.SetActive(true);
    }

    bool TryGetSingleTouchBegin(out Vector2 screenPos)
    {
        screenPos = default;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }
#else
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            screenPos = Input.GetTouch(0).position;
            return true;
        }
#endif
        return false;
    }

    bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return false;
#endif
    }
}
