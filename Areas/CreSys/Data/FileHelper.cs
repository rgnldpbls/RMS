using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;

public static class FileHelper
{
    public static IFormFile ConvertToFormFile(byte[] fileBytes, string fileName)
    {
        if (fileBytes == null)
        {
            return null; // Return null if the fileBytes is null
        }

        var stream = new MemoryStream(fileBytes);
        var formFile = new FormFile(stream, 0, fileBytes.Length, fileName, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf" // You can set the content type as needed
        };
        return formFile;
    }
}
