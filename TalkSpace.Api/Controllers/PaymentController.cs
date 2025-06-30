using Application.Abstractions;
using Application.DTOs.Requests.PaymentRequests;
using Application.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace TalkSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService _paymentService, ILogger<PaymentController> logger)
        {
            paymentService = _paymentService;
            _logger = logger;
        }
        [HttpPost("Checkout")]
        public async Task<IActionResult> CreatePaymentSession([FromBody]SessionBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid) { 
                    return BadRequest(ModelState);
                }
                var result=await paymentService.PayForSessionAsync(request);
                return Ok(new
                {
                    sessionId = request.SessionId,
                    Amount = result.Amount,
                    Currency = result.Currency,
                    PaymentIntentId = result.PaymentIntentId,
                    ClientSecretCredential = result.ClientSecret,
                    Status = "Payment intent created successfully"
                });

                
            }
            catch (StripeException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.StripeError?.Message ?? "Payment processing error");
            }
            catch (Exception ex)
            {
                // Log the exception using the injected logger
                _logger.LogError(ex, "An unexpected error occurred while processing the payment.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook()
        {

            // Read the body first and store it
            try
            {
                // Read the body first and store it
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

                if (!Request.Headers.TryGetValue("Stripe-Signature", out var stripeSignature))
                {
                    _logger.LogWarning("Stripe-Signature header not found");
                    return BadRequest("Missing Stripe-Signature header");
                }

                _logger.LogDebug("Received webhook with headers: {Headers}", Request.Headers);

                try
                {
                    if(string.IsNullOrEmpty(stripeSignature))
                    {
                        _logger.LogWarning("Received empty webhook stripeSignature");
                        return BadRequest("Empty webhook stripeSignature");
                    }
                    await paymentService.HandlePaymentConfirmationAsync(json, stripeSignature);

                    _logger.LogInformation("Payment webhook processed successfully");
                    return Ok(new { received = true, message = "the payment webhook has captured successfully" });
                }
                catch (StripeException ex)
                {

                    _logger.LogError(ex, "Stripe webhook validation failed: {Message}", ex.Message);
                    return BadRequest(new { error = "Webhook validation failed", message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    // Handle invalid arguments (malformed JSON, missing required fields, etc.)
                    _logger.LogError(ex, "Invalid webhook payload: {Message}", ex.Message);
                    return BadRequest(new { error = "Invalid payload", message = ex.Message });
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook processing failed: {ErrorMessage}", ex.StripeError?.Message);
                return BadRequest(); // Return 400 for Stripe signature verification failures
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing failed");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
