# Setup — AR Mechatronic Cell

Guia de configuração manual no Unity Editor após a instalação dos pacotes.

---

## 1. Abrir o projeto

1. Abra o Unity Hub.
2. Abra o projeto `My project` (Unity 6000.6.4f1).
3. Aguarde o Package Manager resolver e baixar os novos pacotes:
   - `com.unity.xr.arfoundation 6.0.3`
   - `com.unity.xr.arcore 6.0.3`
   - `com.unity.xr.management 4.5.0`

> Se houver erro de versão, abra **Window > Package Manager**, pesquise cada pacote e instale a versão mais recente da série 6.x.

---

## 2. Configurar XR Plugin Management

1. Abra **Edit > Project Settings > XR Plug-in Management**.
2. Clique em **Install XR Plug-in Management** se solicitado.
3. Na aba **Android**, marque: `ARCore`.
4. Feche as configurações.

---

## 3. Configurar a cena AR

Abra `Assets/Scenes/SampleScene.unity` e configure:

### 3.1 Limpar a cena padrão

Remova os objetos do template:
- `Main Camera` (será substituída pela AR Camera)
- `Directional Light` (opcional — pode manter)

### 3.2 Criar o AR Session

1. **GameObject > XR > AR Session**
2. Isso cria o objeto `AR Session` com o componente `ARSession`.

### 3.3 Criar o XR Origin

1. **GameObject > XR > XR Origin (Mobile AR)**
2. Isso cria a hierarquia:
   ```
   XR Origin
   └── Camera Offset
       └── Main Camera  (com AR Camera e AR Camera Background)
   ```
3. Certifique-se de que `Main Camera` tem a tag `MainCamera`.

### 3.4 Adicionar AR Plane Manager e AR Raycast Manager

No objeto `XR Origin`, adicione via **Add Component**:
- `AR Plane Manager`
  - **Plane Prefab**: atribuir um prefab de plano (ou usar o default do AR Foundation)
  - **Detection Mode**: `Horizontal`
- `AR Raycast Manager`

---

## 4. Criar o Prefab da Célula Mecatrônica

### 4.1 Estrutura de GameObjects

Crie a seguinte hierarquia na cena com primitivas:

```
MechatronicCell (GameObject vazio — raiz)
├── Robot          (Cubo 0.8×1.6×0.8, posição 0,0.8,0)
│   └── Arm        (Cilindro 0.15×0.6×0.15, posição 0,0.3,0.5)
├── Conveyor       (Cubo 2.0×0.1×0.5, posição 1.2,0.05,0)
├── AssemblyStation (Cubo 0.8×0.9×0.8, posição -1.2,0.45,0)
├── ElectricalPanel (Cubo 0.6×1.8×0.2, posição -2.0,0.9,0)
├── PneumaticActuator (Cilindro 0.15×0.5×0.15, posição 0.5,0.25,-0.5)
└── SafetyFence    (4x Cubos finos formando perímetro)
```

**Escala:** 1 unidade = 1 metro. As dimensões acima são em metros.

### 4.2 Adicionar componente CellComponent

Para cada filho principal (Robot, Conveyor, etc.), adicione **Add Component > CellComponent** e preencha:

| GameObject         | componentName          | function                              | partCode | maintenanceNote                          |
|--------------------|------------------------|---------------------------------------|----------|------------------------------------------|
| Robot              | Robô Manipulador 6 eixos | Manipulação de peças na célula       | ROB-001  | Verificar lubrificação dos eixos e cabos |
| Conveyor           | Esteira Transportadora | Transporte de peças entre estações    | EST-001  | Verificar tensão e alinhamento da correia |
| AssemblyStation    | Estação de Montagem    | Montagem e inspeção de subconjuntos   | ASS-001  | Inspecionar garras e sensores de presença |
| ElectricalPanel    | Painel Elétrico        | Controle e acionamento dos equipamentos | ELE-001 | Verificar disjuntores e conexões         |
| PneumaticActuator  | Atuador Pneumático     | Acionamento de prensas e fixadores    | PNE-001  | Verificar vedações e pressão do circuito |
| SafetyFence        | Perímetro de Segurança | Delimitação da área de risco          | SEG-001  | Verificar integridade das barreiras      |

### 4.3 Adicionar Colliders

Cada componente precisa de um **Collider** para o raycast de seleção:
- Select each child → **Add Component > Box Collider** (ou Capsule Collider para cilindros)
- Ajustar o collider para cobrir o objeto visualmente.

### 4.4 Criar materiais

Crie materiais URP Lit em `Assets/Materials/` para colorir os componentes:

