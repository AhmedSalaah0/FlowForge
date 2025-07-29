using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.Services
{
    public class SectionService(ISectionRepository sectionRepository, IProjectService projectService) : ISectionService
    {
        private readonly ISectionRepository _sectionRepository = sectionRepository;
        public async Task<SectionResponse> AddSection(Guid userId, SectionAddRequest sectionAddRequest)
        {
            sectionAddRequest.CreatedById = userId;
            var section = sectionAddRequest.ToSection();

            var result = await _sectionRepository.AddSection(section);
            if (!result)
            {
                throw new InvalidOperationException("Failed to add section");
            }
            return section.ToSectionResponse();
        }

        public async Task DeleteSection(Guid userId, Guid sectionId)
        {
            if (userId == Guid.Empty || sectionId == Guid.Empty)
            {
                throw new ArgumentException("Invalid userId or sectionId");
            }
            var section = await _sectionRepository.GetProjectSectionById(sectionId);
           
            if (section == null)
            {
                throw new KeyNotFoundException("Section not found");
            }
            var project = await projectService.GetProjectById(userId, section.ProjectId);

            if (section.CreatedById != userId || project.ProjectMembers.FirstOrDefault(u => u.MemberId == userId).MemberRole == Enums.ProjectRole.Member)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this section.");
            }

            var result = await _sectionRepository.DeleteSection(sectionId);
            if (!result)
            {
                throw new InvalidOperationException("Failed to delete section");
            }
        }

        public Task<List<SectionWithTasksResponse>> GetAllProjectSections(Guid userId, Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Task<SectionWithTasksResponse?> GetSectionById(Guid userId, Guid sectionId)
        {
            throw new NotImplementedException();
        }
    }
}
