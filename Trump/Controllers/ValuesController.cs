using System.Collections.Generic;
using System.Web.Http;
using Trump.Models;

namespace Trump.Controllers
{
    public class ValuesController : ApiController
    {
        TrumpEntities db = new TrumpEntities();
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        public string Approve(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
