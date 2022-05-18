using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("premios")]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public PremiosController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetPremioDTO>>> Get()
        {
            var premio = await dbContext.Premios.ToListAsync();
            return mapper.Map<List<GetPremioDTO>>(premio);
        }

        [HttpGet("premioDeRifa/{numeroRifa:int}", Name = "ObtenerPremio")]
        public async Task<ActionResult<List<Rifa>>> Get(int numeroRifa)
        {
            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(x => x.NumeroRifa == numeroRifa);
            if (rifa == null)
            {
                return NotFound();
            }

            var existePremio = await dbContext.Premios.AnyAsync(premioDB => premioDB.RifaId == rifa.Id);
            if (!existePremio)
            {
                return NotFound();
            }

            var premio = await dbContext.Premios.FirstOrDefaultAsync(premioDB => premioDB.RifaId == rifa.Id);

            return Ok(premio);

            //return await dbContext.Rifas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(PremioCreacionDTO premioCreacionDTO)
        {
            var premio = mapper.Map<Premio>(premioCreacionDTO);

            dbContext.Add(premio);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> Put(PremioCreacionDTO premioCreacionDTO, int id)
        {
            var exist = await dbContext.Premios.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            var premio = mapper.Map<Premio>(premioCreacionDTO);
            premio.Id = id;

            if (premio.Id != id)
            {
                return BadRequest("El id del premio no coincide con el establecido en la url.");
            }

            dbContext.Update(premio);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<PremioPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var premio = await dbContext.Premios.FirstOrDefaultAsync(x => x.Id == id);
            if (premio == null)
            {
                return NotFound();
            }

            var premioDTO = mapper.Map<PremioPatchDTO>(premio);
            patchDocument.ApplyTo(premioDTO, ModelState);

            var esValido = TryValidateModel(premioDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(premioDTO, premio);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Premios.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El premio no fue encontrado.");
            }

            dbContext.Remove(new Premio()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();

        }
    }
}
