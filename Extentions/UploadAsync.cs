namespace FrontToBackMvc.Extentions
{
    public static class FileExtension
    {
        public static async Task<string> UploadAsync(this IFormFile file, string rootPath, params string[] subPaths)
        {

            string uploadPath = Path.Combine(new[] { rootPath }.Concat(subPaths).ToArray());


            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }


            string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

            string filePath = Path.Combine(uploadPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //mende bir canliyim, anlasabiliriz👉🏼👈🏼 hadi anlasdikkkkk
            return fileName;
        }

    }
}
