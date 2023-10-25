namespace SaleApi.Models.Services
{
    public class UploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
		public string UploadImage(IFormFile imageFile)
		{
			if (imageFile?.Length > 0)
			{
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
				var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
				if (!allowedExtensions.Contains(fileExtension))
				{
					return "Định dạng tệp hình ảnh không hợp lệ!";
				}
				else
				{
					var fileName = Path.GetFileName(imageFile.FileName);
					var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);
					var imageDirectory = Path.GetDirectoryName(imagePath);
					if (!Directory.Exists(imageDirectory))
					{
						Directory.CreateDirectory(imageDirectory!);
					}
					using (var stream = new FileStream(imagePath, FileMode.Create))
					{
						imageFile.CopyTo(stream);
					}
					return $"/images/products/{fileName}";
				}
			}
			return string.Empty;
		}

	}
}
