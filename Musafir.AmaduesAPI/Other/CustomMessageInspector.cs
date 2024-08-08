﻿using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;

namespace Musafir.AmaduesAPI.Other
{
    public class CustomMessageInspector : IClientMessageInspector
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomMessageInspector> _logger;
        public CustomMessageInspector(IConfiguration configuration, ILogger<CustomMessageInspector> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public object? BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            //custom headers 

            var actionHeader = MessageHeader.CreateHeader("Action", Constants.AddressingUrl, Constants.FlightSearchActionUrl);
            request.Headers.Add(actionHeader);

            var toHeader = MessageHeader.CreateHeader("To", Constants.AddressingUrl, _configuration["AmadeusConfiguration:Url"]);
            request.Headers.Add(toHeader);


            var messageIdHeader = MessageHeader.CreateHeader("MessageID", Constants.AddressingUrl, new UniqueId().ToString());
            request.Headers.Add(messageIdHeader);

            // Copy the message to a buffer so we can log it
            var buffer = request.CreateBufferedCopy(int.MaxValue);
            var copy = buffer.CreateMessage();

            using (var sw = new StringWriter())
            {
                using var writer = new XmlTextWriter(sw);
                copy.WriteMessage(writer);
                writer.Flush();
                var xml = sw.ToString();
                _logger.LogInformation("Request: " + xml);
            }

            // Use the buffered copy to proceed
            request = buffer.CreateMessage();
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Copy the message to a buffer so we can log it
            var buffer = reply.CreateBufferedCopy(int.MaxValue);
            var copy = buffer.CreateMessage();

            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    copy.WriteMessage(writer);
                    writer.Flush();
                    var xml = sw.ToString();
                    _logger.LogInformation("Response: " + xml);
                }
            }

            // Use the buffered copy to proceed
            reply = buffer.CreateMessage();
        }
    }
}