using System.ComponentModel;

namespace docusign_integration.Models
{
    public class DocuSignModel
    {
        [DisplayName("Name")]
        public string UserName { get; set; }

        [DisplayName("Email")]
        public string UserEmail { get; set; }

        [DisplayName("Sign Location")]
        public string KeyString { get; set; }

        [DisplayName("File")]
        public IFormFile? UploadFile { get; set; }
    }
}
