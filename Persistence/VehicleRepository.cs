﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vega.Core;
using Vega.Core.Models;
using Vega.Extensions;

namespace Vega.Persistence
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VegaDbContext _context;

        public VehicleRepository(VegaDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<Vehicle>> GetVehiclesAsync(VehicleQuery queryObj)
        {
            var result = new QueryResult<Vehicle>();

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
                { "contactName", v => v.ContactName }
            };

            query = query.ApplyOrdering(queryObj, columnsMap);
            result.TotalItems = await query.CountAsync();

            result.Items = query.ApplyPaging(queryObj);

            return result;
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
