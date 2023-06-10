using SecureSend.Application.DTO;

namespace SecureSend.Application.Queries
{
    public record DownloadFile(Guid id, string fileName): IQuery<FileResultDto>
    {
    }
}
