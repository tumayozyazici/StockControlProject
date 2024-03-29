using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Entities.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IGenericService<Product> _service;

        public ProductController(IGenericService<Product> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult TumUrunleriGetir()
        {
            return Ok(_service.GetAll(x=>x.Category,y=>y.Supplier));
        }

        [HttpGet]
        public IActionResult TumAktifUrunleriGetir()
        {
            return Ok(_service.GetActive(x => x.Category, y => y.Supplier));
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreUrunleriGetir(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpPost]
        public IActionResult UrunEkle(Product product)
        {
            _service.Add(product);
            return CreatedAtAction("IdyeGoreUrunleriGetir", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UrunGuncelle(Product product,int id)
        {
            if (id != product.Id) return BadRequest();
            try
            {
                if (_service.Update(product)) return Ok(product);
                return NotFound();
            }
            catch (Exception)
            {
                if (!ProductExist(id)) return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult UrunSil(int id)
        {
            Product product = _service.GetById(id);
            if (product is null) return NotFound();
            try
            {
                _service.Remove(product);
                return Ok("Ürün silindi");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult UrunAktifleştir(int id)
        {
            Product product = _service.GetById(id);
            if (product is null) return NotFound();
            try
            {
                _service.Activate(id);
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private bool ProductExist(int id)
        {
            return _service.Any(x => x.Id == id);
        }
    }
}
