using Microsoft.AspNetCore.Mvc;

namespace DevTalles.Ecommerce.WebAPI.Constants;

public abstract class CacheProfiles
{
    public const string DefaultCacheProfile = "DefaultCacheProfile";
    public const string Default20CacheProfile = "Default20CacheProfile";
    public const string NoCacheProfile = "NoCacheProfile";
    
    public static readonly Dictionary<string, CacheProfile> Profiles = new()
    {
        {
            DefaultCacheProfile, new CacheProfile
            {
                Duration = 60,
                Location = ResponseCacheLocation.Any,
                NoStore = false
            }
        },
        {
            Default20CacheProfile, new CacheProfile
            {
                Duration = 20,
                Location = ResponseCacheLocation.Any,
                NoStore = false
            }
        },
        {
            NoCacheProfile, new CacheProfile
            {
                NoStore = true
            }
        }
    };
}