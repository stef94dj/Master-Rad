﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.DTOs
{
    public class CreateTaskRQ
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
    }
    public class CreateTaskRS { }
}