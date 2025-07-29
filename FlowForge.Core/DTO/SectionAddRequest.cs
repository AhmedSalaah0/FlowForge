using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class SectionAddRequest
    {
        public string? SectionName { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        public ProjectSection ToSection()
        {
            return new ProjectSection
            {
                SectionName = SectionName,
                ProjectId = ProjectId,
                CreatedById = CreatedById,
                CreatedBy = CreatedBy,
            };
        }
    }
}
