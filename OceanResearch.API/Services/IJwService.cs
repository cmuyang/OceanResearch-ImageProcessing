using OceanResearch.API.Models;

namespace OceanResearch.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
