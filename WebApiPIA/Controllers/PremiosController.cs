//using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("premios")]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        //private readonly IMapper mapper;

        public PremiosController(ApplicationDbContext context/*, IMapper mapper*/)
        {
            this.dbContext = context;
            //this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Premio>>> Get()
        {
            return await dbContext.Premios.ToListAsync();
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
        public async Task<ActionResult> Post(Premio premio)
        {
            dbContext.Add(premio);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> Put(Premio premio, int id)
        {
            var exist = await dbContext.Premios.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            if (premio.Id != id)
            {
                return BadRequest("El id del premio no coincide con el establecido en la url.");
            }

            dbContext.Update(premio);
            await dbContext.SaveChangesAsync();
            return Ok();
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
