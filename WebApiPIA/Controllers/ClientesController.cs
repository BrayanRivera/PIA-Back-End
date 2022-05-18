using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ClientesController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetClienteDTO>>> Get()
        {
            var cliente = await dbContext.Clientes.ToListAsync();
            return mapper.Map<List<GetClienteDTO>>(cliente);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ClienteCreacionDTO clienteCreacionDTO)
        {
            var cliente = mapper.Map<Cliente>(clienteCreacionDTO);

            dbContext.Add(cliente);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(ClienteCreacionDTO clienteCreacionDTO, int id)
        {
            var exist = await dbContext.Clientes.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            var cliente = mapper.Map<Cliente>(clienteCreacionDTO);
            cliente.Id = id;

            if (cliente.Id != id)
            {
                return BadRequest("El id del cliente no coincide con el establecido en la url.");
            }

            dbContext.Update(cliente);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<ClientePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDTO = mapper.Map<ClientePatchDTO>(cliente);
            patchDocument.ApplyTo(clienteDTO, ModelState);

            var esValido = TryValidateModel(clienteDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(clienteDTO, cliente);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existeCliente = await dbContext.Clientes.AnyAsync(x => x.Id == id);
            if (!existeCliente)
            {
                return NotFound("El cliente no fue encontrado.");
            }

            dbContext.Remove(new Cliente()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();

        }
    }
}
