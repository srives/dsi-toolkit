using Autodesk.Revit.Attributes;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    public class TigerstopBuda : Tigerstop
    {
        public override string LocationPath
        {
            get
            {
                return $"\\\\budacad\\CAD\\TigerStop\\BudaTigerStop-Pipe\\{PackageName}.csv";
            }
        }
    }
}
