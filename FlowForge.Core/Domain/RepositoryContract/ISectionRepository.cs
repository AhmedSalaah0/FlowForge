using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.Domain.RepositoryContract
{
    public interface ISectionRepository
    {
        Task<bool> AddSection(ProjectSection sectionAddRequest);
        Task<ProjectSection?> GetProjectSectionById(Guid sectionId);
        Task<bool> DeleteSection(Guid sectionId);
        Task<ProjectSection?> EditSectionName(ProjectSection projectSection);
    }
}
