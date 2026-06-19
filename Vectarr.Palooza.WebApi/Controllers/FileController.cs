using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class FileController : SystemController
    {
        private readonly IFileManager _fileManager;
        private readonly IImageManager _imageManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FileController(
            IUnitOfWork unitOfWork,
            IFileManager fileManager,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration, IImageManager imageManager) : base(unitOfWork, httpContextAccessor)
        {
            _fileManager = fileManager;
            _unitOfWork = unitOfWork;
            _hostingEnvironment = webHostEnvironment;
            _configuration = configuration;
            _imageManager = imageManager;
        }

        [HttpPost]
        [Route("UploadImage")]
        [AllowAnonymous]
        public async Task<ImageModel> UploadImage()
        {
            try
            {
                var file = HttpContext.Request.Form.Files[0];
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        // ImageManager.UploadImage handles:
                        // 1. Upload via _fileManager (LocalStorageManager)
                        // 2. Save to DB
                        // 3. Return ImageModel
                        return await _imageManager.UploadImage(
                            fileStream,
                            file.FileName,
                            _hostingEnvironment.WebRootPath,
                            file.FileName  // alt text
                        );
                    }
                }
                return new ImageModel();
            }
            catch (Exception ex)
            {
                throw new ErrorValidationException(ex.Message);
            }
        }

        // Endpoint to retrieve files
        [HttpGet("{fileName}")]
        [Authorize]
        public IActionResult GetFile(string fileName)
        {
            // Sanitize filename
            fileName = Path.GetFileName(fileName);

            var basePath = _configuration["FileStorage:BasePath"];
            var filePath = Path.Combine(basePath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".stp" or ".step" => "application/step",
                ".dwg" => "application/acad",
                ".dxf" => "application/dxf",
                ".stl" => "application/sla",
                _ => "application/octet-stream"
            };

            return PhysicalFile(filePath, contentType);
        }

        [HttpGet("blog/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetBlogFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            var basePath = _configuration["FileStorage:BasePath"];
            var filePath = Path.Combine(basePath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return PhysicalFile(filePath, contentType);
        }
    }
}