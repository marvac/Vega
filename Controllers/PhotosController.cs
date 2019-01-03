using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Models;

namespace Vega.Controllers
{
    [Route("/api/vehicles/{vehicleId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IVehicleRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public PhotosController(IHostingEnvironment env, IMapper mapper, IVehicleRepository repo, IUnitOfWork unitOfWork)
        {
            _env = env;
            _mapper = mapper;
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int vehicleId, IFormFile file)
        {
            var vehicle = await _repo.GetVehicleAsync(vehicleId, false);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found");
            }

            var folderPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var uploadFilePath = Path.Combine(folderPath, fileName);

            using (var fs = new FileStream(uploadFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            var photo = new Photo { FileName = fileName };

            vehicle.Photos.Add(photo);
            await _unitOfWork.CompleteAsync();

            var resource = _mapper.Map<Photo, PhotoResource>(photo);
            return Ok(resource);
        }
    }
}
