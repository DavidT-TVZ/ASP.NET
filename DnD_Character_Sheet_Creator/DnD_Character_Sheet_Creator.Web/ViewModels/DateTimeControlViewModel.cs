using System;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class DateTimeControlViewModel
    {
        public DateTime? Value { get; set; }

        public string CssClass { get; set; } = string.Empty;

        public string EmptyText { get; set; } = "No date available";
    }
}