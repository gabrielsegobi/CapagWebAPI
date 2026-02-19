using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace TestSupport.Fakes
{
    public static class FakeFormFile
    {
        public static IFormFile Create(string caminho)
        {
            var bytes = File.ReadAllBytes(caminho);
            var stream = new MemoryStream(bytes);

            return new FormFile(
                stream,
                0,
                bytes.Length,
                "file",
                Path.GetFileName(caminho)
            )
            {
                ContentType = "text/plain"
            };
        }

    }
}
