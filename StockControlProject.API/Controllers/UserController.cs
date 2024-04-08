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

        [HttpGet]
        public IActionResult TumKullanicilariGetir()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet]
        public IActionResult AktifKullanicilariGetir()
        {
            return Ok(_service.GetActive());
        }

        [HttpPut("{id}")]
        public IActionResult KullaniciGuncelle(User user, int id)
        {
            if (id != user.Id) return BadRequest();
            try
            {
                if (_service.Update(user)) return Ok(user);
                return NotFound();
            }
            catch (Exception)
            {
                if (!UserExist(id)) return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult KullaniciSil(int id)
        {
            User user = _service.GetById(id);
            if(user is null) return NotFound();
            try
            {
                if (_service.Remove(user)) return Ok("Kayıt Silindi");
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest("Kayıt silinemedi");
            }
        }

        [HttpGet("{id}")]
        public IActionResult KullaniciAktiflestir(int id)
        {
            User user = _service.GetById(id);
            if(user is null) return NotFound();
            try
            {
                _service.Activate(id);
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private bool UserExist(int id)
        {
            return _service.Any(x => x.Id == id);
        }
    }
}
