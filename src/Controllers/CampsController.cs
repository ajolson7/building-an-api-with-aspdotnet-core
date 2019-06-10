﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    // if you specify certain api versions that are accepted (in this case 1.0 and 1.1) but you only
        // have one version of an endpoint, each api version that is accepted will just use the same
        // version of the endpoint
    // if you specify an api version that isn't accepted (like 2.0 in this case), you'll get an error
        // message
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        // where do repository and mapper come from in the constructor here?
        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        // how is _mapper.Map connected to the line "services.AddAutoMapper();" in the "Startup.cs" file
            // and the CampProfile.cs file that creates the mapping with the line
            // "this.CreateMap<Camp, CampModel>();"?
        // the variable "includeTalks" is a query string
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        // {moniker} is bound to the string moniker in the Get (Get(string moniker))
            // the variable moniker in {moniker} and Get(string moniker) must be the same; you can't
            // do {moniker} and Get(string blah) for example
        // if Get(string moniker) was instead Get(int moniker), do this: [HttpGet("{moniker:int}")]
        // given the url "http://localhost:6600/api/camps/ATL2018", the "ATL2018" part is what is
            // bound to the variable "moniker"; the variable "moniker" is a query string
        [HttpGet("{moniker}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        [MapToApiVersion("1.1")]
        public async Task<ActionResult<CampModel>> Get11(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker, true);

                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        // [HttpGet("search/{theDate}")]
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        // public async Task<ActionResult<CampModel>> Post([FromBody]CampModel model)
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existing = await _repository.GetCampAsync(model.Moniker);

                if (existing != null)
                {
                    return BadRequest("Moniker in use");
                }

                // the link generator is just providing/generating a URL for a given Get request; this
                    // is important because when you do an http post/when you create something on
                    // your database, it will be a single item/model in your database which you need
                    // to be able to reach through a Get request endpoint in your API
                // the link generator "GetPathByAction" function requires the name of the route you
                    // want to generate a link or URL for, the controller that route belongs to, and
                    // an object of any route values for the route (required parameters for the route,
                    // e.g. includeTalks)
                
                // can link generator provide/generate a link with a query string in it?
                var location = _linkGenerator.GetPathByAction("Get",
                    "Camps",
                    new { moniker = model.Moniker }
                    );

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                // Create a new Camp
                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);

                if (await _repository.SaveChangesAsync())
                {
                    // Created requires you to say (with a status code of 201) that you created a new
                    // object, and it requires you to provide a location, which is a URI for getting
                    // the new object you just created from the server
                    return Created(location, model);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);

                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                // model = from, oldCamp = destination
                _mapper.Map(model, oldCamp);

                if (await _repository.SaveChangesAsync())
                {
                    return model;
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);

                if (oldCamp == null) return NotFound();

                _repository.Delete(oldCamp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Failed to delete the camp");
        }
    }
}