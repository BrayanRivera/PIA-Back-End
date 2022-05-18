﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPIA.DTOs;
using WebApiPIA.Entidades;

namespace WebApiPIA.Controllers
{
    [ApiController]
    [Route("rifas")]
    public class RifasController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public RifasController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetRifaDTO>>> Get()
        {
            var rifa = await dbContext.Rifas.ToListAsync();
            return mapper.Map<List<GetRifaDTO>>(rifa);
        }

        [HttpGet("obtenerBoletoGanador/{numeroRifa:int}", Name = "ObtenerBoletoGanador")]
        public async Task<ActionResult<List<Rifa>>> Get(int numeroRifa)
        {
            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(x => x.NumeroRifa == numeroRifa);
            if (rifa == null)
            {
                return NotFound();
            }
                
            var existeBoleto = await dbContext.Boletos.AnyAsync(boletoDB => boletoDB.Id == rifa.Id);
            if (!existeBoleto)
            {
                return NotFound();
            }

            int count = await dbContext.Boletos.CountAsync(boletoDB => boletoDB.RifaID == rifa.Id).ConfigureAwait(false);
            Random r = new Random();
            int boletosRifaCount = r.Next(0, count);

            var boletoGanador = await dbContext.Boletos.Skip(boletosRifaCount).FirstOrDefaultAsync(boletoDB => boletoDB.RifaID == rifa.Id);

            return Ok(boletoGanador);

            //return await dbContext.Rifas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(RifaCreacionDTO rifaCreacionDTO)
        {

            var rifa = mapper.Map<Rifa>(rifaCreacionDTO);

            dbContext.Add(rifa);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(RifaCreacionDTO rifaCreacionDTO, int id)
        {
            var exist = await dbContext.Rifas.AnyAsync(x => x.Id == id);
            if (!exist)
            {
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
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<RifaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(x => x.Id == id);
            if (rifa == null)
            {
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
        public async Task<ActionResult> Delete(int id)
        {
            var existeRifa = await dbContext.Rifas.AnyAsync(x => x.Id == id);
            if (!existeRifa)
            {
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
