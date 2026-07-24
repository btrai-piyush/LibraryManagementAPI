using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Helpers
{
    public static class ActivityLogHelper
    {
        public static ActivityLog CreateActivity(int userId, ActivityType activityType, string description, string? metaData = null, int? referenceId = null)
        {
            return new ActivityLog
            {
                UserId = userId,
                ActivityType = activityType,
                Description = description,
                MetaData = metaData,
                ReferenceId = referenceId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
