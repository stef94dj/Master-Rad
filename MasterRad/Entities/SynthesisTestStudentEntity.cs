﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class SynthesisTestStudentEntity
    {
        public int SynthesisTestId { get; set; }
        public SynthesisTestEntity SynthesisTest { get; set; }
        
        public int StudentId { get; set; }
        public StudentEntity Student { get; set; }

        public SynthesisPaperEntity SynthesisPaper { get; set; }
    }
}