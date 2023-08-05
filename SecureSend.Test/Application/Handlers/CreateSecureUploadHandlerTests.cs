using MediatR;
using Moq;
using SecureSend.Application.Commands;
using SecureSend.Application.Commands.Handlers;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;

namespace SecureSend.Test.Application.Handlers;

public class CreateSecureUploadHandlerTests
{
    #region ARRANGE
    
    private readonly Mock<ISecureSendUploadRepository> _secureSendUploadRepository;
    private readonly Mock<ISecureSendUploadFactory> _secureSendUploadFactory;
    private readonly Mock<ISecureUploadReadService> _secureUploadReadService;
    private readonly ICommandHandler<CreateSecureUpload, Unit> _commandHandler;

    
    public CreateSecureUploadHandlerTests()
    {
        _secureSendUploadRepository = new Mock<ISecureSendUploadRepository>();
        _secureSendUploadFactory = new Mock<ISecureSendUploadFactory>();
        _secureUploadReadService = new Mock<ISecureUploadReadService>();
        _commandHandler = new CreateSecureUploadHandler(_secureSendUploadRepository.Object, _secureSendUploadFactory.Object,
            _secureUploadReadService.Object);
    }
    

    #endregion
    
    [Fact]
    public async void Handle_Succeeds()
    {
        var command = new CreateSecureUpload(Guid.NewGuid(), DateTime.Now.AddDays(5));
        _secureUploadReadService.Setup(x => x.GetUploadId(command.uploadId, new CancellationToken()))
            .ReturnsAsync(() => Guid.Empty);
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, new CancellationToken()));
        Assert.Null(exception);
    }
    [Fact]
    public async void Handle_Throws_UploadAlreadyExistsException()
    {
        var command = new CreateSecureUpload(Guid.NewGuid(), DateTime.Now.AddDays(5));
        _secureUploadReadService.Setup(x => x.GetUploadId(command.uploadId, new CancellationToken()))
            .ReturnsAsync(() => Guid.NewGuid());
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, new CancellationToken()));
        Assert.NotNull(exception);
        Assert.IsType<UploadAlreadyExistsException>(exception);

    }
}