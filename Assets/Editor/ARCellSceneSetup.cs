using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public static class ARCellSceneSetup
{
    const string PrefabsPath   = "Assets/Prefabs";
    const string MaterialsPath = "Assets/Materials";

    [MenuItem("Tools/AR Cell/Setup Scene")]
    public static void SetupScene()
    {
        DestroyIfExists("Main Camera");
        EnsureFolders();

        var mats                         = CreateMaterials();
        var cellPrefab                   = CreateCellPrefab(mats);
        var (xrOriginGO, arCam)          = SetupARScene();
        var (canvasGO, uiCtrl, resetBtn) = CreateUI();
        var selector                     = CreateSelector(arCam, uiCtrl);

        ConfigurePlacementController(xrOriginGO, cellPrefab, uiCtrl, selector, resetBtn);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[ARCellSetup] Concluido. Salve a cena com Ctrl+S.");
    }

    // ─── UTILITIES ─────────────────────────────────────────────────────────

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(PrefabsPath))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(MaterialsPath))
            AssetDatabase.CreateFolder("Assets", "Materials");
    }

    static void DestroyIfExists(string name)
    {
        var go = GameObject.Find(name);
        if (go != null) Object.DestroyImmediate(go);
    }

    // ─── MATERIALS ─────────────────────────────────────────────────────────

    static Dictionary<string, Material> CreateMaterials()
    {
        var defs = new (string name, Color color)[]
        {
            ("Mat_Robot",    new Color(0.30f, 0.30f, 0.30f)),
            ("Mat_Conveyor", new Color(0.50f, 0.50f, 0.50f)),
            ("Mat_Assembly", new Color(0.70f, 0.70f, 0.70f)),
            ("Mat_Panel",    new Color(0.10f, 0.20f, 0.40f)),
            ("Mat_Actuator", new Color(0.10f, 0.10f, 0.10f)),
            ("Mat_Safety",   new Color(1.00f, 0.80f, 0.00f)),
        };

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        var result = new Dictionary<string, Material>();

        foreach (var (matName, color) in defs)
        {
            string path = $"{MaterialsPath}/{matName}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader) { name = matName };
                mat.SetColor("_BaseColor", color);
                AssetDatabase.CreateAsset(mat, path);
            }
            result[matName] = mat;
        }

        AssetDatabase.SaveAssets();
        return result;
    }

    // ─── CELL PREFAB ───────────────────────────────────────────────────────
    // Escala: 1 unidade = 1 metro.
    // Cilindro Unity: diametro 1u, altura 2u em scale(1,1,1) → scaleY = altura_m / 2

    static GameObject CreateCellPrefab(Dictionary<string, Material> mats)
    {
        var root = new GameObject("MechatronicCell");

        var robot = MakeCube("Robot", root,
            new Vector3(0f, 0.8f, 0f), new Vector3(0.8f, 1.6f, 0.8f), mats["Mat_Robot"]);
        MakeCylinder("Arm", robot,
            new Vector3(0f, 0.3f, 0.5f), new Vector3(0.15f, 0.30f, 0.15f), mats["Mat_Robot"]);

        var conveyor = MakeCube("Conveyor", root,
            new Vector3(1.2f, 0.05f, 0f), new Vector3(2.0f, 0.10f, 0.50f), mats["Mat_Conveyor"]);
        var assembly = MakeCube("AssemblyStation", root,
            new Vector3(-1.2f, 0.45f, 0f), new Vector3(0.8f, 0.90f, 0.80f), mats["Mat_Assembly"]);
        var panel = MakeCube("ElectricalPanel", root,
            new Vector3(-2.0f, 0.9f, 0f), new Vector3(0.6f, 1.80f, 0.20f), mats["Mat_Panel"]);
        var actuator = MakeCylinder("PneumaticActuator", root,
            new Vector3(0.5f, 0.25f, -0.5f), new Vector3(0.15f, 0.25f, 0.15f), mats["Mat_Actuator"]);

        var fence = new GameObject("SafetyFence");
        fence.transform.SetParent(root.transform, false);
        MakeCube("Fence_Front", fence, new Vector3(0f,  0.6f, -1.5f), new Vector3(6.10f, 1.2f, 0.05f), mats["Mat_Safety"]);
        MakeCube("Fence_Back",  fence, new Vector3(0f,  0.6f,  1.5f), new Vector3(6.10f, 1.2f, 0.05f), mats["Mat_Safety"]);
        MakeCube("Fence_Left",  fence, new Vector3(-3f, 0.6f,  0f),   new Vector3(0.05f, 1.2f, 3.10f), mats["Mat_Safety"]);
        MakeCube("Fence_Right", fence, new Vector3( 3f, 0.6f,  0f),   new Vector3(0.05f, 1.2f, 3.10f), mats["Mat_Safety"]);

        SetCellData(robot,    "Robo Manipulador 6 eixos",  "Manipulacao de pecas na celula",          "ROB-001", "Verificar lubrificacao dos eixos e cabos");
        SetCellData(conveyor, "Esteira Transportadora",    "Transporte de pecas entre estacoes",      "EST-001", "Verificar tensao e alinhamento da correia");
        SetCellData(assembly, "Estacao de Montagem",       "Montagem e inspecao de subconjuntos",     "ASS-001", "Inspecionar garras e sensores de presenca");
        SetCellData(panel,    "Painel Eletrico",           "Controle e acionamento dos equipamentos", "ELE-001", "Verificar disjuntores e conexoes");
        SetCellData(actuator, "Atuador Pneumatico",        "Acionamento de prensas e fixadores",      "PNE-001", "Verificar vedacoes e pressao do circuito");
        SetCellData(fence,    "Perimetro de Seguranca",    "Delimitacao da area de risco",            "SEG-001", "Verificar integridade das barreiras");

        string prefabPath = $"{PrefabsPath}/MechatronicCell.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.Refresh();

        Debug.Log($"[ARCellSetup] Prefab salvo em {prefabPath}");
        return prefab;
    }

    static GameObject MakeCube(string name, GameObject parent, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale    = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    static GameObject MakeCylinder(string name, GameObject parent, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale    = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    static void SetCellData(GameObject go, string compName, string func, string code, string note)
    {
        var cc = go.AddComponent<CellComponent>();
        cc.data = new CellComponentData
        {
            componentName   = compName,
            function        = func,
            partCode        = code,
            maintenanceNote = note,
        };
    }

    // ─── AR SCENE ──────────────────────────────────────────────────────────
    // Usa os menu items do Unity para criar AR Session e XR Origin com toda
    // a hierarquia correta (evita dependência direta de Unity.XR.CoreUtils).

    static (GameObject xrOriginGO, Camera arCamera) SetupARScene()
    {
        if (GameObject.Find("AR Session") == null)
            EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session");

        if (GameObject.Find("XR Origin") == null)
            EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (Mobile AR)");

        var xrOriginGO = GameObject.Find("XR Origin");
        var arCamera   = xrOriginGO != null ? xrOriginGO.GetComponentInChildren<Camera>() : null;

        if (xrOriginGO != null)
        {
            if (!xrOriginGO.GetComponent<ARPlaneManager>())   xrOriginGO.AddComponent<ARPlaneManager>();
            if (!xrOriginGO.GetComponent<ARRaycastManager>()) xrOriginGO.AddComponent<ARRaycastManager>();
        }

        return (xrOriginGO, arCamera);
    }

    // ─── UI CANVAS ─────────────────────────────────────────────────────────

    static (GameObject canvasGO, UIController uiCtrl, GameObject resetBtnGO) CreateUI()
    {
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        var hintGO = MakeTMP("ScanningHint", canvasGO,
            new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 200), new Vector2(-40, 70),
            "Aponte para uma superficie plana", 36, Color.white);

        var infoPanel = MakePanel("InfoPanel", canvasGO,
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0),
            Vector2.zero, new Vector2(0, 300));
        infoPanel.SetActive(false);

        var nameText = MakeTMP("ComponentName", infoPanel,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -40), new Vector2(-20, 55), "Nome", 32, Color.white);
        var funcText = MakeTMP("ComponentFunc", infoPanel,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -105), new Vector2(-20, 48), "Funcao", 24, new Color(0.8f, 0.8f, 0.8f));
        var codeText = MakeTMP("PartCode", infoPanel,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -163), new Vector2(-20, 42), "Codigo", 22, new Color(0.7f, 0.7f, 0.7f));

        var btnMaint = MakeButton("BtnManutencao", infoPanel,
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0),
            new Vector2(0, 65), new Vector2(-40, 55),
            "Guia de Manutencao", new Color(0.2f, 0.5f, 0.9f), Color.white);

        var maintPanel = MakePanel("MaintenancePanel", canvasGO,
            new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f), new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero);
        maintPanel.SetActive(false);

        var maintText = MakeTMP("MaintenanceText", maintPanel,
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f),
            new Vector2(0, -30), new Vector2(-20, -70), "", 20, Color.white);
        var tmpComp = maintText.GetComponent<TextMeshProUGUI>();
        tmpComp.alignment          = TextAlignmentOptions.TopLeft;
        tmpComp.textWrappingMode = TextWrappingModes.Normal;

        var btnClose = MakeButton("BtnFechar", maintPanel,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -25), new Vector2(-40, 44),
            "Fechar", new Color(0.7f, 0.2f, 0.2f), Color.white);

        var resetBtnGO = MakeButton("ResetButton", canvasGO,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-90, -60), new Vector2(160, 52),
            "Resetar Celula", new Color(0.8f, 0.3f, 0.1f), Color.white);
        resetBtnGO.SetActive(false);

        var uiCtrl = canvasGO.AddComponent<UIController>();
        var soUI   = new SerializedObject(uiCtrl);
        soUI.FindProperty("infoPanel").objectReferenceValue             = infoPanel;
        soUI.FindProperty("componentNameText").objectReferenceValue     = nameText.GetComponent<TextMeshProUGUI>();
        soUI.FindProperty("componentFunctionText").objectReferenceValue = funcText.GetComponent<TextMeshProUGUI>();
        soUI.FindProperty("partCodeText").objectReferenceValue          = codeText.GetComponent<TextMeshProUGUI>();
        soUI.FindProperty("maintenancePanel").objectReferenceValue      = maintPanel;
        soUI.FindProperty("maintenanceText").objectReferenceValue       = maintText.GetComponent<TextMeshProUGUI>();
        soUI.FindProperty("maintenanceButton").objectReferenceValue     = btnMaint.GetComponent<Button>();
        soUI.FindProperty("scanningHint").objectReferenceValue          = hintGO;
        soUI.FindProperty("resetButton").objectReferenceValue           = resetBtnGO;
        soUI.ApplyModifiedProperties();

        UnityEventTools.AddPersistentListener(
            btnMaint.GetComponent<Button>().onClick,
            new UnityAction(uiCtrl.OnMaintenanceButtonClicked));
        UnityEventTools.AddPersistentListener(
            btnClose.GetComponent<Button>().onClick,
            new UnityAction(uiCtrl.OnCloseMaintenanceClicked));

        return (canvasGO, uiCtrl, resetBtnGO);
    }

    static GameObject MakePanel(string name, GameObject parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;
        go.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.78f);
        return go;
    }

    static GameObject MakeTMP(string name, GameObject parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta,
        string text, float fontSize, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject MakeButton(string name, GameObject parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta,
        string label, Color bgColor, Color textColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;
        go.AddComponent<Image>().color = bgColor;
        go.AddComponent<Button>();

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var lrt = labelGO.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.sizeDelta = Vector2.zero;
        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 22;
        tmp.color     = textColor;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    // ─── COMPONENT SELECTOR ────────────────────────────────────────────────

    static ComponentSelector CreateSelector(Camera arCamera, UIController uiCtrl)
    {
        var go       = new GameObject("SelectionController");
        var selector = go.AddComponent<ComponentSelector>();
        var so       = new SerializedObject(selector);
        so.FindProperty("arCamera").objectReferenceValue     = arCamera;
        so.FindProperty("uiController").objectReferenceValue = uiCtrl;
        so.ApplyModifiedProperties();
        return selector;
    }

    // ─── AR PLACEMENT CONTROLLER ───────────────────────────────────────────

    static void ConfigurePlacementController(
        GameObject xrOriginGO, GameObject cellPrefab,
        UIController uiCtrl, ComponentSelector selector,
        GameObject resetBtnGO)
    {
        var ctrl = xrOriginGO.GetComponent<ARPlacementController>()
                   ?? xrOriginGO.AddComponent<ARPlacementController>();

        var so = new SerializedObject(ctrl);
        so.FindProperty("raycastManager").objectReferenceValue    = xrOriginGO.GetComponent<ARRaycastManager>();
        so.FindProperty("planeManager").objectReferenceValue      = xrOriginGO.GetComponent<ARPlaneManager>();
        so.FindProperty("cellPrefab").objectReferenceValue        = cellPrefab;
        so.FindProperty("uiController").objectReferenceValue      = uiCtrl;
        so.FindProperty("componentSelector").objectReferenceValue = selector;
        so.ApplyModifiedProperties();

        UnityEventTools.AddPersistentListener(
            resetBtnGO.GetComponent<Button>().onClick,
            new UnityAction(ctrl.ResetCell));
    }
}
