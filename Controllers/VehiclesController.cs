using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    }
}
