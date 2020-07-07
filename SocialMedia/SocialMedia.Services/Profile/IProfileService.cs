using SocialMedia.Services.Models;
using System.Threading.Tasks;

namespace SocialMedia.Services.Profile
{
    public interface IProfileService
    {
        Task<ProfileServiceModel> GetProfileAsync(string userId);
    }
}
