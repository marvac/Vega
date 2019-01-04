using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
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
        private readonly PhotoSettings _photoSettings;

        public PhotosController(
            IHostingEnvironment env, 
            IMapper mapper, 
            IVehicleRepository repo, 
            IUnitOfWork unitOfWork, 
            IOptionsSnapshot<PhotoSettings> options)
        {
            _env = env;
            _mapper = mapper;
            _repo = repo;
            _unitOfWork = unitOfWork;
            _photoSettings = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int vehicleId, IFormFile file)
        {
            var vehicle = await _repo.GetVehicleAsync(vehicleId, false);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found");
            }

            if (file == null)
            {
                return BadRequest("Invalid file");
            }

            if (file.Length <= _photoSettings.MinBytes)
            {
                return BadRequest("Empty File");
            }

            if (file.Length > _photoSettings.MaxBytes)
            {
                return BadRequest("Max file size exceeded");
            }

            if (!_photoSettings.HasValidExtension(file.FileName))
            {
                return BadRequest("Invalid file type");
            }

            var folderPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
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
