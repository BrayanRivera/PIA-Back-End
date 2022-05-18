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

        public BoletosController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetBoletoDTO>>> Get()
        {
            var boleto = await dbContext.Boletos.ToListAsync();
            return mapper.Map<List<GetBoletoDTO>>(boleto);
        }

        [HttpGet("obtenerClienteGanador/{numeroBoleto:int}", Name = "ObtenerClienteGanador")]
        public async Task<ActionResult<List<Boleto>>> Get(int numeroBoleto)
        {
            var boleto = await dbContext.Boletos.FirstOrDefaultAsync(x => x.NumeroBoleto == numeroBoleto);
            if (boleto == null)
            {
                return NotFound();
            }

            var clienteGanador = await dbContext.Clientes.FirstOrDefaultAsync(clienteDB => clienteDB.Id == boleto.ClienteID);

            return Ok(clienteGanador);

            //return await dbContext.Rifas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(BoletoCreacionDTO boletoCreacionDTO)
        {
            var boleto = mapper.Map<Boleto>(boletoCreacionDTO);

            dbContext.Add(boleto);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(BoletoCreacionDTO boletoCreacionDTO, int id)
        {
            var exist = await dbContext.Boletos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
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
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var boleto = await dbContext.Boletos.FirstOrDefaultAsync(x => x.Id == id);
            if (boleto == null)
            {
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
            var exist = await dbContext.Boletos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
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
