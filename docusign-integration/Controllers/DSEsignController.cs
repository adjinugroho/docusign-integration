using docusign_integration.Helper;
using docusign_integration.Models;
using docusign_integration.Services;
using Microsoft.AspNetCore.Mvc;

namespace docusign_integration.Controllers
{
    public class DSEsignController : BaseController
    {
        public DSEsignController(IDocuSignService docuSignService)
            : base(docuSignService)
        {
        }

        public IActionResult ImpersonateSign()
        {
            return View(new DocuSignModel());
        }

        [HttpPost]
        public IActionResult ImpersonateSign(DocuSignModel dsModel)
        {
            var ext = FileHelper.GetExtension(dsModel.UploadFile.FileName);

            if (ext == "pdf" || ext == "docx")
            {
                var dsResult = _docuSignService.DSCeremony(dsAuthToken, dsModel);

                return Redirect(dsResult);
            }

            dsModel.UploadFile = null;

            return View(dsModel);
        }
    }
}
