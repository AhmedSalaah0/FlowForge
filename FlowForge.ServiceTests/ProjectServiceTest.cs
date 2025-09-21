using AutoFixture;
using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using FlowForge.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace FlowForge.ServiceTests
{
    public class ProjectServiceTest
    {
        private readonly IProjectService _projectService;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IProjectMemberRepository> _projectMemberRepositoryMock;

        private readonly IFixture _fixture; 

        public ProjectServiceTest()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _projectService = new ProjectService(_projectRepositoryMock.Object, _projectMemberRepositoryMock.Object);
            _fixture = new Fixture();
        }

        #region Create Project Tests
        
        [Fact]
        public async Task CreateProject_NullAddRequest_ShouldThrowNullException()
        {
            ProjectAddRequest? addRequest = null;
            
            Func<Task> act = async () => await _projectService.CreateProject(addRequest);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateProject_NullUserId_ShouldThrowNullException()
        {
            var addRequest = _fixture.Create<ProjectAddRequest>();
            addRequest.CreatedById = Guid.Empty;
            
            Func<Task> act = async () => await _projectService.CreateProject(addRequest);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateProject_ValidInput_ShouldReturnProjectResponse()
        {
            var userId = Guid.NewGuid();
            ProjectAddRequest projectAddRequest = _fixture.Build<ProjectAddRequest>().With(p => p.CreatedById, userId).With(p => p.CreatedAt, DateTime.Now).Create();
            Project project = projectAddRequest.ToProject();
            ProjectResponse ExpectedResponse = project.ToProjectResponse(userId);

            _projectRepositoryMock.Setup(p => p.CreateProject(It.IsAny<Project>()))
                .ReturnsAsync(project);

            var projectResponse = await _projectService.CreateProject(projectAddRequest);
            ExpectedResponse.ProjectId = projectResponse.ProjectId;

            projectResponse.ProjectId.Should().NotBe(Guid.Empty);
            projectResponse.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            projectResponse.CreatedById.Should().Be(ExpectedResponse.CreatedById);
        }
        #endregion

        #region GetProjects Tests

        [Fact]
        public async Task GetProjects_EmptyUserGuid_ShouldArgumentException()
        {
            Guid userId = Guid.Empty;

            var act = async () => await _projectService.GetProjects(userId);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetProjects_ValidInput_ShouldReturnUserProjects()
        {
            Guid userId = Guid.NewGuid();

            List<ProjectAddRequest> addRequests = [_fixture.Build<ProjectAddRequest>().With(t => t.CreatedById, userId).Create(), 
                _fixture.Build<ProjectAddRequest>().With(t => t.CreatedById, userId).Create()];

            List<Project> mockResponse = addRequests.Select(p => p.ToProject()).ToList();
            List<ProjectResponse> responses = mockResponse.Select(p => p.ToProjectResponse(userId)).ToList();

            _projectRepositoryMock
               .Setup(p => p.GetProjects(It.IsAny<Guid>()))
               .ReturnsAsync(mockResponse);
            List<ProjectResponse> projectResponses = await _projectService.GetProjects(userId);

            projectResponses.Should().HaveCount(responses.Count);
            projectResponses.Should().BeEquivalentTo(responses);
        }
        #endregion 

        #region GetProjectById Tests
        [Fact]
        public async Task GetProjectById_EmptyUserId_ShouldBeArgumentException()
        {
            var userId = Guid.Empty;
            var projectId = Guid.NewGuid();

            var act = async () => await _projectService.GetProjectById(userId, projectId);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetProjectById_EmptyProjectId_ShouldBeArgumentException()
        {
            var userId = Guid.NewGuid();
            var projectId = Guid.Empty;

            var act = async () => await _projectService.GetProjectById(userId, projectId);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetProjectById_ValidInput_ShouldReturnProject()
        {
            var userId = Guid.NewGuid();
            ProjectAddRequest projectAddRequest = _fixture.Build<ProjectAddRequest>().With(p => p.CreatedById, userId).With(p => p.CreatedAt, DateTime.Now).Create();
            Project project = projectAddRequest.ToProject();

            _projectRepositoryMock.Setup(p => p.CreateProject(It.IsAny<Project>()))
                .ReturnsAsync(project);

            var projectResponse = await _projectService.CreateProject(projectAddRequest);
            project.ProjectId = projectResponse.ProjectId;
            project.ProjectMembers = projectResponse.ProjectMembers;

            _projectRepositoryMock.Setup(p => p.GetProjectMembers(It.IsAny<Guid>()))
                .ReturnsAsync(projectResponse.ProjectMembers);

            _projectRepositoryMock.Setup(p => p.GetProjectById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(project);

            var response = await _projectService.GetProjectById(userId, projectResponse.ProjectId);
            response.Should().NotBeNull();
            response.CreatedAt.Should().BeCloseTo(projectResponse.CreatedAt, TimeSpan.FromSeconds(5));
            response.Should().BeEquivalentTo(projectResponse, options =>
                options.Excluding(p => p.CreatedAt)
            );
        }
        #endregion
    }
}
