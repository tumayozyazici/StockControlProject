using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockControlProject.Entities.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IGenericService<Supplier> _service;

        public SupplierController(IGenericService<Supplier> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult TumTedarikcileriGetir()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet]
        public IActionResult TumAktifTedarikcileriGetir()
        {
            return Ok(_service.GetActive());
        }
        [HttpGet("{id}")]
        public IActionResult IdyeGoreTedarikciGetir(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpPost]
        public IActionResult TedarikciEkle(Supplier supplier)
        {
            if (_service.Add(supplier)) return CreatedAtAction("IdyeGoreTedarikciGetir", new { id = supplier.Id }, supplier);
            return BadRequest();
        }

        [HttpPut("{id}")]
        public IActionResult TedarikciGuncelle(int id, Supplier supplier)
        {
            if (id != supplier.Id) return BadRequest();
            try
            {
                if (_service.Update(supplier)) return Ok(supplier);
                return BadRequest();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public IActionResult TedarikciSil(int id)
        {
            var supplier = _service.GetById(id);
            if (supplier is null) return NotFound();
            if (_service.Remove(supplier)) return Ok("Tedarikçi Silindi");
            return BadRequest();
        }

        [HttpGet("{id}")]
        public IActionResult TedarikciAktiflestir(int id)
        {
            Supplier s = _service.GetById(id);
            if (s is null) return NotFound();
            _service.Activate(id);
            return Ok(s);
        }
    }
}
