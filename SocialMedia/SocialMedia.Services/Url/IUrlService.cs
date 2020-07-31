namespace SocialMedia.Services.Url
{
    using Microsoft.AspNetCore.Http;

    public interface IUrlService
    {
        string GenerateReturnUrl(string path, HttpContext httpContext);
    }
}
