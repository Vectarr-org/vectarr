using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Vectarr.Palooza.Classes.Filtering;
using Vectarr.Palooza.DataAccess;
using System.Security.Claims;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SystemController : ControllerBase
    {
        protected IUnitOfWork UnitOfWork;

        protected SystemController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            UnitOfWork = unitOfWork;

            if (httpContextAccessor != null && httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                unitOfWork.SetEmail(email);
            }
        }

        protected virtual List<Filter> GetFilters()
        {
            HttpRequestMessageFeature hreqmf = new HttpRequestMessageFeature(Request.HttpContext);
            HttpRequestMessage httpRequestMessage = hreqmf.HttpRequestMessage;

            var queryString = httpRequestMessage.GetQueryNameValuePairs().ToList();
            return GetFilters(queryString);
        }

        protected virtual List<Filter> GetFilters(List<KeyValuePair<string, string>> queryString)
        {
            var result = new List<Filter>();

            foreach (var keyValuePair in queryString)
            {
                FilterVariables variable;

                if (Enum.TryParse(keyValuePair.Key, true, out variable))
                {
                    //switch (variable)
                    //{
                    //    case FilterVariables.DateToday:
                    //    case FilterVariables.DateYesterday:
                    //    case FilterVariables.DateLastMonth:
                    //    case FilterVariables.DateLastWeek:
                    //    case FilterVariables.DateThisMonth:
                    //    case FilterVariables.DateThisWeek:
                    //        result.Add(new NoValueFilter(variable)
                    //        {
                    //            SourceKeyValuePair = keyValuePair
                    //        });
                    //        break;
                    //    default:
                    result.Add(new ValueFilter(variable, keyValuePair.Value)
                    {
                        SourceKeyValuePair = keyValuePair
                    });

                    //}
                }
            }
            return result;
        }
    }
}
