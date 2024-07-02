using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        public readonly ILogger<VillaNumberAPIController> _logger;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaNumberAPIController(ILogger<VillaNumberAPIController> logger, IMapper mapper, IVillaNumberRepository villaNumberRepository, IVillaRepository villaRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _response = new();
            _villaNumberRepository = villaNumberRepository;
            _villaRepository = villaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetVillaNumbers()
        {
            try
            {
                _logger.LogInformation("Getting all villas");
                List<VillaNumber> villaNumberList = await _villaNumberRepository.GetAllAsync(includeProperties: "Villa");
                _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting villas");
                var errorResponse = new
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>() { ex.Message }
                };
                return BadRequest(errorResponse);
            }

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError($"problem with id {id}");
                    return BadRequest();
                }
                var villaNumber = await _villaNumberRepository.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting villas");
                var errorResponse = new
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>() { ex.Message }
                };
                return BadRequest(errorResponse);
            }

        }

        // [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateVillaNumber([FromBody] VillaNumberCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest(createDto);
                }
                if (createDto.VillaNo < 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                if (await _villaNumberRepository.GetAsync(u => u.VillaNo == createDto.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already Exists");
                    return BadRequest(ModelState);
                }

                if (await _villaRepository.GetAsync(u => u.Id == createDto.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid");
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDto);
                await _villaNumberRepository.CreateAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute(new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting villas");
                var errorResponse = new
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>() { ex.Message }
                };
                return BadRequest(errorResponse);
            }

        }
        // [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        public async Task<IActionResult> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villaNumber = await _villaNumberRepository.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    return NotFound();
                }
                await _villaNumberRepository.RemoveAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting villas");
                var errorResponse = new
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>() { ex.Message }
                };
                return BadRequest(errorResponse);
            }
        }
        //  [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        public async Task<IActionResult> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null || id != updateDto.VillaNo)
                {
                    return BadRequest();
                }
                if (await _villaRepository.GetAsync(u => u.Id == updateDto.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid");
                }
                VillaNumber model = _mapper.Map<VillaNumber>(updateDto);

                await _villaNumberRepository.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting villas");
                var errorResponse = new
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>() { ex.Message }
                };
                return BadRequest(errorResponse);
            }
        }
    }
}
