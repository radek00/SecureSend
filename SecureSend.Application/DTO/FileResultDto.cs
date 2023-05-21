namespace SecureSend.Application.DTO
{
    public class FileResultDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public FileStream FileStream { get; set; }
    }
}
