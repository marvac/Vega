using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vega.Core;
using Vega.Core.Models;

namespace Vega.Persistence
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VegaDbContext _context;

        public VehicleRepository(VegaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(VehicleQuery queryObj)
        {
            var query = _context.Vehicles
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .AsQueryable();

            if (queryObj.MakeId.HasValue)
            {
                query = query.Where(x => x.Model.MakeId == queryObj.MakeId.Value);
            }

            if (queryObj.ModelId.HasValue)
            {
                query = query.Where(x => x.ModelId == queryObj.ModelId.Value);
            }

            var columnsMap = new Dictionary<string, Expression<Func<Vehicle, object>>>
            {
                { "make", v => v.Model.Make.Name },
                { "model", v => v.Model.Name },
                { "contactName", v => v.ContactName },
                { "id", v => v.Id }
            };

            if (queryObj.IsSortAscending)
            {
                query = query.OrderBy(columnsMap[queryObj.SortBy]);
            }
            else
            {
                query = query.OrderByDescending(columnsMap[queryObj.SortBy]);
            }

            return await query.ToListAsync();
        }

        public async Task<Vehicle> GetVehicleAsync(int id, bool includeRelated = true)
        {
            if (!includeRelated)
            {
                return await _context.Vehicles.FindAsync(id);
            }

            return await _context.Vehicles.Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .SingleOrDefaultAsync(v => v.Id == id);
        }

        public void AddVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await GetVehicleAsync(id, false);

            if (vehicle == null)
            {
                return false;
            }

            _context.Vehicles.Remove(vehicle);

            return true;
        }
    }
}
