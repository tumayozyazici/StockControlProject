using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockControlProject.Entities.Entities;
using StockControlProject.Entities.Enums;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IGenericService<Order> _serviceOrder;
        private readonly IGenericService<OrderDetail> _serviceOrderDetail;
        private readonly IGenericService<Product> _serviceProduct;

        public OrderController(IGenericService<Order> service, IGenericService<OrderDetail> serviceOrderDetail, IGenericService<Product> serviceProduct)
        {
            _serviceOrder = service;
            _serviceOrderDetail = serviceOrderDetail;
            _serviceProduct = serviceProduct;
        }

        [HttpGet]
        public IActionResult TumSiparisleriGetir()
        {
            return Ok(_serviceOrder.GetAll(x => x.User, y => y.OrderDetails));
        }

        [HttpGet]
        public IActionResult AktifSiparisleriGetir()
        {
            return Ok(_serviceOrder.GetActive());
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreSiparisleriGetir(int id)
        {
            return Ok(_serviceOrder.GetById(id, x => x.OrderDetails, y => y.User));
        }

        [HttpGet("{id}")]
        public IActionResult IdyeSiparislerinDetaylariniGetir(int id)
        {
            return Ok(_serviceOrderDetail.GetAll(x => x.OrderId == id, y => y.Product));
        }

        [HttpGet]
        public IActionResult BekleyenSiparsileriGetir()
        {
            return Ok(_serviceOrder.GetDefault(x => x.Status == Status.Pending));
        }

        [HttpGet]
        public IActionResult OnaylananSiparsileriGetir()
        {
            return Ok(_serviceOrder.GetDefault(x => x.Status == Status.Confirmed));
        }

        [HttpGet]
        public IActionResult IptalEdilenSiparsileriGetir()
        {
            return Ok(_serviceOrder.GetDefault(x => x.Status == Status.Cancelled));
        }

        [HttpPost]
        public IActionResult SiparisOlustur(int userId, [FromQuery] int[] productIds, [FromQuery] short[] quantities)
        {
            Order newOrder = new Order();
            newOrder.UserId = userId;
            newOrder.Status = Status.Pending;
            newOrder.isActive = true;
            _serviceOrder.Add(newOrder);

            for (int i = 0; i < productIds.Length; i++)
            {
                OrderDetail newOrderDetail = new OrderDetail();
                newOrderDetail.OrderId = newOrder.Id;
                newOrderDetail.ProductId = productIds[i];
                newOrderDetail.Quantity = quantities[i];
                newOrderDetail.UnitPrice = _serviceProduct.GetById(productIds[i]).UnitPrice * newOrderDetail.Quantity;
                newOrderDetail.isActive = true;
                _serviceOrderDetail.Add(newOrderDetail);
            }
            return Ok(newOrder);
        }

        [HttpGet("{id}")]
        public IActionResult SiparsiOnayla(int id)
        {
            Order order = _serviceOrder.GetById(id);
            if (order is null) return NotFound();
            else
            {
                List<OrderDetail> orderdetails = _serviceOrderDetail.GetDefault(x => x.OrderId == order.Id);
                foreach (OrderDetail item in orderdetails)
                {
                    Product productInOrder = _serviceProduct.GetById(item.ProductId);
                    if (productInOrder.Stock > item.Quantity)
                    {
                        productInOrder.Stock -= item.Quantity;
                        _serviceProduct.Update(productInOrder);
                    }
                    else return BadRequest();
                }
                order.Status = Status.Confirmed;
                order.isActive = false;
                _serviceOrder.Update(order);
                return Ok(order);
            }
        }

        [HttpGet("{id}")]
        public IActionResult SiparisiReddet(int id)
        {
            Order canceledOrder = _serviceOrder.GetById(id);
            if (canceledOrder is null) return NotFound();

            canceledOrder.Status = Status.Cancelled;
            canceledOrder.isActive = false;
            _serviceOrder.Update(canceledOrder);
            return Ok(canceledOrder);
        }
    }
}