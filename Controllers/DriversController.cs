using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using BLINK.Application.DTO;
using AutoMapper;

namespace MyApp.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class DriversController : ControllerBase
  {
    private readonly IDriverService _driverService;
    private readonly IMapper _mapper;

    public DriversController(IDriverService driverService, IMapper mapper)
    {
      _driverService = driverService;
      _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddDriver([FromBody] DriverDto driverDto)
    {
      var driver = _mapper.Map<Driver>(driverDto);
      await _driverService.AddDriverAsync(driver);
      var driverAddedDto = _mapper.Map<DriverDto>(driver);
      string location = Url.Action(nameof(GetDriver), new { id = driver.Id });

      return Created(location, driverAddedDto);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetDriver(int id)
    {
      var driver = await _driverService.GetDriverAsync(id);
      if (driver == null)
        return NotFound();

      var driverDto = _mapper.Map<DriverDto>(driver);
      return Ok(driverDto);
    }


    [HttpGet]
    public async Task<IActionResult> GetAllDrivers()
    {
      IEnumerable<Driver> drivers = await _driverService.GetAllDriversAsync();
      var driverDtos = _mapper.Map<IEnumerable<DriverDto>>(drivers);
      return Ok(driverDtos);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDriver(int id, [FromBody] DriverDto driverDto)
    {
      var existingDriver = await _driverService.GetDriverAsync(id);
      if (existingDriver == null)
        return NotFound();

      // Map the updated fields from the DTO to the existing driver entity
      _mapper.Map(driverDto, existingDriver);

      // Set the ID of the driver to be updated
      existingDriver.Id = id;

      await _driverService.UpdateDriverAsync(existingDriver);
      var updatedDriverDto = _mapper.Map<DriverDto>(existingDriver);

      return Ok(updatedDriverDto); 
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDriver(int id)
    {
      var driver = await _driverService.GetDriverAsync(id);
      if (driver == null)
        return NotFound();

      await _driverService.DeleteDriverAsync(id);
      return NoContent();
    }

    [HttpPost("AddRandom")]
public async Task<IActionResult> AddRandomDrivers()
{
    await _driverService.AddRandomDriversAsync();
    return Ok();
}

[HttpGet("Alphabetized")]
public async Task<IActionResult> GetAlphabetizedDrivers()
{
    var drivers = await _driverService.GetAlphabetizedDriversAsync();
      var driverDtos = _mapper.Map<IEnumerable<DriverDto>>(drivers);
      return Ok(driverDtos);
}

[HttpGet("AlphabetizedName/{id}")]
public async Task<IActionResult> GetAlphabetizedName(int id)
{
    var driver = await _driverService.GetDriverAsync(id);
    if (driver == null)
        return NotFound();

    var name = _driverService.GetAlphabetizedName(driver);
    return Ok(name);
}

  }
}
