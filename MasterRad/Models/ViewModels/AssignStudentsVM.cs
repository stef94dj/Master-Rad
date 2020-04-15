using MasterRad.DTO.RS;

namespace MasterRad.Models.ViewModels
{
    public class AssignStudentsVM
    {
        public int TestId { get; set; }
        public TestType TestType { get; set; }
        public int InitialPageSize { get; set; }
    }
}
