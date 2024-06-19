using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        public readonly ILogger<VillaAPIController> _logger;
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }



        [HttpGet]
        public async Task<IActionResult> GetVillas()
        {
            _logger.LogInformation("Getting all villas");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVilla(int id) 
        {
            if (id == 0) 
            {
                _logger.LogError($"problem with id {id}");
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null) 
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        public  async Task<IActionResult> CreateVilla([FromBody]VillaDto villaDto) 
        {
            if (villaDto == null) 
            {
                return BadRequest(villaDto);
            }
            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };
            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();
            return CreatedAtRoute(new {id=villaDto.Id}, villaDto);
        }

        [HttpDelete("{id:int}", Name ="DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0) 
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u=>u.Id == id);
            if (villa == null) 
            {
                return NotFound();
            }
             _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaDto villaDto)
        {
            if(villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };
            _db.Villas.Update(model);
           await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u=>u.Id==id);
            VillaDto villaDto = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };
           
            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villaDto, ModelState);
            Villa model = new Villa()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };
            _db.Villas.Update(model);
           await _db.SaveChangesAsync();
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
