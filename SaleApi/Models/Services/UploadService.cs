namespace SaleApi.Models.Services
{
	public class UploadService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public UploadService(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}
		//----------------------------- Upload 1 hình -----------------------------
		public string UploadImageUrl(IFormFile imageFile)
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

		public string UploadImageString(IFormFile imageFile)
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

					return imagePath;
				}
			}

			return string.Empty;
		}

		//----------------------------- Upload nhiều hình -----------------------------
		public List<string> UploadImageMulti(List<IFormFile> imageFiles)
		{
			var imagePaths = new List<string>();
			foreach (var imageFile in imageFiles)
			{
				var imagePath = UploadImageString(imageFile);
				if (!string.IsNullOrEmpty(imagePath))
				{
					imagePaths.Add(imagePath);
				}
			}
			return imagePaths;
		}
		//------------------------------------------------------------------------------
		public void DeleteImage(string imagePath)
		{
			if (File.Exists(imagePath))
			{
				File.Delete(imagePath);
			}
		}

	}
}
