//using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        //private readonly IMapper mapper;

        public ClientesController(ApplicationDbContext context/*, IMapper mapper*/)
        {
            this.dbContext = context;
            //this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> Get()
        {
            return await dbContext.Clientes.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Cliente cliente)
        {
            dbContext.Add(cliente);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Cliente cliente, int id)
        {
            var exist = await dbContext.Clientes.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            if (cliente.Id != id)
            {
                return BadRequest("El id del cliente no coincide con el establecido en la url.");
            }

            dbContext.Update(cliente);
            await dbContext.SaveChangesAsync();
            return Ok();
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
