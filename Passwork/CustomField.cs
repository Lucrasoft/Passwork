namespace Passwork
{

    /// <summary>
    /// Custom fiels within a password record.
    /// </summary>
    public class CustomField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public CustomFieldType Type { get; set; } = CustomFieldType.text;
        public CustomField() { }
        public CustomField(string name, string value, CustomFieldType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }


    }
}
