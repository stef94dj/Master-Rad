using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class TaskSolutionVM
    {
        public int Id { get; set; }
        public string TimeStamp { get; set; }
        public string TaskName { get; set; }
        public string NameOnServer { get; set; }
    }
}
