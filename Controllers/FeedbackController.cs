﻿using Microsoft.AspNetCore.Mvc;
using ModernWallet.Models;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace ModernWallet.Controllers
{
    [Route("api/[controller]")]
    public class FeedbackController : BaseController
    {
        public FeedbackController(IHostingEnvironment envrnmt) : base(envrnmt) { }

        // POST api/feedback
        [HttpPost]
        public IActionResult Post([FromForm]FeedbackModel feedback)
        {
            // This fields must not have any value (robots detection). 
            if (!string.IsNullOrEmpty(feedback.Dummy) || !ModelState.IsValid)
            {
                var errors = ModelState.Where(s => s.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                    .ToDictionary(
                    k => k.Key,
                    v => string.Join(" / ", v.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    ));

  
                return NotFound(errors);
            }

            EmailSender.SendFeedback(feedback);
            AzureStorageHelper.Store(feedback);

            return Ok(ApplicationSettings.Configuration["Email:Messages:Feedback"]);
        }

    }
}
