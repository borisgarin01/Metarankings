using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces;
public interface IGamesPlatformsRepository
{
    public Task AddAsync(GamePlatform platform);
    public Task<IEnumerable<GamePlatform>> GetAllAsync();
}
