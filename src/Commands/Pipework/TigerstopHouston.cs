using Autodesk.Revit.Attributes;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    public class TigerstopHouston : Tigerstop
    {
        public override string LocationPath
        {
            get
            {
                return $"\\\\houcad\\cad\\TigerStop\\HoustonTigerStop-Pipe\\{PackageName}.csv";
            }
        }
    }
}
