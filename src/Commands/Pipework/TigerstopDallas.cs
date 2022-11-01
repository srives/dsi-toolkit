using Autodesk.Revit.Attributes;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    public class TigerstopDallas : Tigerstop
    {
        public override string LocationPath
        {
            get
            {
                return $"\\\\dallas\\Detailing\\CAD\\TigerStop\\DallasTigerStop-Pipe\\{PackageName}.csv";
            }
        }
    }
}
