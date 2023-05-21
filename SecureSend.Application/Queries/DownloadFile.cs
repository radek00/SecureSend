using SecureSend.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSend.Application.Queries
{
    public record DownloadFile(Guid id, string fileName, string? contentType): IQuery<FileResultDto>
    {
    }
}
