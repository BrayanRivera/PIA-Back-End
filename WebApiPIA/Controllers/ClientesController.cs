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
    [Route("clientes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ClientesController> logger;

        public ClientesController(ApplicationDbContext context, IMapper mapper, ILogger<ClientesController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetClienteDTO>>> Get()
        {
            logger.LogInformation("*****OBTENIENDO LOS CLIENTES*****");
            var clientes = await dbContext.Clientes.ToListAsync();
            return mapper.Map<List<GetClienteDTO>>(clientes);
        }

        [HttpGet("{id:int}", Name = "obtenerCliente")]
        public async Task<ActionResult<GetClienteDTO>> Get(int id)
        {
            logger.LogInformation("*****OBTENIENDO EL CLIENTE*****");
            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(x => x.Id == id);

            if (cliente == null)
            {
                logger.LogError("*****NO SE ENCONTRO EL CLIENTE SOLICITADO*****");
                return NotFound();
            }

            return mapper.Map<GetClienteDTO>(cliente);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ClienteCreacionDTO clienteCreacionDTO)
        {
            logger.LogInformation("*****AGREGANDO CLIENTE NUEVO*****");
            var existeCliente = await dbContext.Clientes.AnyAsync(x => x.NumeroCliente == clienteCreacionDTO.NumeroCliente);

            if (existeCliente)
            {
                logger.LogError("*****NO SE PUEDE DUPLICAR EL CLIENTE*****");
                return BadRequest($"Ya existe un cliente con el numero: {clienteCreacionDTO.NumeroCliente}");
            }

            var cantidadDeClientes = await dbContext.Clientes.CountAsync();
            if(cantidadDeClientes >= 54)
            {
                logger.LogError("*****YA SE TIENE EL LIMITE DE CLIENTES (54)*****");
                return BadRequest($"Ya se alcanzo el limite de clientes participantes (54 participantes)");
            }

            var cliente = mapper.Map<Cliente>(clienteCreacionDTO);
            dbContext.Add(cliente);
            await dbContext.SaveChangesAsync();
            var clienteDTO = mapper.Map<GetClienteDTO>(cliente);
            return CreatedAtRoute("obtenerCliente", new { id = cliente.Id }, clienteDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(ClienteCreacionDTO clienteCreacionDTO, int id)
        {
            logger.LogInformation("*****EDITANDO CLIENTE*****");
            var exist = await dbContext.Clientes.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                logger.LogError("*****NO EXISTE EL CLIENTE A EDITAR*****");
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
            logger.LogInformation("*****EDITANDO CLIENTE*****");
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            if (cliente == null)
            {
                logger.LogError("*****NO EXISTE EL CLIENTE A EDITAR*****");
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
            logger.LogInformation("*****BORRANDO CLIENTE*****");
            var existeCliente = await dbContext.Clientes.AnyAsync(x => x.Id == id);
            if (!existeCliente)
            {
                logger.LogError("*****NO EXISTE EL CLIENTE A BORRAR*****");
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
