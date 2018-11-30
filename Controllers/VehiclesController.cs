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

        [HttpGet()]
        public IActionResult GetVehicles()
        {
            return NotFound("GET not implemented");
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody]VehicleResource resource)
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

            var vehicle = _mapper.Map<VehicleResource, Vehicle>(resource);
            vehicle.LastUpdated = DateTime.Now;

            _context.Add(vehicle);

            int results = await _context.SaveChangesAsync();
            if (results > 0)
            {
                var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);
                return Ok(result);
            }

            return NotFound("Could not save object");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody]VehicleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _context.Vehicles.Include(v => v.Features)
                .SingleOrDefaultAsync(v => v.Id == id);

            vehicle.LastUpdated = DateTime.Now;

            _mapper.Map<VehicleResource, Vehicle>(resource, vehicle);


            int results = await _context.SaveChangesAsync();
            if (results > 0)
            {
                var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);
                return Ok(result);
            }

            return NotFound("Could not save object");
        }
    }
}
