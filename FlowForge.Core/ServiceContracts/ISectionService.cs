using FlowForge.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.ServiceContracts
{
    public interface ISectionService
    {
        Task<SectionResponse> AddSection(Guid userId, SectionAddRequest sectionAddRequest);
        Task<List<SectionWithTasksResponse>> GetAllProjectSections(Guid userId, Guid projectId);
        Task<SectionWithTasksResponse?> GetSectionById(Guid userId, Guid sectionId);
        Task DeleteSection(Guid userId, Guid sectionId);

        Task<SectionResponse> EditSectionName(Guid userId, SectionUpdateRequest sectionUpdateRequest);
        //Task UpdateSection(Guid userId, SectionUpdateRequest sectionUpdateRequest);
    }
}
