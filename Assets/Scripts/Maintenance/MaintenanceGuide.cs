/// <summary>
/// Gera o texto do guia de manutencao para um componente da celula.
/// Classe estatica para facilitar testes sem dependencia de MonoBehaviour.
/// </summary>
public static class MaintenanceGuide
{
    public static string BuildGuideText(CellComponentData data)
    {
        if (data == null)
            return "Nenhum componente selecionado.";

        return "MANUTENCAO — SIMULACAO EDUCACIONAL\n\n" +
               $"Componente: {data.componentName}\n" +
               $"Codigo: {data.partCode}\n\n" +
               "Passos:\n" +
               "1. Sinalizar a area e isolar o equipamento (LOTO).\n" +
               $"2. Identificar o componente: {data.componentName}\n" +
               "3. Verificar o estado visual do componente.\n" +
               $"4. {data.maintenanceNote}\n" +
               "5. Registrar a intervencao no sistema.\n\n" +
               "AVISO: Esta e uma simulacao para fins educacionais.\n" +
               "Nao utilize como instrucao operacional real.";
    }
}
