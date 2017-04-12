# Application Insights Best Practices
Tried to gather various best practices for using Application Insights. Starting with ASP .NET MVC and Web API projects and including various suggestions gathered from the community and the Application Insights team.

## Web API
Including Application Insights in a .NET Web API project is a fairly easy task and with Visual Studio 2015 onwards it is as easy as ticking a checkbox in the new project template, or via the dropdown menu in each Web project. You can also add it by adding the Microsoft.ApplicationInsights.Web Nuget package along with 6 other dependencies. Application Insights will also help you track your onboarding progress via the Application Insights extension which is part of VS15 and VS17. However there might still be work needed to maximize your AI experience.

### 1. Dependencies and async operations don't correlate with the Request
One of the important features in the Application Insights is viewing all Telemetry for the same operation which essentially means that for a Web API action, each custom event, dependency, exception, trace should all be linked with the request. Unfortunately this doesn't quite work for a few async scenarios. After a bit of testing I've observed the correlation fail in two scenarios:
* Any asychronous dependency call (HttpClient, SQL calls, etc)

  ```cs
  await (new HttpClient()).GetAsync("http://www.github.com");
  ```
* Any telemetry logged on a background thread (including custom events, exceptions etc)

  ```cs
  Task.Run(async () =>
  {
      telemetryClient.TrackEvent("SampleCustomEventOnBackgroundThread");
      telemetryClient.TrackException(new System.InvalidOperationException("Background Thread Exception"));
  });
  ```

 
