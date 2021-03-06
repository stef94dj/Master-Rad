﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class TaskEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(8191)]
        public string Description { get; set; }
        [MaxLength(255)]
        public string NameOnServer { get; set; }

        public int TemplateId { get; set; }
        public TemplateEntity Template { get; set; }

        public string SolutionSqlScript { get; set; }
        public List<SolutionColumnEntity> SolutionColumns { get; set; }
        public List<SynthesisTestEntity> SynthesisTests { get; set; }

        
    }
}
