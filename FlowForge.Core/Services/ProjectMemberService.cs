using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;
using FlowForge.Core.Hubs;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;

namespace FlowForge.Core.Services
{
    public class ProjectMemberService(IProjectRepository projectRepository, IProjectMemberRepository projectMemberRepository, INotificationService notificationService, IHubContext<NotificationHub> notificationHub) : IProjectMemberService
    {
        public async Task<bool> SendJoinRequest(ProjectJoinRequest projectJoinRequest)
        {
            ArgumentNullException.ThrowIfNull(projectJoinRequest);

            if (projectJoinRequest.ProjectId == Guid.Empty)
            {
                throw new ArgumentException("Invalid Project.");
            }
            var projectMembers = (await projectRepository.GetProjectMembers(projectJoinRequest.ProjectId));

            bool memberExist = projectMembers.Any(pm => pm.MemberId == projectJoinRequest.AddedUserId);

            if (!memberExist)
            {

                var existingProject = projectMembers.FirstOrDefault(p => p.ProjectId == projectJoinRequest.ProjectId).Project;
                var projectMember = new ProjectMember
                {
                    MemberId = projectJoinRequest.AddedUserId,
                    MemberRole = projectJoinRequest.MemberRole,
                    ProjectId = projectJoinRequest.ProjectId,
                    Project = existingProject,
                    Member = projectJoinRequest.AddedUser
                };

                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    SenderId = projectJoinRequest.AddedUserId,
                    ReceiverId = projectJoinRequest.AddingUserId,
                    ProjectId = projectJoinRequest.ProjectId,
                    Project = existingProject,
                    Message = $"You have been added to the project {existingProject.ProjectTitle}.",
                    NotificationType = NotificationType.JOIN_REQUEST,
                    Sender = projectJoinRequest.AddingUser,
                    Receiver = projectJoinRequest.AddedUser,
                };
                await projectMemberRepository.AddProjectMember(projectMember);
                bool result = await notificationService.SendNotification(notification);
                return result;
            }
            return false;
        }
        public async Task<bool> RemoveProjectMember(Guid projectId, Guid memberId, Guid userId)
        {
            if (projectId == Guid.Empty || memberId == Guid.Empty ||userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid Remove Attempts");
            }
            var projectMembers = await projectRepository.GetProjectMembers(projectId);

            var memberToRemove = projectMembers.FirstOrDefault(pm => pm.MemberId == memberId && pm.ProjectId == projectId) ?? throw new ArgumentException("Member not found in the project.");
            
            if (projectMembers.Any(pm => pm.MemberId == userId && pm.ProjectId == projectId && pm.MemberRole == ProjectRole.Member) && userId != memberId)
            {
                throw new UnauthorizedAccessException("Only admin and moderator can remove members from the project.");
            }

            if (memberToRemove.MemberRole == ProjectRole.Creator && userId != memberToRemove.MemberId)
            {
                throw new UnauthorizedAccessException("Cannot remove the project creator.");
            }

            await projectMemberRepository.RemoveProjectMember(memberToRemove);
            try
            {
                await notificationService.DeleteAllUserProjectNotifications(memberId, projectId);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Failed to delete notification.", ex);
            }
            await notificationService.SendNotification(new Notification
            {
                NotificationId = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = memberToRemove.Project.CreatedById,
                ProjectId = projectId,
                Message = $"{memberToRemove.Member.PersonName} has been removed from the project {memberToRemove.Project.ProjectTitle}.",
                NotificationType = NotificationType.INFO,
                Sender = memberToRemove.Member,
                Receiver = memberToRemove.Project.CreatedBy,
                Project = memberToRemove.Project
            });
            return true;
        }
        public async Task<bool> AcceptProjectMember(Guid projectId, Guid userId)
        {
            if (projectId == Guid.Empty || userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid project or user ID.");
            }
            var projectMember = await projectRepository.GetProjectMemberById(projectId, userId);
            if (projectMember == null)
            {
                throw new ArgumentException("Project member not found.");
            }
            await projectMemberRepository.AcceptProjectMember(projectMember);
            var notification = (await notificationService.GetNotifications(userId))?.FirstOrDefault(t => t.ProjectId == projectId && t.ReceiverId == userId);

            await notificationService.SendNotification(new Notification
            {
                NotificationId = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = projectMember.Project.CreatedById,
                ProjectId = projectId,
                Message = $"{projectMember.Member.PersonName} has been accepted to join the project {projectMember.Project.ProjectTitle}.",
                NotificationType = NotificationType.INFO,
                Sender = projectMember.Member,
                Receiver = projectMember.Project.CreatedBy,
                Project = projectMember.Project
            });

            notification.NotificationType = NotificationType.INFO;
            notification.Message = $"you have been successfully added to the project {projectMember.Project.ProjectTitle}";
            await notificationService.EditNotification(notification);

            return true;
        }

        public async Task<bool> RejectProjectMember(Guid projectId, Guid userId)
        {
            if (projectId == Guid.Empty || userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid project or user ID.");
            }
            var projectMember = projectRepository.GetProjectMembers(projectId).Result.FirstOrDefault(pm => pm.MemberId == userId && pm.ProjectId == projectId);
            if (projectMember == null)
            {
                throw new ArgumentException("Project member not found.");
            }
            await projectMemberRepository.RejectProjectMember(projectMember);
            var notification = (await notificationService.GetNotifications(userId))?.FirstOrDefault(t => t.ProjectId == projectId && t.ReceiverId == userId);


            await notificationService.SendNotification(new Notification
            {
                NotificationId = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = projectMember.Project.CreatedById,
                ProjectId = projectId,
                Message = $"{projectMember.Member.PersonName} has been rejected your request to join the project {projectMember.Project.ProjectTitle}.",
                NotificationType = NotificationType.INFO,
                Sender = projectMember.Member,
                Receiver = projectMember.Project.CreatedBy,
                Project = projectMember.Project
            });

            notification.NotificationType = NotificationType.INFO;
            notification.Message = $"You have been reject to join the project {projectMember.Project.ProjectTitle}";
            await notificationService.EditNotification(notification);
            return true;
        }
    }
}