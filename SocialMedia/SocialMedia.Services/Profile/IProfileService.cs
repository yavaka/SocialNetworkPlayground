namespace SocialMedia.Services.Profile
{
    using System.Threading.Tasks;
    
    public interface IProfileService
    {
        Task<ProfileServiceModel> GetProfileAsync(string userId);
    }
}
