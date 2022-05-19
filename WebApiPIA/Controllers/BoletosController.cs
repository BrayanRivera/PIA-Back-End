using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("boletos")]
    public class BoletosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ClientesController> logger;

        public BoletosController(ApplicationDbContext context, IMapper mapper, ILogger<ClientesController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetBoletoDTO>>> Get()
        {
            logger.LogInformation("*****OBTENIENDO LOS BOLETOS*****");
            var boleto = await dbContext.Boletos.ToListAsync();
            return mapper.Map<List<GetBoletoDTO>>(boleto);
        }

        [HttpGet("obtenerClienteGanador/{numeroBoleto:int}", Name = "ObtenerClienteGanador")]
        public async Task<ActionResult<List<Boleto>>> Get(int numeroBoleto)
        {
            logger.LogInformation("*****OBTENIENDO CLIENTE GANADOR*****");
            var boleto = await dbContext.Boletos.FirstOrDefaultAsync(x => x.NumeroBoleto == numeroBoleto);
            if (boleto == null)
            {
                logger.LogError("*****NO SE ENCONTRO EL BOLETO*****");
                return NotFound("");
            }

            var clienteGanador = await dbContext.Clientes.FirstOrDefaultAsync(clienteDB => clienteDB.Id == boleto.ClienteID);
            if (clienteGanador == null)
            {
                logger.LogError("*****NO SE ENCONTRO CLIENTE ASCOCIADA AL BOLETO*****");
                return NotFound();
            }

            return Ok(clienteGanador);
        }

        [HttpPost]
        public async Task<ActionResult> Post(BoletoCreacionDTO boletoCreacionDTO)
        {
            logger.LogInformation("*****AGREGANDO BOLETO*****");
            var existeBoleto = await dbContext.Boletos.AnyAsync(x => x.NumeroBoleto == boletoCreacionDTO.NumeroBoleto);

            if (existeBoleto)
            {
                logger.LogError("*****NO SE PUEDE DUPLICAR BOLETO*****");
                return BadRequest($"Ya existe un boleto con el numero: {boletoCreacionDTO.NumeroBoleto}");
            }

            var boleto = mapper.Map<Boleto>(boletoCreacionDTO);
            dbContext.Add(boleto);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(BoletoCreacionDTO boletoCreacionDTO, int id)
        {
            logger.LogInformation("*****EDITANDO BOLETO*****");
            var exist = await dbContext.Boletos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                logger.LogError("*****NO EXISTE EL BOLETO A EDITAR*****");
                return NotFound();
            }

            var boleto = mapper.Map<Boleto>(boletoCreacionDTO);
            boleto.Id = id;

            if (boleto.Id != id)
            {
                return BadRequest("El id del cliente no coincide con el establecido en la url.");
            }

            dbContext.Update(boleto);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BoletoPatchDTO> patchDocument)
        {
            logger.LogInformation("*****EDITANDO BOLETO*****");
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var boleto = await dbContext.Boletos.FirstOrDefaultAsync(x => x.Id == id);
            if (boleto == null)
            {
                logger.LogError("*****NO EXISTE EL BOLETO A EDITAR*****");
                return NotFound();
            }

            var boletoDTO = mapper.Map<BoletoPatchDTO>(boleto);
            patchDocument.ApplyTo(boletoDTO, ModelState);
            var esValido = TryValidateModel(boletoDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(boletoDTO, boleto);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            logger.LogInformation("*****BORRANDO BOLETO*****");
            var exist = await dbContext.Boletos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                logger.LogError("*****NO EXISTE EL BOLETO A BORRAR*****");
                return NotFound("El boleto no fue encontrado.");
            }

            dbContext.Remove(new Boleto()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
