using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Detecta toque/clique em componentes da celula e notifica o UIController.
/// Usa Physics.Raycast contra os colliders dos GameObjects com CellComponent.
/// </summary>
public class ComponentSelector : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private UIController uiController;
    [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0f, 1f);

    private CellComponent selected;

    void Update()
    {
        if (!TryGetTouchBegin(out Vector2 screenPos)) return;
        if (IsPointerOverUI(screenPos)) return;
        HandleRaycast(screenPos);
    }

    bool TryGetTouchBegin(out Vector2 screenPos)
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

    void HandleRaycast(Vector2 screenPos)
    {
        Camera cam = arCamera != null ? arCamera : Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            var component = hit.collider.GetComponentInParent<CellComponent>();
            if (component != null)
            {
                Select(component);
                return;
            }
        }
        Deselect();
    }

    void Select(CellComponent component)
    {
        if (selected == component) return;
        Deselect();
        selected = component;
        SetHighlight(component.gameObject, true);
        uiController?.ShowComponentInfo(component.data);
    }

    void Deselect()
    {
        if (selected == null) return;
        SetHighlight(selected.gameObject, false);
        selected = null;
        uiController?.HideComponentInfo();
    }

    void SetHighlight(GameObject target, bool on)
    {
        foreach (var r in target.GetComponentsInChildren<Renderer>())
        {
            if (on)
            {
                r.material.EnableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", highlightColor * 0.4f);
            }
            else
            {
                r.material.DisableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    public void ClearSelection() => Deselect();
    public CellComponent GetSelected() => selected;
}
