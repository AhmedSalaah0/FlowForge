using FlowForge.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class ChangeVisibilityRequest
    {
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public string? ProjectVisibility { get; set; }
    }
}
