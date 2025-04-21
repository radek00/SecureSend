using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using SecureSend.Application.Commands;
using SecureSend.Application.Commands.Handlers;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Options;
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
    private readonly Mock<IOptions<FileStorageOptions>> _fileStorageOptions;
    private readonly Mock<IUploadSizeTrackerService> _sizeTrackerService;
    private readonly Mock<IFileService> _fileService;

    private readonly FileStorageOptions _sampleOptions = new FileStorageOptions()
    {
        MaxExpirationInDays = 0
    };

    
    public CreateSecureUploadHandlerTests()
    {
        _secureSendUploadRepository = new Mock<ISecureSendUploadRepository>();
        _secureSendUploadFactory = new Mock<ISecureSendUploadFactory>();
        _secureUploadReadService = new Mock<ISecureUploadReadService>();
        _fileStorageOptions = new Mock<IOptions<FileStorageOptions>>();
        _sizeTrackerService = new Mock<IUploadSizeTrackerService>();
        _fileService = new Mock<IFileService>();
        _commandHandler = new CreateSecureUploadHandler(_secureSendUploadRepository.Object, _secureSendUploadFactory.Object,
            _secureUploadReadService.Object, _fileStorageOptions.Object, _sizeTrackerService.Object, _fileService.Object);
    }
    

    #endregion
    
    [Fact]
    public async Task Handle_Succeeds()
    {
        _fileStorageOptions.Setup(op => op.Value).Returns(_sampleOptions);
        var command = new CreateSecureUpload(Guid.NewGuid(), DateTime.Now.AddDays(5), String.Empty);
        _secureUploadReadService.Setup(x => x.GetUploadId(command.uploadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Guid.Empty);
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.Null(exception);
    }
    [Fact]
    public async Task Handle_Throws_UploadAlreadyExistsException()
    {
        _fileStorageOptions.Setup(op => op.Value).Returns(_sampleOptions);
        var command = new CreateSecureUpload(Guid.NewGuid(), DateTime.Now.AddDays(5), String.Empty);
        _secureUploadReadService.Setup(x => x.GetUploadId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Guid.NewGuid());
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<UploadAlreadyExistsException>(exception);

    }

    [Fact]
    public async Task Handle_Throws_MaxExpirationExceededException()
    {
        _sampleOptions.MaxExpirationInDays = 3;
        _fileStorageOptions.Setup(op => op.Value).Returns(_sampleOptions);
        
        var command = new CreateSecureUpload(Guid.NewGuid(), DateTime.Now.AddDays(5), String.Empty);
        _secureUploadReadService.Setup(x => x.GetUploadId(command.uploadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Guid.Empty);
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.NotNull(exception);
        Assert.IsType<MaxExpirationExceededException>(exception);
    }
    
    [Fact]
    public async Task Handle_Succeeds_When_ExpiryDate_Is_Null()
    {
        _sampleOptions.MaxExpirationInDays = 3;
        _fileStorageOptions.Setup(op => op.Value).Returns(_sampleOptions);
        
        var command = new CreateSecureUpload(Guid.NewGuid(), null, String.Empty);
        _secureUploadReadService.Setup(x => x.GetUploadId(command.uploadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Guid.Empty);
        

        var exception = await Record.ExceptionAsync(() => _commandHandler.Handle(command, It.IsAny<CancellationToken>()));
        Assert.Null(exception);
    }
}