| Material        | Cor Base (HDR)              |
|-----------------|-----------------------------|
| Mat_Robot       | Cinza escuro (0.3, 0.3, 0.3)|
| Mat_Conveyor    | Cinza médio (0.5, 0.5, 0.5) |
| Mat_Assembly    | Cinza claro (0.7, 0.7, 0.7) |
| Mat_Panel       | Azul escuro (0.1, 0.2, 0.4) |
| Mat_Actuator    | Preto (0.1, 0.1, 0.1)       |
| Mat_Safety      | Amarelo (1.0, 0.8, 0.0)     |

### 4.5 Salvar como Prefab

1. Arraste `MechatronicCell` para `Assets/Prefabs/`.
2. Remova o objeto da cena (será instanciado via AR).

---

## 5. Configurar ARPlacementController

No objeto `XR Origin`, adicione **Add Component > ARPlacementController** e preencha:

| Campo               | Valor                            |
|---------------------|----------------------------------|
| Raycast Manager     | (auto — GetComponent)            |
| Plane Manager       | (auto — GetComponent)            |
| Cell Prefab         | `Assets/Prefabs/MechatronicCell` |
| UI Controller       | (objeto com UIController)        |
| Component Selector  | (objeto com ComponentSelector)   |
| Rotation Speed      | 1.0                              |

---

## 6. Configurar ComponentSelector

Crie um objeto vazio chamado `SelectionController` e adicione **ComponentSelector**:

| Campo         | Valor                           |
|---------------|---------------------------------|
| AR Camera     | `XR Origin/Camera Offset/Main Camera` |
| UI Controller | (objeto com UIController)       |

---

## 7. Criar a Interface (UI Canvas)

### 7.1 Criar Canvas

1. **GameObject > UI > Canvas**
2. Canvas Render Mode: `Screen Space — Overlay`
3. Adicione **Canvas Scaler**: Reference Resolution 1080×1920, Match Width or Height 0.5

### 7.2 Estrutura sugerida

```
Canvas
├── ScanningHint       (Text TMP: "Aponte para uma superficie plana")
├── InfoPanel          (Panel com fundo semi-transparente)
│   ├── ComponentName  (TMP_Text — nome do componente)
│   ├── ComponentFunc  (TMP_Text — função)
│   ├── PartCode       (TMP_Text — código)
│   └── BtnManutencao  (Button — chama OnMaintenanceButtonClicked)
├── MaintenancePanel   (Panel com ScrollView)
│   ├── MaintenanceText (TMP_Text — texto gerado por MaintenanceGuide)
│   └── BtnFechar      (Button — chama OnCloseMaintenanceClicked)
└── ResetButton        (Button — chama ARPlacementController.ResetCell)
```

### 7.3 Adicionar UIController

No Canvas, adicione **Add Component > UIController** e atribua todos os campos do Inspector.

### 7.4 Configurar os botões

- `BtnManutencao.onClick` → `UIController.OnMaintenanceButtonClicked()`
- `BtnFechar.onClick` → `UIController.OnCloseMaintenanceClicked()`
- `ResetButton.onClick` → `ARPlacementController.ResetCell()`

---

## 8. Executar Testes

1. **Window > General > Test Runner**
2. Aba **EditMode**
3. Clique **Run All**
4. Todos os 12 testes devem passar (verde).

---

## 9. Configurações de Build Android

1. **File > Build Settings**
2. Plataforma: **Android** — clique **Switch Platform** se necessário.
3. **Player Settings**:
   - Bundle Identifier: `com.academic.armechatroniccell`
   - Minimum API Level: 26
   - Target Architecture: ARM64
   - Scripting Backend: IL2CPP
4. **XR Plug-in Management** (Android): ARCore ativado.
5. Clique **Build** e escolha a pasta de saída.

---

## 10. Gerar .unitypackage

1. **Assets > Export Package...**
2. Selecione:
   - `Assets/Scenes/`
   - `Assets/Scripts/`
   - `Assets/Tests/`
   - `Assets/Prefabs/`
   - `Assets/Materials/`
   - `Assets/Models/` (se houver)
3. Marque **Include dependencies**.
4. Clique **Export** e salve como `ARMechatronicCell.unitypackage`.

---

## Resolução de Problemas Comuns

| Problema | Solução |
|----------|---------|
| AR Foundation não resolve | Window > Package Manager > instalar versão 6.x manual |
| ARCore não disponível no dispositivo | Verificar se dispositivo suporta ARCore (Android 7.0+ com ARCore instalado) |
| Testes não aparecem no Test Runner | Verificar se `Tests.EditMode.asmdef` está no projeto e `UNITY_INCLUDE_TESTS` está definido |
| Célula não aparece após toque | Verificar se ARRaycastManager e ARPlaneManager estão no XR Origin; verificar Cell Prefab atribuído |
| Componente não selecionável | Verificar se o componente tem `CellComponent` script + Collider + está no Layer padrão |
