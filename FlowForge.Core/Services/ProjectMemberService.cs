using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;
using FlowForge.Core.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Core.Services
{
    public class ProjectMemberService(IProjectRepository projectRepository, IProjectMemberRepository projectMemberRepository, INotificationService notificationService) : IProjectMemberService
    {
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository = projectMemberRepository;
        private readonly INotificationService _notificationService = notificationService;
        public async Task<bool> SendJoinRequest(ProjectJoinRequest projectJoinRequest)
        {
            ArgumentNullException.ThrowIfNull(projectJoinRequest);

            if (projectJoinRequest.ProjectId == Guid.Empty)
            {
                throw new ArgumentException("Invalid Project.");
            }
            var alreadyExists = (await _projectRepository.GetProjectMembers(projectJoinRequest.ProjectId))
                .Any(t => t.MemberId == projectJoinRequest.AddedUserId);


            if (!alreadyExists)
            {
                var existingProject = await _projectRepository.GetProjectById(projectJoinRequest.AddingUserId, projectJoinRequest.ProjectId) ?? throw new ArgumentException("Project not found.");

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
                await _projectMemberRepository.AddProjectMember(projectMember);
                bool result = await _notificationService.SendNotification(notification);
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
            var projectMembers = await _projectRepository.GetProjectMembers(projectId);

            var memberToRemove = projectMembers.FirstOrDefault(pm => pm.MemberId == memberId && pm.ProjectId == projectId);
            if (memberToRemove == null)
            {
                throw new ArgumentException("Member not found in the project.");
            }
            if (projectMembers.Any(pm => pm.MemberId == userId && pm.ProjectId == projectId && pm.MemberRole == ProjectRole.Member))
            {
                throw new UnauthorizedAccessException("Only admin and moderator can remove members from the project.");
            }

            if (memberToRemove.MemberRole == ProjectRole.Creator)
            {
                throw new UnauthorizedAccessException("Cannot remove the project creator.");
            }

            await _projectMemberRepository.RemoveProjectMember(memberToRemove);
            var notification = (await _notificationService.GetNotifications(userId))?.FirstOrDefault(t => t.ProjectId == projectId && t.ReceiverId == userId)?.NotificationId;
            await _notificationService.DeleteNotification(notification ?? Guid.Empty);
            await _notificationService.SendNotification(new Notification
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
            var projectMember = _projectRepository.GetProjectMembers(projectId).Result.FirstOrDefault(pm => pm.MemberId == userId && pm.ProjectId == projectId);
            if (projectMember == null)
            {
                throw new ArgumentException("Project member not found.");
            }
            await _projectMemberRepository.AcceptProjectMember(projectMember);
            var notification = (await _notificationService.GetNotifications(userId))?.FirstOrDefault(t => t.ProjectId == projectId && t.ReceiverId == userId);

            await _notificationService.SendNotification(new Notification
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

            var AcceptedNotification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                SenderId = projectMember.Project.CreatedById,
                ReceiverId = userId,
                ProjectId = projectId,
                Message = $"You have been joined to the project {projectMember.Project.ProjectTitle}.",
                NotificationType = NotificationType.INFO,
                Sender = projectMember.Project.CreatedBy,
                Receiver = projectMember.Member,
                Project = projectMember.Project
            };
            await _notificationService.SendNotification(AcceptedNotification);
            await _notificationService.DeleteNotification(notification.NotificationId);
            return true;
        }

        public async Task<bool> RejectProjectMember(Guid projectId, Guid userId)
        {
            if (projectId == Guid.Empty || userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid project or user ID.");
            }
            var projectMember = _projectRepository.GetProjectMembers(projectId).Result.FirstOrDefault(pm => pm.MemberId == userId && pm.ProjectId == projectId);
            if (projectMember == null)
            {
                throw new ArgumentException("Project member not found.");
            }
            await _projectMemberRepository.RejectProjectMember(projectMember);
            var notification = (await _notificationService.GetNotifications(userId))?.FirstOrDefault(t => t.ProjectId == projectId && t.ReceiverId == userId)?.NotificationId;

            await _notificationService.DeleteNotification(notification ?? Guid.Empty);
            await _notificationService.SendNotification(new Notification
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
            return true;
        }
    }
}