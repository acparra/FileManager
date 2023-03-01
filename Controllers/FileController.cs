using FileManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {

        #region Attributes

        private IConfiguration _configuration;
        private string _location;
        private static Dictionary<string, List<byte[]>> _fileSignatures = new Dictionary<string, List<byte[]>>()
        {
            { "png",
                new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            },
            { "jpg",
                new List<byte[]>
                {
                    new byte[] { 0xFF, 0XD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }
                }
            },
            { "jpeg",
                new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }
                }
            },
            { "tif",
                new List<byte[]>
                {
                    new byte[] { 0x49, 0x49, 0x2A, 0x00 },
                    new byte[] { 0x4D, 0x4D, 0x00, 0x2A }
                }
            },
            { "tiff",
                new List<byte[]>
                {
                    new byte[] { 0x49, 0x49, 0x2A, 0x00 },
                    new byte[] { 0x4D, 0x4D, 0x00, 0x2A }
                }
            },
            { "pdf",
                new List<byte[]>
                {
                    new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }
                }
            },
            { "xls",
                new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                }
            },
            { "doc",
                new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                }
            },
            { "ppt",
                new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                }
            },
            { "zip",
                new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 }
                }
            },
            { "docx",
                new List<byte[]>
                {
                    new byte[] { 0x50, 0x4b, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4b, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4b, 0x07, 0x08 }
                }
            },
            { "xlsx",
                new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 }
                }
            },
            { "pptx",
                new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 }
                }
            },
            { "gif",
                new List<byte[]>
                {
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }
                }
            },
            { "rar",
                new List<byte[]>
                {
                    new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 },
                    new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00 }
                }
            },
            { "txt",
                new List<byte[]>
                {
                    new byte[] { 0xEF, 0xBB, 0xBF },
                    new byte[] { 0xFF, 0xFE },
                    new byte[] { 0xFE, 0xFF },
                    new byte[] { 0xFF, 0xFE, 0x00, 0x00 },
                    new byte[] { 0x00, 0x00, 0xFE, 0xFF },
                    new byte[] { 0x2B, 0x2F, 0x76, 0x38 },
                    new byte[] { 0x2B, 0x2F, 0x76, 0x39 },
                    new byte[] { 0x2B, 0x2F, 0x76, 0x2B },
                    new byte[] { 0x2B, 0x2F, 0x76, 0x2F },
                    new byte[] { 0x0E, 0xFE, 0xFF }
                }
            },
            { "xml",
                new List<byte[]>
                {
                    new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20 },
                    new byte[] { 0x3C, 0x00, 0x3F, 0x00, 0x78, 0x00, 0x6D, 0x00, 0x6C, 0x00, 0x20 },
                    new byte[] { 0x00, 0x3C, 0x00, 0x3F, 0x00, 0x78, 0x00, 0x6D, 0x00, 0x6C, 0x00, 0x20 },
                    new byte[] { 0x3C, 0x00, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x78, 0x00, 0x00, 0x00, 0x6D, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00 },
                    new byte[] { 0x00, 0x00, 0x00, 0x3C, 0x00, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x78, 0x00, 0x00, 0x00, 0x6D, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x20 },
                    new byte[] { 0x4C, 0x6F, 0xA7, 0x94, 0x93, 0x40 }
                }
            }
        };
        
        #endregion

        #region Constructor

        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
            _location = _configuration.GetValue<string>("Path");
        }

        #endregion

        #region Endpoints

        [HttpGet]
        [Route("Ping")]
        public IActionResult Ping() => Ok("Pong");

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm] FileModel data)
        {
            data.Path = data.Path is null ? "" : data.Path;

            data.Path = data.Path.Trim();
            data.Path = data.Path.Replace("/", @"\");

            if (data.Path.StartsWith(@"\"))
                return BadRequest("Direccion invalida");

            if (!data.Path.EndsWith(@"\"))
                data.Path = data.Path + @"\";

            var route = $"{_location}{data.Path}";

            if (!IsFileValid(data.File))
                return BadRequest(new
                {
                    Status = "error",
                    Message = $"{data.File.FileName} file extension is not allowed.",
                });

            if (!Directory.Exists(route))
                Directory.CreateDirectory(route);

            try
            {
                using (var stream = new FileStream(route + data.File.FileName, FileMode.Create))
                {
                    await data.File.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        Status = "error",
                        Message = "An error has ocurred while saving the file",
                        Data = new
                        {

                        }
                    });
            }

            return Ok(new
            {
                Status = "success",
                Message = "The file was successfully loaded",
                Data = new
                {
                    Path = route,
                    File = data.File.FileName
                }
            });
        }
        
        [HttpDelete]

        public IActionResult DeleteFile([FromQuery] string path)
        {

            if (System.IO.File.Exists($"{path}"))
                System.IO.File.Delete(path);
            else
                return BadRequest(new
                {
                    Status = "error",
                    Message = "The specified file does not exist"
                });

            return Ok(new
            {
                Status = "success",
                Message = $"File was succesfully deleted",
                Path = path
            });
        }

        [HttpGet]
        [Route("Extensions")]

        public IActionResult GetValidExtensions()
        {
            var extensions = _fileSignatures.Select(key => key.Key).ToList();

            return Ok(new
            {
                Status = "success",
                Data = extensions
            });
        }

        [HttpGet]
        [Route("Download")]

        public IActionResult DownloadFile([FromQuery] string path)
        {

            byte[] file = System.IO.File.ReadAllBytes(path);

            return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
        }
        #endregion

        #region Functions

        public static bool IsFileValid(IFormFile file)
        {
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                byte[] headerBytes = reader.ReadBytes(_fileSignatures.Max(m => m.Value.Max(n => n.Length)));
                List<string> resultsKey = new List<string>();
                bool result = false;
                foreach (KeyValuePair<string, List<byte[]>> kvp in _fileSignatures)
                {
                    foreach (byte[] sequence in kvp.Value)
                    {
                        result = headerBytes.Take(sequence.Length).SequenceEqual(sequence);
                        if (result) break;
                    }
                    if (result) resultsKey.Add(kvp.Key);
                }
                string ext = file.FileName.Substring(file.FileName.LastIndexOf('.')).Replace(".", string.Empty);

                return resultsKey.Any(key => key == ext);
            }
        }

        #endregion


    }
}
