using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.Controllers.Resources;
using Vega.Models;
using Vega.Persistence;

namespace Vega.Controllers
{
    [Route("/api/vehicles")]
    public class VehiclesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly VegaDbContext _context;

        public VehiclesController(IMapper mapper, VegaDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles.Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .SingleOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            var resource = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(resource);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody]SaveVehicleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _context.Models.FindAsync(resource.ModelId);
            if (model == null)
            {
                ModelState.AddModelError(nameof(resource.ModelId), "Invalid modelId");
                return BadRequest(ModelState);
            }

            var vehicle = _mapper.Map<SaveVehicleResource, Vehicle>(resource);
            vehicle.LastUpdated = DateTime.Now;

            _context.Add(vehicle);

            int results = await _context.SaveChangesAsync();
            if (results > 0)
            {
                var result = _mapper.Map<Vehicle, SaveVehicleResource>(vehicle);
                return Ok(result);
            }

            return NotFound("Could not save object");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody]SaveVehicleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _context.Vehicles.Include(v => v.Features)
                .SingleOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            vehicle.LastUpdated = DateTime.Now;

            _mapper.Map<SaveVehicleResource, Vehicle>(resource, vehicle);


            int results = await _context.SaveChangesAsync();
            if (results > 0)
            {
                var result = _mapper.Map<Vehicle, SaveVehicleResource>(vehicle);
                return Ok(result);
            }

            return NotFound("Could not save object");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            _context.Vehicles.Remove(vehicle);

            int results = await _context.SaveChangesAsync();
            if (results > 0)
            {
                return Ok(vehicle);
            }

            return NotFound("Could not delete object");
        }
    }
}
