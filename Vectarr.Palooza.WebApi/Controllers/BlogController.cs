using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.BlogServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : BaseController<BlogModel>
    {
        private readonly BlogServices _blogServices;
        public BlogController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _blogServices = new BlogServices(unitOfWork);
        }

        protected override IBaseManager<BlogModel> GetManager()
        {
            return _blogServices;
        }

        [HttpPost("GenerateNow")]
        [AllowAnonymous]
        public IActionResult GenerateNow()
        {
            BackgroundJob.Enqueue<BlogGeneratorService>(service => service.GenerateDailyBlogs());
            return Ok(new { message = "Blog generation job enqueued." });
        }

        [HttpGet("Get")]
        [AllowAnonymous]
        public override ListViewModel<BlogModel> Get(int pageNumber = 1, int countPerPage = 10, string? orderBy = null, string? sort = null)
        {
            var filters = GetFilters();
            return GetInternal(GetOnlyManager(), pageNumber, countPerPage, filters, orderBy,
                sort != null && sort.StartsWith("d"));
        }

        [HttpGet]
        [Route("GetItem")]
        [AllowAnonymous]
        public override ItemViewModel<BlogModel> GetItem(int id)
        {
            return GetItemInternal(GetOnlyManager(), id);
        }
    }
}
