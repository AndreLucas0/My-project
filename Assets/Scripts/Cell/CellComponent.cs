using UnityEngine;

/// <summary>
/// Identifica um componente da celula mecatronica e armazena seus dados informativos.
/// Deve ser colocado no GameObject raiz de cada componente (Robo, Esteira, etc.).
/// </summary>
public class CellComponent : MonoBehaviour
{
    public CellComponentData data;
}
