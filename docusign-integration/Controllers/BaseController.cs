using docusign_integration.Helper;
using docusign_integration.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using static DocuSign.eSign.Client.Auth.OAuth;

namespace docusign_integration.Controllers
{
    public class BaseController : Controller
    {
        public OAuthToken dsAuthToken = new();

        protected IDocuSignService _docuSignService { get; }

        public BaseController(IDocuSignService docuSignService)
        {
            _docuSignService = docuSignService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isNeedNewToken = true;
            var dsAuthData = TempData[TempConstant.DSAuthString];
            var dsAuthExpire = TempData[TempConstant.ExpireOn];

            if (dsAuthData != null && dsAuthExpire != null)
            {
                if (_docuSignService.IsDSTokenActive(dsAuthExpire.ToString()))
                {
                    isNeedNewToken = false;
                    dsAuthToken = JsonConvert.DeserializeObject<OAuthToken>(dsAuthData.ToString());

                    TempData[TempConstant.DSAuthString] = TempData[TempConstant.DSAuthString];
                    TempData[TempConstant.ExpireOn] = TempData[TempConstant.ExpireOn];
                }
            }

            if (isNeedNewToken)
            {
                dsAuthToken = _docuSignService.DSGetToken();

                TempData[TempConstant.DSAuthString] = JsonConvert.SerializeObject(dsAuthToken);
                TempData[TempConstant.ExpireOn] = DateTime.UtcNow.AddHours(1).ToString(AppConstant.DateFormat);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
