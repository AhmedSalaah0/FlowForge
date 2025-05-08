using Entities;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class GroupResponse
    {
        public Guid GroupId { get; set; }
        public string? groupTitle { get; set; }
        public string selectedColor { get; set; }
        public DateTime? createdAt { get; set; }
    }
}
public static class GroupExtensions
{
    public static GroupResponse ToGroupResponse(this TasksGroup group)
    {
        return new GroupResponse
        {
            GroupId = group.GroupId,
            groupTitle = group.groupTitle,
            selectedColor = group.selectedColor,
            createdAt = group.createdAt,
        };
    }
}
