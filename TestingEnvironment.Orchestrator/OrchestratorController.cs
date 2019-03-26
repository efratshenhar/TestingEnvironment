﻿using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using TestingEnvironment.Common;

// ReSharper disable VirtualMemberCallInConstructor

namespace TestingEnvironment.Orchestrator
{    
    public class OrchestratorController : NancyModule
    {
        private readonly OrchestratorConfiguration _config;
        private static readonly object Empty = new object();

        public OrchestratorController(OrchestratorConfiguration config)
        {
            
            _config = config;
            Put("/register", @params => 
                Orchestrator.Instance.RegisterTest(
                    Uri.UnescapeDataString((string) Request.Query.testName), 
                    Uri.UnescapeDataString((string) Request.Query.testClassName),
                    Uri.UnescapeDataString((string) Request.Query.author)));

            Put("/unregister", @params =>
            {
                Orchestrator.Instance.UnregisterTest(Uri.UnescapeDataString((string) Request.Query.testName));
                return Empty;
            });

            Post("/report", @params => Orchestrator.Instance.ReportEvent(Uri.UnescapeDataString((string) Request.Query.testName), this.Bind<EventInfo>()));
            
            //get latest test by name
            Get<dynamic>("/latest-tests", @params => 
                Response.AsJson(Orchestrator.Instance.GetLastTestByName(Uri.UnescapeDataString((string) Request.Query.testName))));

            Get<dynamic>("/config-selectors",_ =>
                Response.AsJson(Orchestrator.Instance.ConfigSelectorStrategies.Select(x => x.Name)));
        }
    }
}
