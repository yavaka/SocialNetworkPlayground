using SocialMedia.Data.Models;
using SocialMedia.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Services.Group
{
    public interface IGroupService
    {
        Task<ICollection<GroupServiceModel>> GetNonMemberGroupsAsync(UserServiceModel currentUser);

        Task<ICollection<GroupServiceModel>> GetJoinedGroupsAsync(UserServiceModel currentUser);

        Task<ICollection<GroupServiceModel>> GetGroupsAsync();
      
        Task<GroupServiceModel> GetGroupAsync(int id);

        Task<bool> IsTitleExistAsync(string title);

        Task AddGroupAsync(GroupServiceModel serviceModel);

        Task EditGroupAsync(GroupServiceModel serviceModel);
        
        Task DeleteGroupAsync(int groupId);
        
        string GetGroupTitle(int groupId);

        Task<bool> IsCurrentUserMember(string currentUserId, int groupId);
        
        Task JoinGroupAsync(int groupId, string currentUserId);
       
        Task LeaveGroupAsync(int groupId, string currentUserId);
    }
}
