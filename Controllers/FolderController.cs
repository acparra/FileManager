using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {

        #region Attributes

        private IConfiguration _configuration;
        private string _location;

        #endregion

        #region Constructor

        public FolderController(IConfiguration configuration)
        {
            _configuration = configuration;
            _location = _configuration.GetValue<string>("Path");
        }

        #endregion

        #region Endpoints

        [HttpPost]

        public IActionResult CreateDirectory([FromForm] string path)
        {
            path = path.Replace("/", @"\");
            try
            {
                if (Directory.Exists(_location + path))
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "The folder already exists",
                    });
                Directory.CreateDirectory(_location + path);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "error",
                    Message = "The folder could not be created",
                    Data = new
                    {

                    }
                });
            }

            return Ok(new
            {
                Status = "success",
                Message = "The folder was successfully created",
                Data = new
                {
                    Directory = _location + path
                }
            });
        }

        [HttpGet]

        public IActionResult GetContent([FromQuery] string? path)
        {
            path = path ?? string.Empty;
            path = path.Length > 0 && path[0] == '/' ? path.Substring(1) : path;
            path = path.Replace("/", @"\");
            string[] files;
            string[] folders;
            try
            {
                files = Directory.GetFiles(_location + path);
                folders = Directory.GetDirectories(_location + path);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "error",
                    Message = "An error has ocurred while getting the content",
                    Data = new
                    {

                    }
                });
            }

            return Ok(new
            {
                Status = "success",
                Message = "Content gotten successfully",
                Data = new
                {
                    Files = files,
                    Folders = folders
                }
            });
        }

        [HttpDelete]

        public IActionResult DeleteFolder([FromQuery] string path)
        {
            if (Directory.Exists($"{path}"))
                Directory.Delete($"{path}", true);
            else
                return BadRequest(new
                {
                    Status = "error",
                    Message = "The specified folder does not exist"
                });


            return Ok(new
            {
                Status = "success",
                Message = "Folder and its content was successfully deleted"
            });
        }

        #endregion

    }
}
