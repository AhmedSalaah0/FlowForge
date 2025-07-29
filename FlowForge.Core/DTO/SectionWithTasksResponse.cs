using FlowForge.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class SectionWithTasksResponse
    {
        public Guid SectionId { get; set; }
        public string? SectionName { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CreatedById { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; } = [];
    }

    public static class SectionWithTasksResponseExtensions
    {
        public static SectionWithTasksResponse ToSectionWithTasksResponse(this ProjectSection section)
        {
            return new SectionWithTasksResponse
            {
                SectionId = section.SectionId,
                SectionName = section.SectionName,
                ProjectId = section.ProjectId,
                CreatedById = section.CreatedById,
                Tasks = section.Tasks ?? new List<ProjectTask>()
            };
        }
    }
}
