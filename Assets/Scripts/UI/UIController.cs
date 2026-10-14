using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla todos os paineis de UI do protótipo AR.
/// Recebe eventos de ARPlacementController e ComponentSelector.
/// Atribuir referencias no Inspector.
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("Painel de Informacoes do Componente")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text componentNameText;
    [SerializeField] private TMP_Text componentFunctionText;
    [SerializeField] private TMP_Text partCodeText;

    [Header("Painel de Manutencao")]
    [SerializeField] private GameObject maintenancePanel;
    [SerializeField] private TMP_Text maintenanceText;
    [SerializeField] private Button maintenanceButton;

    [Header("Dica de Escaneamento")]
    [SerializeField] private GameObject scanningHint;

    [Header("Controles")]
    [SerializeField] private GameObject resetButton;

    private CellComponentData currentComponent;

    void Start()
    {
        HideAll();
        SetScanningHint(true);
    }

    public void ShowComponentInfo(CellComponentData data)
    {
        currentComponent = data;
        if (infoPanel == null) return;

        infoPanel.SetActive(true);
        maintenancePanel?.SetActive(false);

        if (componentNameText)  componentNameText.text  = data.componentName;
        if (componentFunctionText) componentFunctionText.text = $"Funcao: {data.function}";
        if (partCodeText)       partCodeText.text       = $"Codigo: {data.partCode}";

        if (maintenanceButton) maintenanceButton.interactable = true;
    }

    public void HideComponentInfo()
    {
        infoPanel?.SetActive(false);
        maintenancePanel?.SetActive(false);
        currentComponent = null;
        if (maintenanceButton) maintenanceButton.interactable = false;
    }

    // Chamado pelo botao Manutencao no Inspector
    public void OnMaintenanceButtonClicked()
    {
        if (currentComponent == null) return;

        bool visible = maintenancePanel != null && maintenancePanel.activeSelf;
        if (visible)
        {
            maintenancePanel.SetActive(false);
            return;
        }

        if (maintenancePanel != null)
        {
            maintenancePanel.SetActive(true);
            if (maintenanceText)
                maintenanceText.text = MaintenanceGuide.BuildGuideText(currentComponent);
        }
    }

    // Chamado pelo botao Fechar do painel de manutencao
    public void OnCloseMaintenanceClicked()
    {
        maintenancePanel?.SetActive(false);
    }

    public void OnCellPlaced()
    {
        SetScanningHint(false);
        resetButton?.SetActive(true);
    }

    public void OnCellReset()
    {
        SetScanningHint(true);
        resetButton?.SetActive(false);
        HideAll();
    }

    void HideAll()
    {
        infoPanel?.SetActive(false);
        maintenancePanel?.SetActive(false);
        if (maintenanceButton) maintenanceButton.interactable = false;
    }

    void SetScanningHint(bool show)
    {
        scanningHint?.SetActive(show);
    }
}
