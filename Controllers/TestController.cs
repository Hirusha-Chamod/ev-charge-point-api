using ev_charge_point_api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ev_charge_point_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public TestController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet("db-status")]
        public IActionResult GetDatabaseStatus()
        {
            try
            {
                var database = _mongoDBService.GetDatabase();
                // Try to list collections to test connection
                var collections = database.ListCollectionNames().ToList();

                return Ok(new
                {
                    status = "Connected to MongoDB Atlas",
                    database = database.DatabaseNamespace.DatabaseName,
                    collections_count = collections.Count,
                    collections = collections
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Connection failed",
                    error = ex.Message
                });
            }
        }
    }
}