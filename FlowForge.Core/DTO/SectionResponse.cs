using FlowForge.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class SectionResponse
    {
        public Guid SectionId { get; set; }
        public string? SectionName { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CreatedById { get; set; }
    }

    public static class SectionResponseExtensions
    {
        public static SectionResponse ToSectionResponse(this ProjectSection section)
        {
            return new SectionResponse
            {
                SectionId = section.SectionId,
                SectionName = section.SectionName,
                ProjectId = section.ProjectId,
                CreatedById = section.CreatedById
            };
        }
    }
}
