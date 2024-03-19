using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        // Mock data for demonstration
        private readonly List<Entity> entities = new List<Entity>
        {
            new Entity { Id = "1", Gender = "Male", Deceased = false, Addresses = new List<Address>{ new Address { AddressLine = "123 Main St", City = "Anytown", Country = "USA" } }, Names = new List<Name>{ new Name { FirstName = "John", Surname = "Doe" } } },
            new Entity { Id = "2", Gender = "Female", Deceased = true, Addresses = new List<Address>{ new Address { AddressLine = "456 Elm St", City = "Othertown", Country = "Canada" } }, Names = new List<Name>{ new Name { FirstName = "Jane", Surname = "Smith" } } },
            // Add more mock data here
        };

        // GET: api/entities
        [HttpGet]
        public IActionResult GetEntities()
        {
            return Ok(entities);
        }

        // GET: api/entities/1
        [HttpGet("{id}")]
        public IActionResult GetEntity(string id)
        {
            var entity = entities.FirstOrDefault(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        // GET: api/entities/search?query=bob%20smith
        [HttpGet("search")]
        public IActionResult SearchEntities([FromQuery] string query)
        {
            var results = entities.Where(e =>
                (e.Names != null && e.Names.Any(n =>
                    (n.FirstName != null && n.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (n.MiddleName != null && n.MiddleName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (n.Surname != null && n.Surname.Contains(query, StringComparison.OrdinalIgnoreCase))
                )) ||
                (e.Addresses != null && e.Addresses.Any(a =>
                    (a.AddressLine != null && a.AddressLine.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (a.City != null && a.City.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (a.Country != null && a.Country.Contains(query, StringComparison.OrdinalIgnoreCase))
                ))
            ).ToList();
            return Ok(results == null ? "None" : results);
        }

        // GET: api/entities/filter?gender=Male&startDate=2000-01-01&endDate=2000-12-31&countries=USA,Canada
        [HttpGet("filter")]
        public IActionResult FilterEntities([FromQuery] string gender, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string countries)
        {
            if (entities == null)
            {
                return NotFound();
            }

            var filteredEntities = entities.Where(e =>
                (string.IsNullOrEmpty(gender) || e.Gender == gender) &&
                (!startDate.HasValue || e.Dates != null && e.Dates.Any(d => d.DateValue >= startDate)) &&
                (!endDate.HasValue || e.Dates != null && e.Dates.Any(d => d.DateValue <= endDate)) &&
                (string.IsNullOrEmpty(countries) || e.Addresses != null && countries.Split(',').Contains(e.Addresses.FirstOrDefault()?.Country))
            ).ToList();
            return Ok(filteredEntities == null ? "None" : filteredEntities);
        }
    }

    public class Entity
    {
        public List<Address> Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        public string Gender { get; set; }
        public string Id { get; set; }
        public List<Name> Names { get; set; }
    }

    public class Address
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class Date
    {
        public string DateType { get; set; }
        public DateTime DateValue { get; set; }
    }

    public class Name
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
    }
}
