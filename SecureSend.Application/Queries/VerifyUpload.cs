using SecureSend.Application.DTO;

namespace SecureSend.Application.Queries;

public record VerifyUpload(Guid id): IQuery<UploadVerifyResponseDTO>;