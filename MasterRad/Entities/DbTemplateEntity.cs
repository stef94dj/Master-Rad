﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class DbTemplateEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        public string ModelDescription { get; set; }
        [MaxLength(200)]
        public string NameOnServer { get; set; }

        public List<TaskEntity> Tasks{ get; set; }
    }
}
