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
        private readonly ILogger<ClientesController> logger;

        public PremiosController(ApplicationDbContext context, IMapper mapper, ILogger<ClientesController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetPremioDTO>>> Get()
        {
            logger.LogInformation("*****OBTENIENDO LOS PREMIOS*****");
            var premio = await dbContext.Premios.ToListAsync();
            return mapper.Map<List<GetPremioDTO>>(premio);
        }

        [HttpGet("premioDeRifa/{numeroRifa:int}", Name = "ObtenerPremio")]
        public async Task<ActionResult<List<Rifa>>> Get(int numeroRifa)
        {
            logger.LogInformation("*****OBTENIENDO EL PREMIO*****");
            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(x => x.NumeroRifa == numeroRifa);
            if (rifa == null)
            {
                logger.LogError("*****NO SE ENCONTRO PREMIO ASCOCIADA A LA RIFA*****");
                return NotFound();
            }

            var existePremio = await dbContext.Premios.AnyAsync(premioDB => premioDB.RifaId == rifa.Id);
            if (!existePremio)
            {
                logger.LogError("*****NO SE ENCONTRO EL PREMIO SOLICITADO*****");
                return NotFound();
            }

            var premio = await dbContext.Premios.FirstOrDefaultAsync(premioDB => premioDB.RifaId == rifa.Id);
            return Ok(premio);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PremioCreacionDTO premioCreacionDTO)
        {
            logger.LogInformation("*****AGREGANDO EL PREMIO*****");
            var existePremio = await dbContext.Premios.AnyAsync(x => x.NombrePremio == premioCreacionDTO.NombrePremio);

            if (existePremio)
            {
                logger.LogError("*****NO SE PUEDE DUPLICAR EL PREMIO*****");
                return BadRequest($"Ya existe el premio: {premioCreacionDTO.NombrePremio}");
            }

            var premio = mapper.Map<Premio>(premioCreacionDTO);
            dbContext.Add(premio);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> Put(PremioCreacionDTO premioCreacionDTO, int id)
        {
            logger.LogInformation("*****EDITANDO EL PREMIO*****");
            var exist = await dbContext.Premios.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                logger.LogError("*****NO SE ENCONTRO EL PREMIO A EDITAR*****");
                return NotFound();
            }

            var premio = mapper.Map<Premio>(premioCreacionDTO);
            premio.Id = id;

            if (premio.Id != id)
            {
                logger.LogError("*****NO SE ENCONTRO EL PREMIO A EDITAR*****");
                return BadRequest("El id del premio no coincide con el establecido en la url.");
            }

            dbContext.Update(premio);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<PremioPatchDTO> patchDocument)
        {
            logger.LogInformation("*****EDITANDO EL PREMIO*****");
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var premio = await dbContext.Premios.FirstOrDefaultAsync(x => x.Id == id);
            if (premio == null)
            {
                logger.LogError("*****NO SE ENCONTRO EL PREMIO A EDITAR*****");
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
            logger.LogInformation("*****BORRANDO EL PREMIO*****");
            var exist = await dbContext.Premios.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                logger.LogError("*****NO SE ENCONTRO EL PREMIO A BORRAR*****");
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
