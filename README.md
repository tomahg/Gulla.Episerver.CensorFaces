# Gulla.EpiserverCensorFaces for CMS 12

This is the readme for the CMS 12 version, the version for CMS 11 is [over here](https://github.com/tomahg/Gulla.Episerver.CensorFaces/tree/cms11).

## Censors eye region of faces

This addon will use Azure Cognitive Services Face API to detect faces, and place a black rectangle over the eye region of all detected faces, in images uploaded in Optimizely CMS (formerly Episerver CMS).

![Censored](images/censored.jpg)

## Prerequisites

-   Create an Azure Cognitive Services, Face API-resource using the Azure portal.
-   Add the following keys to appsettings.json. Get the values from the Azure portal.

```
  "Gulla": {
    "EpiserverCensorFaces": {
      "CognitiveServices": {
        "SubscriptionKey": "my-subscription-key",
        "Endpoint": "https://my-service.cognitiveservices.azure.com/"
      }
    }
  }
```

- Add the following to `Configure` in `Startup.cs` and add IContentEvents as an input paramter if not already present
```
public void Configure(IApplicationBuilder app, IContentEvents contentEvents)
{
  app.UseCensorFaces(contentEvents);
}
```

## More information

Check out [this blog post](https://www.gulla.net/no/blog/episerver-image-anonymization-using-microsoft-cognitive-services-and-face-api/) or [this blog post](https://www.gulla.net/no/blog/personvernvennlig-bildeopplasting/).

## Get it

Install the NuGet package [Gulla.Episerver.CensorFaces](https://nuget.optimizely.com/package/?id=Gulla.Episerver.CensorFaces). You need to check «Include prerelease» in Visual Studio, or your packet manager.
