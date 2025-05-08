using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class GroupAddRequest
    {
        public string? groupTitle { get; set; }
        public string selectedColor { get; set; }
        public DateTime? createdAt { get; set; }
    
        public TasksGroup ToGroupTasks()
        {
            return new TasksGroup
            {
                GroupId = Guid.NewGuid(),
                groupTitle = groupTitle,
                selectedColor = selectedColor,
                createdAt = createdAt,
            };
        }
    }
}
