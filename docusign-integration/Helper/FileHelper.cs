namespace docusign_integration.Helper
{
    public class FileHelper
    {
        public static string PrepareFullPrivateKeyFilePath(string fileName)
        {
            const string DefaultRSAPrivateKeyFileName = "docusign_private_key.txt";

            var fileNameOnly = Path.GetFileName(fileName);
            if (string.IsNullOrEmpty(fileNameOnly))
            {
                fileNameOnly = DefaultRSAPrivateKeyFileName;
            }

            var filePath = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Directory.GetCurrentDirectory();
            }

            return Path.Combine(filePath, fileNameOnly);
        }

        public static byte[] ReadFileContent(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static string GetExtension(string fileName)
        {
            string[] strArr = fileName.Split('.');

            return strArr[strArr.Length - 1];
        }
    }
}
