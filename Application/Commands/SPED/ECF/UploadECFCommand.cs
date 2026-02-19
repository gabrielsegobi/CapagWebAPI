using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.SPED.ECF
{
    public class UploadECFCommand : IRequest
    {
        public UploadECFCommand(UploadECFRequest request)
        {
            Request = request;
        }

        public UploadECFRequest Request { get; set; }
    }


    public class UploadECFRequest
    {
        public IReadOnlyList<IFormFile> Files { get; set; } = [];
        public long IdEmpresa { get; set; }
        public bool Overwrite { get; set; } = false;
    }

}
