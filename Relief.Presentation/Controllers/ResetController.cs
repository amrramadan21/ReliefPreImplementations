//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Relief.Domain.Contracts;
//using Relief.ServiceAbstraction.Interfaces.Profiles;
//using Shared.ProfileDTOs;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Relief.Presentation.Controllers
//{
//    [ApiController] // MISSING: Tells .NET this is an API controller
//    [Route("api/[controller]")] // MISSING: Sets the base URL to /api/reset
//    public class ResetController : ControllerBase
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        public ResetController(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//        [HttpDelete("reset-database")]
//        public async Task<IActionResult> NukeDatabase()
//        {
//            // This will take a few seconds as AWS drops and rebuilds the files
//            await _unitOfWork.RebuildDatabaseAsync();

//            return Ok("Database completely dropped and rebuilt with empty tables.");
//        }
//    }

//}
