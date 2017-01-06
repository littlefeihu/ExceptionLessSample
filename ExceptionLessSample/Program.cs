using Exceptionless;
using Exceptionless.Models;
using Exceptionless.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceptionLessSample
{
    class Program
    {
        static void Main(string[] args)
        {
            ExceptionlessClient.Default.Startup("enWbE4WjiEWl12CrSdpmOxIdYP7kpN59rKwTEBv7");
            ExceptionlessClient.Default.Configuration.ServerUrl = "http://localhost:8004";
            // Submit logs
            ExceptionlessClient.Default.SubmitLog("Logging made easy");

            // You can also specify the log source and log level.
            // We recommend specifying one of the following log levels: Trace, Debug, Info, Warn, Error
            ExceptionlessClient.Default.SubmitLog(typeof(Program).FullName, "This is so easy", "Info");
            ExceptionlessClient.Default.CreateLog(typeof(Program).FullName, "This is so easy", "Info").AddTags("Exceptionless").Submit();

            // Submit feature usages
            ExceptionlessClient.Default.SubmitFeatureUsage("MyFeature");
            ExceptionlessClient.Default.CreateFeatureUsage("MyFeature").AddTags("Exceptionless").Submit();
            ExceptionlessClient.Default.CreateFeatureUsage("MyFeature1").AddTags("Exceptionless1").Submit();
     
            // Submit a 404
            ExceptionlessClient.Default.SubmitNotFound("/somepage");
            ExceptionlessClient.Default.CreateNotFound("/somepage").AddTags("Exceptionless").Submit();

            // Submit a custom event type
            ExceptionlessClient.Default.SubmitEvent(new Event { Message = "Low Fuel", Type = "racecar3", Source = "Fuel System" });
            ExceptionlessClient.Default.SubmitEvent(new Event { Message = "Low Fuel", Type = "racecar1", Source = "Fuel System" });
            ExceptionlessClient.Default.SubmitEvent(new Event { Message = "Low Fuel", Type = "racecar2", Source = "Fuel System" });
          
            try
            {
                throw new ApplicationException(Guid.NewGuid().ToString());
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
            }
            try
            {
                throw new ApplicationException("Unable to create order from quote.");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                    // Set the reference id of the event so we can search for it later (reference:id).
                    // This will automatically be populated if you call ExceptionlessClient.Default.Configuration.UseReferenceIds();
                    .SetReferenceId(Guid.NewGuid().ToString("N"))
                    // Add the order object but exclude the credit number property.
                    .AddObject("dsa", "Order", excludedPropertyNames: new[] { "CreditCardNumber" }, maxDepth: 2)
                    // Set the quote number.
                    .SetProperty("Quote", 123)
                    // Add an order tag.
                    .AddTags("Order")
                    // Mark critical.
                    .MarkAsCritical()
                    // Set the coordinates of the end user.
                    .SetGeo(43.595089, -88.444602)
                    // Set the user id that is in our system and provide a friendly name.
                    .SetUserIdentity(new UserInfo { Identity = "Identity", Name = "allen" })
                    // Set the users description of the error.
                    .SetUserDescription("378917466@qq.com", "I tried creating an order from my saved quote.")
                    // Submit the event.
                    .Submit();
            }
            
        }
    }
}
