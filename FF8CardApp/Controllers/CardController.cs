using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FF8CardApp.Model;
using LiteDB;

namespace FF8CardApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        static LiteDatabase db = new LiteDatabase(@"FF8.db");
        Random rnd = new Random();
        private readonly ILogger<CardController> _logger;

        public CardController(ILogger<CardController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Card> Get()
        {
            var enemiesCol = db.GetCollection<Card>("cards");

            var retVal = enemiesCol.FindAll().OrderBy(x => rnd.Next()).Take(5).ToList();
            return retVal;
        }
    }
}
