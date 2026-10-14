using NUnit.Framework;

public class MaintenanceGuideTests
{
    [Test]
    public void BuildGuideText_WithValidData_ContainsComponentName()
    {
        var data = new CellComponentData
        {
            componentName = "Robo Manipulador",
            partCode = "ROB-001",
            maintenanceNote = "Verificar lubrificacao"
        };

        string guide = MaintenanceGuide.BuildGuideText(data);

        Assert.IsTrue(guide.Contains("Robo Manipulador"),
            "O guia deve conter o nome do componente.");
    }

    [Test]
    public void BuildGuideText_WithValidData_ContainsPartCode()
    {
        var data = new CellComponentData
        {
            componentName = "Painel Eletrico",
            partCode = "ELE-001",
            maintenanceNote = "Verificar disjuntores"
        };

        string guide = MaintenanceGuide.BuildGuideText(data);

        Assert.IsTrue(guide.Contains("ELE-001"),
            "O guia deve conter o codigo da peca.");
    }

    [Test]
    public void BuildGuideText_WithValidData_ContainsEducationalWarning()
    {
        var data = new CellComponentData
        {
            componentName = "Esteira",
            partCode = "EST-001",
            maintenanceNote = "Verificar tensao da correia"
        };

        string guide = MaintenanceGuide.BuildGuideText(data);

        Assert.IsTrue(guide.Contains("SIMULACAO EDUCACIONAL"),
            "O guia deve conter aviso de simulacao educacional.");
    }

    [Test]
    public void BuildGuideText_WithValidData_ContainsMaintenanceNote()
    {
        var data = new CellComponentData
        {
            componentName = "Atuador Pneumatico",
            partCode = "PNE-001",
            maintenanceNote = "Verificar vedacoes e pressao"
        };

        string guide = MaintenanceGuide.BuildGuideText(data);

        Assert.IsTrue(guide.Contains("Verificar vedacoes e pressao"),
            "O guia deve conter a nota de manutencao do componente.");
    }

    [Test]
    public void BuildGuideText_WithNullData_ReturnsNonEmptyFallback()
    {
        string guide = MaintenanceGuide.BuildGuideText(null);

        Assert.IsFalse(string.IsNullOrEmpty(guide),
            "O guia nao deve retornar vazio para dados nulos.");
    }

    [Test]
    public void BuildGuideText_AlwaysContainsLOTOReference()
    {
        var data = new CellComponentData { componentName = "Qualquer", partCode = "X-001" };

        string guide = MaintenanceGuide.BuildGuideText(data);

        Assert.IsTrue(guide.Contains("LOTO"),
            "O guia deve referenciar o procedimento LOTO.");
    }
}
