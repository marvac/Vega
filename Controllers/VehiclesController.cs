using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Models;

namespace Vega.Controllers
{
    [Route("/api/vehicles")]
    public class VehiclesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVehicleRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public VehiclesController(IMapper mapper, IVehicleRepository repo, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<QueryResult<VehicleResource>> GetVehicles(VehicleQueryResource filter)
        {
            var filterMap = _mapper.Map<VehicleQueryResource, VehicleQuery>(filter);
            var queryResult = await _repo.GetVehiclesAsync(filterMap);
            return _mapper.Map<QueryResult<Vehicle>, QueryResult<VehicleResource>>(queryResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await _repo.GetVehicleAsync(id);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            var resource = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(resource);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVehicle([FromBody]SaveVehicleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = _mapper.Map<SaveVehicleResource, Vehicle>(resource);
            vehicle.LastUpdated = DateTime.Now;

            _repo.AddVehicle(vehicle);
            await _unitOfWork.CompleteAsync();

            vehicle = await _repo.GetVehicleAsync(vehicle.Id);

            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody]SaveVehicleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _repo.GetVehicleAsync(id);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            vehicle.LastUpdated = DateTime.Now;

            _mapper.Map<SaveVehicleResource, Vehicle>(resource, vehicle);


            await _unitOfWork.CompleteAsync();

            vehicle = await _repo.GetVehicleAsync(vehicle.Id);
            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(vehicle);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var succeeded = await _repo.DeleteVehicleAsync(id);

            if (!succeeded)
            {
                return BadRequest("Vehicle not found");
            }

            int results = await _unitOfWork.CompleteAsync();

            if (results > 0)
            {
                return Ok("Vehicle deleted");
            }

            return NotFound("Could not delete object");
        }
    }
}
