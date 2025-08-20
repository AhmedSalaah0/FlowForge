using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Infrastructure.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Infrastructure.Repositories
{
    public class SectionRepository(ApplicationDbContext context) : ISectionRepository
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<bool> AddSection(ProjectSection projectSection)
        {
            await _context.Sections.AddAsync(projectSection);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<ProjectSection?> GetProjectSectionById(Guid sectionId)
        {
            return await _context.Sections.FindAsync(sectionId);
        }

        public async Task<bool> DeleteSection(Guid sectionId)
        {
            _context.Sections.RemoveRange(_context.Sections.Where(temp => temp.SectionId == sectionId));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProjectSection?> EditSectionName(ProjectSection projectSection)
        {
            context.Update(projectSection);
            await context.SaveChangesAsync();
            return projectSection ?? null;
        }
    }
}
