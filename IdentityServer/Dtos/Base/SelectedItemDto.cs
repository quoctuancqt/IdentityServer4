namespace IdentityServer.Dtos.Base
{
    public class SelectedItemDto
    {
        public SelectedItemDto() { }

        public SelectedItemDto(string value, string text, bool selected)
        {
            Value = value;
            Text = text;
            Selected = selected;
        }

        public SelectedItemDto(string value, string text, bool selected, object extensionValue)
        {
            Value = value;
            Text = text;
            Selected = selected;
            ExtensionValue = extensionValue;
        }

        public string Value { get; set; }

        public string Text { get; set; }

        public object ExtensionValue { get; set; }

        public bool Selected { get; set; }
    }
}
