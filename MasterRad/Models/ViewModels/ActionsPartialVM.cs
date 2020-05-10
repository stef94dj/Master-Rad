using System.Collections.Generic;

namespace MasterRad.Models.ViewModels
{
    public class ActionsPartialVM
    {
        public string Id { get; set; }
        public List<OptionModel> Options { get; set; }
    }

    public class OptionModel
    {
        public string Text { get; set; }
        public string TargetModalId { get; set; }
        public string ActionItemId { get; set; }
    }
}
