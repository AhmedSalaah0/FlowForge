﻿using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;
using FlowForge.Infrastructure.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Infrastructure.Repositories
{
    public class ProjectMemberRepository(ApplicationDbContext context) : IProjectMemberRepository
    {
        private readonly ApplicationDbContext _context = context;
        public async Task AcceptProjectMember(ProjectMember projectMember)
        {
            projectMember.MembershipStatus = MembershipStatus.ACCEPTED;
            _context.ProjectMembers.Update(projectMember);
            await _context.SaveChangesAsync();
        }

        public async Task AddProjectMember(ProjectMember projectMember)
        {
            await _context.ProjectMembers.AddAsync(projectMember);
        }

        public Task RejectProjectMember(ProjectMember projectMember)
        {
            projectMember.MembershipStatus = MembershipStatus.REJECTED;
            _context.ProjectMembers.Update(projectMember);
            return _context.SaveChangesAsync();
        }

        public Task<bool> RemoveProjectMember(ProjectMember projectMember)
        {
            _context.ProjectMembers.Remove(projectMember);
            return _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
        }
    }
}
