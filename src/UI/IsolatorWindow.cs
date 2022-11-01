using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace DSI.UI
{
    [Transaction(TransactionMode.Manual)]
    public partial class IsolatorWindow : Form
    {
        private readonly List<string> elementPropertyNames = new List<string>();
        private List<Data> uniquePropertyNames = new List<Data>();

        public IsolatorWindow(ExternalCommandData commandData, ElementArray elements, string propertyName)
        {
            CommandData = commandData;
            Elements = elements;
            PropertyName = propertyName;
            InitializeComponent();
        }

        public ExternalCommandData CommandData { get; set; }
        public ElementArray Elements { get; set; }
        public string PropertyName { get; set; }
        
        private void GenericIsolatorWindow_Load(object sender, EventArgs e)
        {
            foreach (Element elem in Elements)
            {
                if (elem is FabricationPart fp && fp != null && fp.GetType().GetProperty(PropertyName) != null)
                {
                    string a = fp.GetType().GetProperty(PropertyName).GetValue(fp, null).ToString();
                    elementPropertyNames.Add(a);
                }
                else
                {
                    IList<Parameter> parameters = elem.GetParameters(PropertyName);

                    if (parameters.Count == 1)
                    {
                        if (parameters[0].AsString() is string s && s.Length != 0)
                        {
                            elementPropertyNames.Add(s);
                        }
                        else if (parameters[0].AsValueString() is string vs && vs.Length != 0)
                        {
                            elementPropertyNames.Add(vs);
                        }
                    }
                }
            }

            if (elementPropertyNames.Count != 0) {
                TotalNumOfElementsLabel.Text = elementPropertyNames.Count.ToString(CultureInfo.InvariantCulture);
                TotalNumOfSelectedElementsLabel.Text = TotalNumOfElementsLabel.Text;

                uniquePropertyNames = PruneNonUniques(elementPropertyNames);

                foreach (Data property in uniquePropertyNames)
                {
                    ElementProperties.Items.Add(property.PropertyName);
                }

                for (int i = 0; i < ElementProperties.Items.Count; i++)
                {
                    ElementProperties.SetItemChecked(i, true);
                }
            }
        }

        private void CheckAllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ElementProperties.Items.Count; i++)
            {
                ElementProperties.SetItemChecked(i, true);
            }

            SelectedIndexChanged();
        }

        private void UncheckAllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ElementProperties.Items.Count; i++)
            {
                ElementProperties.SetItemChecked(i, false);
            }

            SelectedIndexChanged();
        }

        private void ElementProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                    foreach (string property in ElementProperties.CheckedItems)
                    {
                        if (elem is FabricationPart fp && fp != null && fp.GetType().GetProperty(PropertyName) != null)
                        {
                            string a = fp.GetType().GetProperty(PropertyName).GetValue(fp, null).ToString();
                            if (property == a)
                            {
                                selectedElementIds.Add(elem.Id);
                            }
                        }
                        else
                        {
                            IList<Parameter> parameters = elem.GetParameters(PropertyName);

                            if (parameters.Count == 1)
                            {
                                if (parameters[0].AsString() is string s && s.Length != 0)
                                {
                                    if(property == s)
                                    {
                                        selectedElementIds.Add(elem.Id);
                                    }
                                }
                                else if (parameters[0].AsValueString() is string vs && vs.Length != 0)
                                {
                                    if (property == vs)
                                    {
                                        selectedElementIds.Add(elem.Id);
                                    }
                                }
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

        private void SelectedIndexChanged()
        {
            int totalSelectedElements = 0;
            foreach (string elementProperty in ElementProperties.CheckedItems)
            {
                Data d = uniquePropertyNames.Find(p => p.PropertyName == elementProperty);
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
            public Data(string propertyName, int quantity)
            {
                PropertyName = propertyName;
                Quantity = quantity;
            }

            public string PropertyName { get; set; }
            public int Quantity { get; set; }
        }
    }
}
