namespace HRMS_ERP.Forms
{
    /// <summary>
    /// Reusable ComboBox item that stores a display Text and an integer Value.
    /// Used in all dropdowns across the application (Payroll, Taxation, Attendance, etc.)
    /// 
    /// Usage:
    ///   comboBox.Items.Add(new ComboItem("Ahmed Khan (EMP-001)", 5));
    ///   ComboItem selected = comboBox.SelectedItem as ComboItem;
    ///   int id = selected.Value;
    /// </summary>
    internal class ComboItem
    {
        public string Text  { get; }
        public int    Value { get; }

        public ComboItem(string text, int value)
        {
            Text  = text;
            Value = value;
        }

        // This makes the ComboBox display Text instead of the class name
        public override string ToString() => Text;
    }
}
