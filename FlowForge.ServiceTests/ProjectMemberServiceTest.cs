using AutoFixture;
using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;
using FlowForge.Core.Hubs;
using FlowForge.Core.ServiceContracts;
using FlowForge.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Moq;

namespace FlowForge.ServiceTests
{
    public class ProjectMemberServiceTest
    {
        private readonly IProjectMemberService projectMemberService;
        private readonly Mock<IProjectMemberRepository> projectMemberRepository;
        private readonly Mock<IProjectRepository> projectRepository;
        private readonly Mock<NotificationService> notificationService;
        private readonly Mock<INotificationRepository> notificationRepository;

        private readonly Mock<IHubContext<NotificationHub>> hubContextMock;
        private readonly Mock<IClientProxy> clientProxyMock;
        private readonly Mock<IHubClients> hubClientsMock;
        private readonly Mock<IGroupManager> groupManagerMock;
        private readonly IFixture _fixure;

        public ProjectMemberServiceTest()
        {
            hubContextMock = new Mock<IHubContext<NotificationHub>>();
            hubClientsMock = new Mock<IHubClients>();
            clientProxyMock = new Mock<IClientProxy>();
            groupManagerMock = new Mock<IGroupManager>();

            hubClientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
            hubContextMock.Setup(c => c.Clients).Returns(hubClientsMock.Object);
            hubContextMock.Setup(c => c.Groups).Returns(groupManagerMock.Object);


            notificationRepository = new Mock<INotificationRepository>();
            projectRepository = new Mock<IProjectRepository>();
            projectMemberRepository = new Mock<IProjectMemberRepository>();
            notificationService = new Mock<NotificationService>();

            projectMemberService = new ProjectMemberService(projectRepository.Object, projectMemberRepository.Object, notificationService.Object, hubContextMock.Object);
            _fixure = new Fixture();
        }
        [Fact]
        public async Task SendJoinRequest_NullRequest_ThrowArgumentNullException()
        {
            ProjectJoinRequest? request = null;

            Func<Task> act = async () => await projectMemberService.SendJoinRequest(request);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SendJoinRequest_EmptyProjectId_ThrowArgumentException()
        {
            ProjectJoinRequest request = _fixure.Build<ProjectJoinRequest>().With(p => p.ProjectId, Guid.Empty).Create();

            Func<Task> act = async() => await projectMemberService.SendJoinRequest(request);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task SendJoinRequest_AlreadyExist_ReturnFalse()
        {
            ProjectMember member = _fixure.Create<ProjectMember>();

            projectRepository.Setup(m => m.GetProjectMembers(member.ProjectId))
                .ReturnsAsync([member]);

            notificationRepository.Setup(n => n.SendNotification(It.IsAny<Notification>()))
                .ReturnsAsync(true);

            var act = async () => await projectMemberService.SendJoinRequest(new ProjectJoinRequest
            {
                ProjectId = member.ProjectId,
                AddedUserId = member.MemberId != Guid.Empty ?  member.MemberId : Guid.NewGuid(),
                AddingUserId = Guid.NewGuid(),
                MemberRole = member.MemberRole
            });

            var result = await act();


            result.Should().BeFalse();
        }

        [Fact]
        public async Task SendJoinRequest_ValidInput_ReturnTrue()
        {
            var projectJoinRequest = _fixure.Create<ProjectJoinRequest>();

            ProjectMember member = new ProjectMember()
            {
                MemberId = projectJoinRequest.AddedUserId,
                ProjectId = projectJoinRequest.ProjectId,
                MemberRole = projectJoinRequest.MemberRole,
                MembershipStatus = MembershipStatus.PENDING
            };
            
            projectRepository.Setup(m => m.GetProjectMembers(member.ProjectId))
                .ReturnsAsync([member]);

            notificationRepository.Setup(n => n.SendNotification(It.IsAny<Notification>()))
                .ReturnsAsync(true);

            var act = async () => await projectMemberService.SendJoinRequest(projectJoinRequest);

            var result = await act();

            result.Should().BeTrue();
        }

    }
}
