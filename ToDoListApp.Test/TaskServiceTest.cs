using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ToDoListApp.Test
{
    public class TaskServiceTest(ITestOutputHelper testOutputHelper)
    {
        private readonly ITaskService taskService = new TaskService(null, null);
        private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
        #region AddTask Tests

        [Fact]
        public async Task AddTask_NullToDoTask()
        {
            TaskAddRequest? request = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            {
                await taskService.AddTask(request);
            });      
        }

        [Fact]
        public async Task AddTask_Success()
        {
            TaskAddRequest taskAddRequest = new()
            {
                Title = "Title",
                Description = "Description",
                GroupId = Guid.NewGuid(),
            };
            
            var taskResponse = await taskService.AddTask(taskAddRequest);
            var AddedTaskResponse = await taskService.GetTaskById(taskResponse.GroupId, taskResponse.TaskId);
            _testOutputHelper.WriteLine($"Expected:\n {taskResponse}");
            _testOutputHelper.WriteLine($"Actual:\n {AddedTaskResponse}");

            Assert.NotNull(taskResponse);
            Assert.Equal(taskResponse, AddedTaskResponse);
        }
        #endregion

        #region GetTaskById Tests

        [Fact]
        public void GetTaskById_NullId()
        {
            var taskResponse = taskService.GetTaskById(null ,null);
            Assert.Null(taskResponse);
        }

        [Fact]
        public async Task GetTaskById_Success()
        {
            TaskAddRequest taskAddRequest = new()
            {
                Title = "Title",
                Description = "Description",
                Success = true,
                GroupId = Guid.NewGuid(),
            };

            var taskResponse = await taskService.AddTask(taskAddRequest);

            var AddedTaskResponse = await taskService.GetTaskById(taskResponse.GroupId, taskResponse.TaskId);
            _testOutputHelper.WriteLine($"Expected:\n {taskResponse}");
            _testOutputHelper.WriteLine($"Actual:\n {AddedTaskResponse}");
            Assert.NotNull(AddedTaskResponse);
            Assert.Equal(taskResponse, AddedTaskResponse);
        }
        #endregion

        #region GetAllGroupTasks Tests

        [Fact]
        public async Task GetAllGroupTasks_Success()
        {
            Guid groupId = Guid.NewGuid();
            Guid AnotherId = Guid.NewGuid();
            List<TaskAddRequest> taskAddRequests = new List<TaskAddRequest>()
            {
                new() {
                    Title = "Title",
                    Description = "Description",
                    Success = true,
                    GroupId = groupId,
                },
                new()
                {
                    Title = "Title2",
                    Description = "Description2",
                    Success = false,
                    GroupId = groupId,
                },
                new()
                {
                    Title = "Different",
                    Description = "Other Description",
                    Success = true,
                    GroupId = AnotherId,
                }
            };
            List<TaskResponse> taskResponses = [];
            foreach (TaskAddRequest taskAddRequest in taskAddRequests)
            {
                taskResponses.Add(await taskService.AddTask(taskAddRequest));
            }
            List<TaskResponse>? taskResponsesFromGet = [.. (await taskService.GetAllGroupTasks(groupId))];

            _testOutputHelper.WriteLine($"Expected: ");
            foreach (var t in taskResponses)
            {
                if (t.GroupId == groupId)
                    _testOutputHelper.WriteLine(t.ToString());
            }

            _testOutputHelper.WriteLine($"Actual: ");
            foreach (var t in taskResponsesFromGet)
            {
                _testOutputHelper.WriteLine(t.ToString());
            }

            for (int i = 0; i < taskResponsesFromGet.Count; i++)
            {
                Assert.Equal(taskResponses[i], taskResponsesFromGet[i]);
            }
        }
        #endregion
    }
}
