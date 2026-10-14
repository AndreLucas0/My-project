using NUnit.Framework;

public class CellComponentDataTests
{
    [Test]
    public void CellComponentData_StoresComponentName()
    {
        var data = new CellComponentData { componentName = "Robo Manipulador" };
        Assert.AreEqual("Robo Manipulador", data.componentName);
    }

    [Test]
    public void CellComponentData_StoresFunction()
    {
        var data = new CellComponentData { function = "Manipulacao de pecas na celula" };
        Assert.AreEqual("Manipulacao de pecas na celula", data.function);
    }

    [Test]
    public void CellComponentData_StoresPartCode()
    {
        var data = new CellComponentData { partCode = "ROB-001" };
        Assert.AreEqual("ROB-001", data.partCode);
    }

    [Test]
    public void CellComponentData_StoresMaintenanceNote()
    {
        var data = new CellComponentData { maintenanceNote = "Verificar lubrificacao dos eixos" };
        Assert.AreEqual("Verificar lubrificacao dos eixos", data.maintenanceNote);
    }

    [Test]
    public void CellComponentData_DefaultInstantiation_DoesNotThrow()
    {
        Assert.DoesNotThrow(() =>
        {
            var data = new CellComponentData();
            _ = data.componentName;
        });
    }

    [Test]
    public void CellComponentData_AllFields_CanBeSetTogether()
    {
        var data = new CellComponentData
        {
            componentName = "Painel Eletrico",
            function = "Controle e acionamento",
            partCode = "ELE-001",
            maintenanceNote = "Verificar disjuntores"
        };

        Assert.AreEqual("Painel Eletrico",          data.componentName);
        Assert.AreEqual("Controle e acionamento",   data.function);
        Assert.AreEqual("ELE-001",                  data.partCode);
        Assert.AreEqual("Verificar disjuntores",    data.maintenanceNote);
    }
}
