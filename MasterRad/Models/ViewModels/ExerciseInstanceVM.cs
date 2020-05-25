using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class ExerciseInstanceVM
    {
        public int InstanceId { get; set; }
        public string InstanceTimeStamp { get; set; }

        public string Name { get; set; }
        public string DatabaseDescription { get; set; }
        public string NameOnServer { get; set; }
        public string SqlScript { get; set; }
    }
}
