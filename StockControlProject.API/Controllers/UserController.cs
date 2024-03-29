using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockControlProject.Entities.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGenericService<User> _service;

        public UserController(IGenericService<User> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Login(string email, string password)
        {
            if (_service.Any(x => x.Email == email && x.Password == password))
            {
                User user = _service.GetByDefault(x => x.Email == email && x.Password == password);
                return Ok(user);
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreKullaniciGetir(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpPost]
        public IActionResult KullaniciEkle(User user)
        {
            if (_service.Add(user)) return CreatedAtAction("IdyeGoreKullaniciGetir", new { id = user.Id }, user);
            return BadRequest();
        }
    }
}
