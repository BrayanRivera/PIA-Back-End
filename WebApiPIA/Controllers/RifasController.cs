using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("rifas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RifasController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ClientesController> logger;

        public RifasController(ApplicationDbContext context, IMapper mapper, ILogger<ClientesController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetRifaDTO>>> Get()
        {
            logger.LogInformation("*****OBTENIENDO LAS RIFAS*****");
            var rifa = await dbContext.Rifas.ToListAsync();
            return mapper.Map<List<GetRifaDTO>>(rifa);
        }

        [HttpGet("obtenerBoletoGanador/{numeroRifa:int}", Name = "ObtenerBoletoGanador")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult<List<Rifa>>> Get(int numeroRifa)
        {
            logger.LogInformation("*****OBTENIENDO EL BOLETO GANADOR*****");
            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(x => x.NumeroRifa == numeroRifa);
            if (rifa == null)
            {
                logger.LogError("*****NO SE ENCONTRO RIFA ASOCIADO AL BOLETO*****");
                return NotFound();
            }
                
            var existeBoleto = await dbContext.Boletos.AnyAsync(boletoDB => boletoDB.Id == rifa.Id);
            if (!existeBoleto)
            {
                logger.LogError("*****NO SE ENCONTRO EL BOLETO SOLICITADO*****");
                return NotFound();
            }

            int boletosRifaCount = await dbContext.Boletos.CountAsync(boletoDB => boletoDB.RifaID == rifa.Id).ConfigureAwait(false);
            Random r = new Random();
            int numeroRandom = r.Next(0, boletosRifaCount);
            var boletoGanador = await dbContext.Boletos.Skip(numeroRandom).FirstOrDefaultAsync(boletoDB => boletoDB.RifaID == rifa.Id);
            return Ok(boletoGanador);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> Post(RifaCreacionDTO rifaCreacionDTO)
        {
            logger.LogInformation("*****AGREGANDO LA RIFA*****");
            var existeRifa = await dbContext.Rifas.AnyAsync(x => x.NumeroRifa == rifaCreacionDTO.NumeroRifa);

            if (existeRifa)
            {
                logger.LogError("*****NO SE PUEDE DUPLICAR LA RIFA*****");
                return BadRequest($"Ya existe una rifa con el numero {rifaCreacionDTO.NumeroRifa}");
            }


            var rifa = mapper.Map<Rifa>(rifaCreacionDTO);
            dbContext.Add(rifa);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> Put(RifaCreacionDTO rifaCreacionDTO, int id)
        {
            logger.LogInformation("*****EDITANDO LA RIFA*****");
            var exist = await dbContext.Rifas.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                logger.LogError("*****NO SE PUEDE ENCONTRAR LA RIFA A EDITAR*****");
                return NotFound();
            }

            var rifa = mapper.Map<Rifa>(rifaCreacionDTO);
            rifa.Id = id;

            if (rifa.Id != id)
            {
                return BadRequest("El id del cliente no coincide con el establecido en la url.");
            }

            dbContext.Update(rifa);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<RifaPatchDTO> patchDocument)
        {
            logger.LogInformation("*****EDITANDO LA RIFA*****");
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(x => x.Id == id);
            if (rifa == null)
            {
                logger.LogError("*****NO SE PUEDE ENCONTRAR LA RIFA A EDITAR*****");
                return NotFound();
            }

            var rifaDTO = mapper.Map<RifaPatchDTO>(rifa);
            patchDocument.ApplyTo(rifaDTO, ModelState);
            var esValido = TryValidateModel(rifaDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(rifaDTO, rifa);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            logger.LogInformation("*****BORRANDO LA RIFA*****");
            var existeRifa = await dbContext.Rifas.AnyAsync(x => x.Id == id);
            if (!existeRifa)
            {
                logger.LogError("*****NO SE PUEDE ENCONTRAR LA RIFA A BORRAR*****");
                return NotFound("La rifa no fue encontrada.");
            }

            dbContext.Remove(new Rifa()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
