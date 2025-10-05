// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>EvUserService.cs</file>
// <author>Thilochana J M (IT22899224)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-08</date>
// <summary>
//   Service class implementing business logic for EV user management.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;

namespace ev_charge_point_api.Services
{
    public class EvUserService
    {
        private readonly EvUserRepository _repo;

        public EvUserService(EvUserRepository repo)
        {
            _repo = repo;
        }

        public Task<EvUser> RegisterAsync(EvUser user) => _repo.CreateAsync(user);

        public Task<List<EvUser>> GetAllAsync() => _repo.GetAllAsync();
        public Task<List<EvUser>> GetAllDeactiveAsync() => _repo.GetAllDeactiveAsync();
        public Task<EvUser> GetByNicAsync(string nic) => _repo.GetByNicAsync(nic);

        public async Task<bool> UpdateAsync(string nic, EvUser updated, string callerNic)
        {
            // owners can only update their own account
            if (!string.Equals(nic, callerNic, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Owners can only update their own account");

            // If password provided, hash it (controller/service should set updated.Password accordingly)
            if (!string.IsNullOrEmpty(updated.Password))
                updated.Password = BCrypt.Net.BCrypt.HashPassword(updated.Password);

            return await _repo.UpdateAsync(nic, updated);
        }

        public async Task<bool> DeactivateAsync(string nic, string callerNic, string? reason = null)
        {
            // owners can deactivate their own account
            if (!string.Equals(nic, callerNic, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Owners can only deactivate their own account");

            return await _repo.DeactivateAsync(nic, callerNic, reason);
        }

        public async Task<bool> ReactivateAsync(string nic)
        {
            // must be performed by backoffice - controller enforces this via role claim
            return await _repo.ReactivateAsync(nic);
        }
    }
}
