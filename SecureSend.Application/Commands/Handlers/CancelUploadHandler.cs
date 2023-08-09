using MediatR;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.Commands.Handlers;

public class CancelUploadHandler: ICommandHandler<CancelUpload, Unit>
{
    private readonly IFileService _fileService;
    private readonly ISecureSendUploadRepository _repository;

    public CancelUploadHandler(IFileService fileService, ISecureSendUploadRepository repository)
    {
        _fileService = fileService;
        _repository = repository;
    }

    public async Task<Unit> Handle(CancelUpload command, CancellationToken cancellationToken)
    {
        var persisted = await _repository.GetAsync(command.id, cancellationToken);
        if (persisted is null) throw new UploadDoesNotExistException(command.id);
        persisted.RemoveFile(command.fileName);
        _fileService.RemoveFileFromUpload(command.id, command.fileName);
        return Unit.Value;
    }
}