namespace SecureSend.Application.DTO
{
    public class FileResultDto
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }

        public required FileStream FileStream { get; set; }
    }
}
