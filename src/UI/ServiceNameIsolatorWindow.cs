using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace DSI.UI
{
    [Transaction(TransactionMode.Manual)]
    public partial class ServiceNameIsolatorWindow : Form
    {
        private readonly List<string> elementServiceNames = new List<string>();
        private List<Data> uniqueServices = new List<Data>();

        public ServiceNameIsolatorWindow(ExternalCommandData commandData, ElementArray elements)
        {
            CommandData = commandData;
            Elements = elements;
            InitializeComponent();
        }

        public ExternalCommandData CommandData { get; set; }
        public ElementArray Elements { get; set; }

        private void ServiceNameIsolatorWindow_Load(object sender, EventArgs e)
        {
            foreach(Element elem in Elements)
            {
                if (elem is FabricationPart fp && fp.ServiceName != null)
                {
                    elementServiceNames.Add(fp.ServiceName);
                }
            }

            TotalNumOfElementsLabel.Text = elementServiceNames.Count.ToString(CultureInfo.InvariantCulture);
            TotalNumOfSelectedElementsLabel.Text = TotalNumOfElementsLabel.Text;

            uniqueServices = PruneNonUniques(elementServiceNames);

            foreach(Data service in uniqueServices)
            {
                ServiceNames.Items.Add(service.ServiceName);
            }

            for (int i = 0; i < ServiceNames.Items.Count; i++)
            {
                ServiceNames.SetItemChecked(i, true);
            }
        }

        private void CheckAllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ServiceNames.Items.Count; i++)
            {
                ServiceNames.SetItemChecked(i, true);
            }

            SelectedIndexChanged();
        }

        private void UncheckAllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ServiceNames.Items.Count; i++)
            {
                ServiceNames.SetItemChecked(i, false);
            }

            SelectedIndexChanged();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            UIDocument doc = CommandData.Application.ActiveUIDocument;
            List<ElementId> selectedElementIds = new List<ElementId>();

            if (Elements.Size != 0)
            {
                foreach (Element elem in Elements)
                {
                    foreach (string serviceName in ServiceNames.CheckedItems)
                    {
                        if (elem is FabricationPart fp && fp != null)
                        {
                            if (serviceName == fp.ServiceName)
                            {
                                selectedElementIds.Add(elem.Id);
                            }
                        }
                    }
                }

                if (IsolateElemBtn.Checked)
                {
                    using (Transaction t = new Transaction(doc.Document, "Isolate by Service Name"))
                    {
                        t.Start();
                        Autodesk.Revit.DB.View view = doc.ActiveView;
                        view.IsolateElementsTemporary(selectedElementIds);
                        doc.Selection.SetElementIds(new List<ElementId>());
                        t.Commit();
                    }
                }
                else if (SelectElemBtn.Checked)
                {
                    doc.Selection.SetElementIds(selectedElementIds);
                }
                else if (IsolateAndSelectBtn.Checked)
                {
                    using (Transaction t = new Transaction(doc.Document, "Isolate by Service Name"))
                    {
                        t.Start();
                        Autodesk.Revit.DB.View view = doc.ActiveView;
                        view.IsolateElementsTemporary(selectedElementIds);
                        doc.Selection.SetElementIds(selectedElementIds);
                        t.Commit();
                    }
                }
            }

            doc.RefreshActiveView();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
        
        }

        private void ServiceNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexChanged();
        }

        private void SelectedIndexChanged()
        {
            int totalSelectedElements = 0;
            foreach (string serviceName in ServiceNames.CheckedItems)
            {
                Data d = uniqueServices.Find(s => s.ServiceName == serviceName);
                totalSelectedElements += d.Quantity;
            }
            TotalNumOfSelectedElementsLabel.Text = totalSelectedElements.ToString(CultureInfo.InvariantCulture);
        }

        private List<Data> PruneNonUniques(List<string> _strings)
        {
            List<string> strings = _strings;
            List<Data> uniqueStrings = new List<Data>();
            if (strings.Count != 0)
            {
                string first = strings[0];
                int quantity = 0;

                for (int i = strings.Count - 1; i >= 0; i--)
                {
                    if (first == strings[i])
                    {
                        strings.RemoveAt(i);
                        quantity++;
                    }
                }

                uniqueStrings.Add(new Data(first, quantity));

                if (strings.Count > 0)
                {
                    return uniqueStrings.Concat(PruneNonUniques(strings)).ToList();
                }
                else
                {
                    return uniqueStrings;
                }
            }
            else
            {
                return uniqueStrings;
            }
        }

        private class Data
        {
            public Data(string serviceName, int quantity)
            {
                ServiceName = serviceName;
                Quantity = quantity;
            }

            public string ServiceName { get; set; }
            public int Quantity { get; set; }
        }
    }
}
