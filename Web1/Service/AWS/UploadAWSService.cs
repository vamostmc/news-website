
using Amazon.S3;
using Amazon.S3.Transfer;
using Web1.Models;

namespace Web1.Service.AWS
{
    public class UploadAWSService : IUploadAWSService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;

        public UploadAWSService(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _config = config;
        }

        public async Task<Success> DeleteFileAWS(string nameFile)
        {
            try
            {
                await _s3Client.DeleteObjectAsync(_config["AWS:BucketName"], nameFile);
                return new Success { success = true };
            }
            catch (Exception ex)
            {
                return new Success { success = false, message = ex.Message };
            }

        }

        public async Task<string> UploadFileToAWS(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ!");

            using var transferUtility = new TransferUtility(_s3Client);
            using var stream = file.OpenReadStream();

            // Đổi tên file để tránh trùng lặp
            string fileKey = $"{Guid.NewGuid()}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = fileKey,
                BucketName = _config["AWS:BucketName"],
                ContentType = file.ContentType,
            };

            await transferUtility.UploadAsync(uploadRequest);

            // Trả về URL đầy đủ của file trên S3
            return fileKey;
        }
    }
}
