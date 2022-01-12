using dgWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dgWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ClientesController : ControllerBase
    {
        static List<Cliente> _clientes;
        [HttpGet]
        public List<Cliente> Get()
        {

            return _clientes;
        }
        [HttpPost]
        public void Post([FromBodyAttribute] Cliente _cli)
        {
            if (_clientes == null)
            {
                _clientes = new List<Cliente>();
            }
            _clientes.Add(_cli);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            if (id != 0)
            {
                _clientes.RemoveAt(_clientes.IndexOf(_clientes.Find(x => x.Id == id)));
            }
        }

    }
   
}
