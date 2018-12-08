using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.Core.Models;

namespace Vega.Core
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetVehicleAsync(int id, bool includeRelated = true);
        void AddVehicle(Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(int id);
    }
}
