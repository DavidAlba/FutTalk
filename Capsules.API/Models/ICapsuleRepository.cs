using System.Threading.Tasks;

namespace Capsule.API.Models
{
    public interface ICapsuleRepository
    {
        Capsule GetCapsuleByUser(string user);
        Task<Capsule> GetCapsuleByUserAsync(string user);
        Capsule CreateNewCapsule(string user);
        Task<Capsule> CreateNewCapsuleAsync(string user);
        Capsule CreateNewCapsule(Capsule capsule);
        Task<Capsule> CreateNewCapsuleAsync(Capsule capsule);
        Capsule SaveCapsule(Capsule capsule);
        Task<Capsule> SaveCapsuleAsync(Capsule capsule);
    }
}
